package com.example.app;

import java.io.*;
import java.nio.charset.StandardCharsets;
import java.util.Arrays;
import java.util.Comparator;
import java.util.zip.GZIPInputStream;

public class App {
    public static void main(String[] args) {
        System.out.println("JTL 파일 병합기 시작");
        try {
            File currentDir = new File(".");
            File[] gzFiles = currentDir.listFiles((dir, name) -> name.endsWith(".jtl.gz"));
            if (gzFiles == null || gzFiles.length == 0) {
                System.out.println("압축된 JTL 파일이 없습니다.");
                return;
            }
            // 파일을 정렬(옵션) - 예: 파일명 기준
            Arrays.sort(gzFiles, Comparator.comparing(File::getName));

            File outputFile = new File("merged.jtl");
            try (BufferedWriter writer = new BufferedWriter(new OutputStreamWriter(new FileOutputStream(outputFile), StandardCharsets.UTF_8))) {
                boolean isFirstFile = true;
                for (File gzFile : gzFiles) {
                    try (GZIPInputStream gzipIn = new GZIPInputStream(new FileInputStream(gzFile));
                         BufferedReader reader = new BufferedReader(new InputStreamReader(gzipIn, StandardCharsets.UTF_8))) {
                        String line;
                        if (isFirstFile) {
                            // 첫 번째 파일: 헤더를 포함해 그대로 복사
                            while ((line = reader.readLine()) != null) {
                                writer.write(line);
                                writer.newLine();
                            }
                            isFirstFile = false;
                        } else {
                            // 이후 파일: 첫 번째 라인(헤더)을 건너뛰고 복사
                            // 헤더 라인 읽기
                            reader.readLine();
                            while ((line = reader.readLine()) != null) {
                                writer.write(line);
                                writer.newLine();
                            }
                        }
                    }
                }
            }
            System.out.println("병합 완료: " + outputFile.getAbsolutePath());
        } catch (IOException e) {
            System.err.println("병합 중 오류 발생: " + e.getMessage());
            e.printStackTrace();
        }
    }
}
