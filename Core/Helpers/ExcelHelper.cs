using System;
using System.Diagnostics;

namespace ReportAnalysis.Core.Helpers
{
    internal static class ExcelHelper
    {
        public static string? GetString(string path, int row, int column)
        {
            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"get_string_from_excel.py \"{path}\" \"{row}\" \"{column}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = "C:\\Users\\manai\\source\\repos\\reports-analysis\\" // FIXME
            });
            if (process == null) throw new InvalidOperationException();

            var value = process.StandardOutput.ReadLine();

            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                var error = process.StandardError.ReadToEnd();
                throw new ParsingException(error);
            }

            return value;
        }

        public static string GetStringOrThrow(string path, int row, int column) =>
            GetString(path, row, column) ?? throw new InvalidOperationException("string is null");
    }
}
