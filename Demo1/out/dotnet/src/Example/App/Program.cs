using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Example.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("JTL 파일 병합기 시작");

            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.jtl.gz");
            if (files.Length == 0)
            {
                Console.WriteLine("병합할 파일이 없습니다.");
                return;
            }

            string mergedPath = Path.Combine(Directory.GetCurrentDirectory(), "merged.jtl");

            using (var mergedStream = new FileStream(mergedPath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var mergedWriter = new StreamWriter(mergedStream))
            {
                bool isFirstFile = true;
                foreach (var file in files.OrderBy(f => f))
                {
                    using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                    using (var gzip = new GZipStream(fileStream, CompressionMode.Decompress))
                    using (var reader = new StreamReader(gzip))
                    {
                        string line;
                        if (isFirstFile)
                        {
                            // 첫 번째 파일의 헤더를 유지
                            if ((line = reader.ReadLine()) != null)
                            {
                                mergedWriter.WriteLine(line);
                            }
                            isFirstFile = false;
                        }
                        else
                        {
                            // 이후 파일은 헤더를 건너뜀
                            reader.ReadLine();
                        }

                        // 나머지 라인들을 스트리밍 방식으로 병합
                        while ((line = reader.ReadLine()) != null)
                        {
                            mergedWriter.WriteLine(line);
                        }
                    }
                }
            }

            Console.WriteLine($"병합 완료: {mergedPath}");
        }
    }
}
