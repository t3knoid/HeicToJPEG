using ImageConverters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeicToJPEG_cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new CommandLine();
            parser.Parse(args);

            if (parser.Arguments.Count > 0)
            {
                string filePath = String.Empty;

                // User must specify a fully qualified path
                if (parser.Arguments.ContainsKey("file"))
                {
                    try
                    {
                        filePath = parser.Arguments["file"][0];
                        // Check if file exist
                        if (!File.Exists(filePath))
                        {
                            Console.WriteLine("Cannot find" + filePath + "Exiting");
                            Environment.Exit(1);
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        usage();
                        Environment.Exit(1);
                    }
                }
                else
                {
                    usage();
                }

                // Perform conversion here
                try
                {
                    JPEGImage image = new JPEGImage(filePath);
                    image.ToFile();
                    Console.WriteLine("Conversion successful. Converted file available at " + image.ConvertedFilePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Conversion failed.");
                }

            }
            else
            {
                usage();
            }
        }
        static void usage()
        {
            Console.WriteLine("usage: HeicToJPEG-cmd -file pathToHEIC");
        }

    }
}
