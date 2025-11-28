import os
import glob
import gzip


def main():
    print("JTL 파일 병합기 시작")

    # 현재 디렉토리에서 *.jtl.gz 파일 목록을 가져옵니다.
    files = sorted(glob.glob("*.jtl.gz"))
    if not files:
        print("No .jtl.gz files found.")
        return

    # merged.jtl 파일을 바이너리 모드로 엽니다.
    with open("merged.jtl", "wb") as outfile:
        for idx, file_path in enumerate(files):
            with gzip.open(file_path, "rb") as infile:
                # 첫 번째 파일은 헤더를 그대로 쓰고, 이후 파일은 헤더를 건너뜁니다.
                header = infile.readline()
                if idx == 0:
                    outfile.write(header)
                # 헤더 이후의 데이터를 스트리밍 방식으로 복사합니다.
                while True:
                    chunk = infile.read(1024 * 1024)  # 1MB 단위로 읽기
                    if not chunk:
                        break
                    outfile.write(chunk)

    print("Merged file created: merged.jtl")


if __name__ == "__main__":
    main()
