use std::fs::{self, File};
use std::io::{self, BufRead, BufReader, BufWriter, Write, Read};
use std::path::Path;
use std::process::Command;

fn main() -> io::Result<()> {
    // 로그 출력
    println!("JTL 파일 병합기 시작");

    // 현재 디렉토리에서 *.jtl.gz 파일 목록 가져오기
    let mut files: Vec<std::path::PathBuf> = Vec::new();
    for entry in fs::read_dir(".")? {
        let entry = entry?;
        let path = entry.path();
        if path.is_file() && path.extension().and_then(|s| s.to_str()) == Some("gz") {
            if let Some(stem) = path.file_stem() {
                if stem.to_str().map_or(false, |s| s.ends_with(".jtl")) {
                    files.push(path);
                }
            }
        }
    }

    // 파일이 없으면 종료
    if files.is_empty() {
        eprintln!("No *.jtl.gz files found in the current directory.");
        return Ok(());
    }

    // merged.jtl 파일 열기 (쓰기 모드, 기존 파일이 있으면 덮어쓰기)
    let output = File::create("merged.jtl")?;
    let mut writer = BufWriter::new(output);

    // 첫 번째 파일은 헤더를 그대로 쓰고, 이후 파일은 헤더를 건너뛰어야 함
    let mut first_file = true;

    for file_path in files {
        // gzip -dc 명령어를 사용해 파일을 스트리밍 방식으로 압축 해제
        let mut child = Command::new("gzip")
            .arg("-dc")
            .arg(&file_path)
            .stdout(std::process::Stdio::piped())
            .spawn()?;

        let stdout = child.stdout.take().expect("Failed to capture stdout");
        let mut reader = BufReader::new(stdout);

        let mut line = String::new();
        let mut is_first_line = true;
        loop {
            line.clear();
            let bytes_read = reader.read_line(&mut line)?;
            if bytes_read == 0 {
                break; // EOF
            }

            if first_file {
                // 첫 번째 파일은 헤더를 그대로 쓰고, 이후 파일은 헤더를 건너뜀
                writer.write_all(line.as_bytes())?;
                first_file = false;
            } else if is_first_line {
                // 이후 파일의 첫 번째 라인은 헤더이므로 건너뜀
                is_first_line = false;
                continue;
            } else {
                writer.write_all(line.as_bytes())?;
            }
        }

        // 자식 프로세스가 종료될 때까지 기다림
        child.wait()?;
    }

    writer.flush()?;
    println!("merged.jtl 파일이 생성되었습니다.");
    Ok(())
}
