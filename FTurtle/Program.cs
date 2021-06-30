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
        /// <summary>
        /// Main function can do something useful IMHO. Here:
        /// It gets the file name as an argument, verifies and sanitizes it.
        /// Then Main() reads and sanitizes the content of the file.
        /// After that, it builds the configuration and passes control to the application "engine".
        /// </summary>
        /// <param name="args">arg[0] contains the trial configuration file name</param>
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please enter a file name.");
                Console.WriteLine("Usage: FTurtle <file_name>");
                return;
            }

            // Assume that file is in the local (or NFS-like) filesystem and not a URI of a remote host.

            var fileName = GetCleanedFileName(args[0]);
            if (fileName.Length == 0)
            {
                Console.WriteLine("Please enter a valid file name. '{0}' is not acceptable.", args[0]);
                Console.WriteLine("Usage: FTurtle <file_name>");
                return;
            }

            IList<string> rawData; // All valid lines will go here. We need an ordered collection for the line meaning is defined by it's number.

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

            // The first line should define the board size.
            // The second line should contain a list of mines (i.e. list of co-ordinates separated by a space).
            // The third line of the file should contain the exit point.
            // The fourth line of the file should contain the starting position of the turtle.
            // The fifth line to the end of the file should contain a series of moves. 
            if (rawData.Count < 5)
            {
                Console.WriteLine("'{0}' does not contain valid data.", fileName);
                return;
            }

            
 //           string currentDirName = System.IO.Directory.GetCurrentDirectory();
        }

        /// <summary>
        /// Also verifies and sanitizes at basic level
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static IList<string> ReadRawConfig(TextReader file)
        {
            var data = new List<string>();

            // Accept digits, spaces, comma, and valid commands in either case. At least 1 per line.
            var validator = new Regex(@"[\d\s,RLMNSEW]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            string rawLine;

            // line by line to handle big files.
            // In any case, the program assumes that all valid lines can fit into RAM.

            while ((rawLine = file.ReadLine()) != null)
            {
                var line = rawLine.Trim().TrimEnd(',').TrimStart(',');
                if (line.Length == 0 || !validator.IsMatch(line))
                {
                    continue;
                }

                line = Regex.Replace(line, @"\s+", " ").ToUpper(); // multiple spaces to a single one and string to uppercase
                line = Regex.Replace(line, @"\s*,\s*", ","); // no spaces around comma
                line = Regex.Replace(line, @",+", ","); // multiple commas to a single one
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
