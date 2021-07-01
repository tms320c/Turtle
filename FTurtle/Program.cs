using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        static async Task Main(string[] args)
        {
            // It could be so much better control if I use cmd line parameters, but
            // spec prohibits any third party packages except for the unit testing
            // and cmd line parsing would rather be a project on its own...
            // So, I limit myself on just one parameter - configuration file name.
            if (args.Length == 0)
            {
                Console.WriteLine("Please enter a file name.");
                Console.WriteLine("Usage: FTurtle <file_name>");
                return;
            }

            // Assume that file is in the local (or NFS-like mounted) filesystem and not a URI of remote host.

            var fileName = SanitizeFileName(args[0]);
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

                file.Close(); // to be on the safe side. 
                // Maybe, not required because of "using" and IDisposable.
                // Or, should it be in "finally"? But then it may run after Dispose(). Go figure...
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

            // I open new "try" block to avoid loooong list of "catch" clauses.
            // Plus, the possible exceptions in this block are of totally different semantic.
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

            // Here we can use DI to instantiate IPathMapper, IPathTokenizer, and boundary collision detection
            // But I believe it is way too much for this poor little turtle
            var tokenizer = new PathTokenizer();
            var mapper = new PathMapper(config.Board);

            // build runner and give it simple boundary collision handling, which is clipping (one may just use null as the parameter value for this)
            // there are more strategies in the unit tests

            var strategy = BoundaryAvoidanceFactory.Create(StrategyKind.Clip);

            var runner = new TrialRunner(config, mapper, tokenizer, strategy);

            await runner.Run(Console.WriteLine, true); // async - small step toward multi-turtle future.
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

        /// <summary>
        /// Never trust the user input!
        /// Super-smart cases like "LPT1" etc will be handled later by OS.
        /// </summary>
        /// <param name="fileName">Name of file as user has input it</param>
        /// <returns>File name that is closer to something that may exist in a file system. Or empty.</returns>
        private static string SanitizeFileName(string fileName)
        {
            return (new Regex($"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()))}]",
                    RegexOptions.Singleline | RegexOptions.CultureInvariant))
                .Replace(fileName.Trim(), "");
        }
    }
}
