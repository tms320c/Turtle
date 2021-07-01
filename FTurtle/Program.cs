using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using FTurtle.Application;
using FTurtle.Domain;
using FTurtle.Infrastructure;
using TurtleWorld.Core;
using TurtleWorld.Structure;

namespace FTurtle
{
    public class Program
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

            try
            {
                var builder = ConfigurationFactory.GetBuilder();
                using var file = new StreamReader(fileName);

                ReadRawConfig(file, builder);

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

            IConfiguration config;
            try
            {
                config = ConfigurationFactory.GetConfiguration();
            }
            catch (ApplicationException ex)
            {
                Console.WriteLine(ex.GetType() + " " + ex.Message);
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.WriteLine("Got config");

            // Here we can use DI to instantiate IPathMapper, IPathTokenizer, and boundary collision detection
            // But I believe it is way too much in this case
            var tokenizer = new PathTokenizer();
            var mapper = new PathMapper(config.Board);

            // build runner and give it simple boundary collision handling, which is clipping (one may just use null as the parameter value for this)
            var runner = new TrialRunner(config, mapper, tokenizer, (p, b) => new Position
            {
                // Simple clipping strategy.
                // The turtle stays by the boundary until a rotation command (or end of the path)
                X = p.X >= b.Height
                    ? b.Height - 1
                    : (p.X < 0
                        ? 0
                        : p.X),
                Y = p.Y >= b.Width
                    ? b.Width - 1
                    : (p.Y < 0
                        ? 0
                        : p.Y),
                // Heading does not matter
            });

            runner.Run(Console.WriteLine, true);
        }

        /// <summary>
        /// Read file line by line and delegates the job to a builder
        /// </summary>
        /// <param name="file">File to read</param>
        /// <param name="builder">Builder - the data processor</param>
        /// <returns></returns>
        private static void ReadRawConfig(TextReader file, Action<string> builder)
        {

            string rawLine;

            // line by line to handle big files.
            // In any case, the program assumes that all valid lines can fit into RAM.

            while ((rawLine = file.ReadLine()) != null)
            {
#if DEBUG
                Console.WriteLine(rawLine);
#endif
                builder?.Invoke(rawLine);
            }
        }

        private static string GetCleanedFileName(string fileName)
        {
            return (new Regex($"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()))}]",
                    RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.CultureInvariant))
                .Replace(fileName.Trim(), "");
        }
    }
}
