using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using PdfExtractor.Models;

namespace PdfExtractor.Parsers
{
    public abstract class PythonParser : IParser
    {
        private readonly string _python = "python";
        private readonly string _workingDirectory = "C:\\Users\\manai\\source\\repos\\reports-analysis\\";
        private readonly string _script;

        protected PythonParser(string script)
        {
            _script = script;
        }

        public virtual IEnumerable<Operation> Parse(string path)
        {
            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = _python,
                Arguments = $"\"{_script}\" \"{path}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = _workingDirectory
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
