using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace console_call_generator_pdf
{
    class Program
    {
        static void Main(string[] args)
        {
            var order = CreateRequests();

            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "node.exe",
                    Arguments = $"index.js \"{order}\"",
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    RedirectStandardOutput = false
                };

                using var process = Process.Start(processStartInfo);
                process.WaitForExit();

                CombineFiles(order);
            }
            finally
            {
                Directory.Delete(order, recursive: true);
            }
        }

        private static void CombineFiles(string order)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var outputDocument = new PdfDocument();
            foreach (var file in Directory.GetFiles(order))
            {
                var inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);
                int count = inputDocument.PageCount;
                for (int idx = 0; idx < count; idx++)
                {
                    PdfPage page = inputDocument.Pages[idx];
                    outputDocument.AddPage(page);
                }
            }

            outputDocument.Save($"{order}.pdf");
        }

        private static string CreateRequests()
        {
            // DocDefinition, coloquei em um arquivo txt para o editor não ficar lento.
            var objectPdf = File.ReadAllText("request-file.txt");

            var directory = Guid.NewGuid().ToString();
            Directory.CreateDirectory(directory);

            foreach (var index in Enumerable.Range(0, 100))
            {
                File.WriteAllText(Path.Combine(directory, $"request_{index}.js"), $"module.exports = {objectPdf}");
            }

            return directory;
        }
    }
}
