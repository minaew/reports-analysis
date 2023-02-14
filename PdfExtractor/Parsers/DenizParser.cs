using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using PdfExtractor.Models;

namespace PdfExtractor.Parsers
{
    public class DenizParser : IParser
    {
        private const string Python = "python";
        private const string Script = "deniz_parser.py";
        private const string WorkingDirectory = "C:\\Users\\manai\\source\\repos\\reports-analysis\\";

        public IEnumerable<Operation> Parse(string path)
        {
            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = Python,
                Arguments = $"\"{Script}\" \"{path}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = WorkingDirectory
            });
            if (process == null) throw new InvalidOperationException();

            while (true)
            {
                var line = process.StandardOutput.ReadLine();
                if (line == null) break;

                var operation = JsonSerializer.Deserialize<Operation>(line);
                yield return operation;
            }

            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                var error = process.StandardError.ReadToEnd();
                throw new ParsingException(error);
            }
        }
    }
}
