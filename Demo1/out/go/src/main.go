package main

import (
    "bufio"
    "compress/gzip"
    "fmt"
    "io"
    "os"
    "path/filepath"
)

func main() {
    fmt.Println("JTL 파일 병합기 시작")

    files, err := filepath.Glob("*.jtl.gz")
    if err != nil {
        fmt.Println("Error globbing:", err)
        return
    }
    if len(files) == 0 {
        fmt.Println("No .jtl.gz files found")
        return
    }

    outFile, err := os.Create("merged.jtl")
    if err != nil {
        fmt.Println("Error creating output file:", err)
        return
    }
    defer outFile.Close()

    first := true
    for _, f := range files {
        gzFile, err := os.Open(f)
        if err != nil {
            fmt.Println("Error opening", f, ":", err)
            continue
        }
        gzReader, err := gzip.NewReader(gzFile)
        if err != nil {
            fmt.Println("Error creating gzip reader for", f, ":", err)
            gzFile.Close()
            continue
        }

        if first {
            _, err = io.Copy(outFile, gzReader)
            if err != nil {
                fmt.Println("Error copying data:", err)
            }
            first = false
        } else {
            // Skip header line
            br := bufio.NewReader(gzReader)
            _, err = br.ReadString('\n')
            if err != nil && err != io.EOF {
                fmt.Println("Error reading header from", f, ":", err)
            }
            _, err = io.Copy(outFile, br)
            if err != nil {
                fmt.Println("Error copying data:", err)
            }
        }

        gzReader.Close()
        gzFile.Close()
    }

    fmt.Println("Merge completed to merged.jtl")
}
