#include <iostream>
#include <filesystem>
#include <fstream>
#include <vector>
#include <string>

namespace fs = std::filesystem;

int main() {
    // 로그 출력
    std::cout << "JTL 파일 병합기 시작" << std::endl;

    // 현재 디렉토리에서 *.jtl.gz 파일 목록을 가져온다.
    std::vector<fs::path> files;
    for (const auto& entry : fs::directory_iterator(fs::current_path())) {
        if (entry.is_regular_file() && entry.path().extension() == ".gz" && entry.path().stem().extension() == ".jtl") {
            files.push_back(entry.path());
        }
    }

    if (files.empty()) {
        std::cerr << "No *.jtl.gz files found." << std::endl;
        return 1;
    }

    // 병합 파일을 스트리밍 방식으로 열어둔다.
    std::ofstream out("merged.jtl", std::ios::binary);
    if (!out) {
        std::cerr << "Failed to open output file." << std::endl;
        return 1;
    }

    bool firstFile = true;
    for (const auto& file : files) {
        // gzip 파일을 압축 해제하지 않고 스트림으로 읽는다.
        // 여기서는 시스템 명령어를 사용해 gunzip -c 로 압축을 해제하고
        // 파이프를 통해 읽는 예시를 보여준다. 실제 구현에서는 zlib 등
        // 라이브러리를 사용해 직접 압축 해제할 수 있다.
        std::string cmd = "gunzip -c \"" + file.string() + "\"";
        FILE* pipe = popen(cmd.c_str(), "r");
        if (!pipe) {
            std::cerr << "Failed to decompress " << file << std::endl;
            continue;
        }
        std::ifstream in(pipe, std::ios::binary);
        if (!in) {
            std::cerr << "Failed to open pipe for " << file << std::endl;
            pclose(pipe);
            continue;
        }

        if (firstFile) {
            // 첫 번째 파일은 헤더를 그대로 복사한다.
            out << in.rdbuf();
            firstFile = false;
        } else {
            // 이후 파일은 헤더(첫 번째 라인)를 건너뛴 뒤 복사한다.
            std::string dummy;
            std::getline(in, dummy); // 헤더 라인 스킵
            out << in.rdbuf();
        }
        pclose(pipe);
    }

    out.close();
    std::cout << "Merged file created: merged.jtl" << std::endl;
    return 0;
}
