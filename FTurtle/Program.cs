using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using FTurtle.Application;
using FTurtle.Domain;

namespace FTurtle
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please enter a file name.");
                Console.WriteLine("Usage: FTurtle <file_name>");
                return;
            }

            var fileName = GetCleanedFileName(args[0]);
            if (fileName.Length == 0)
            {
                Console.WriteLine("Please enter a valid file name. '{0}' is not acceptable.", args[0]);
                Console.WriteLine("Usage: FTurtle <file_name>");
                return;
            }

            IList<string> rawData;

            try
            {
                using var file = new StreamReader(fileName);
                rawData = ReadRawConfig(file);
                file.Close();
            }
            catch (NotSupportedException)
            {
                Console.WriteLine("'{0}' is not a file.", fileName); // the original message is too verbose.
                return;
            }
            catch (Exception ex) // FileNotFoundException, IOException, maybe PathTooLongException, etc
            {
                Console.WriteLine(ex.Message);
                return;
            }

            if (rawData.Count == 0)
            {
                Console.WriteLine("'{0}' does not contain any valid data.", fileName);
                return;
            }

            
            string currentDirName = System.IO.Directory.GetCurrentDirectory();
        }

        private static IList<string> ReadRawConfig(TextReader file)
        {
            var data = new List<string>();

            // Accept digits, spaces, comma, and valid commands in either case. At least 1 per line.
            var validator = new Regex(@"[\d\s,rRlLmMnNsSeEwW]+");

            string rawLine;

            while ((rawLine = file.ReadLine()) != null)
            {
                var line = rawLine.Trim();
                if (line.Length == 0 || !validator.IsMatch(line))
                {
                    continue;
                }
                line = Regex.Replace(line, @"\s+", " ").ToUpper(); // multiple spaces to a single one and to uppercase
#if DEBUG
                System.Console.WriteLine(line);
#endif
                data.Add(line);
            }

            return data;
        }

        private static string GetCleanedFileName(string fileName)
        {
            return (new Regex($"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()))}]",
                    RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.CultureInvariant))
                .Replace(fileName.Trim(), "");
        }
    }
}
