using Abyat.Shared;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.IO;
using System.Linq;
using System;

namespace Abyat
{
    class Program
    {
        static void Main(string[] args)
        {
            var provider = ConfigureServices();

            var parsers = provider.GetServices<ISportParser>();

            //Process Data Files
            var dataFiles = Directory.GetFiles("Data", "*.txt");
            if (dataFiles.Count() == 0)
            {
                Console.WriteLine("0 files found...");
                return;
            }

            var tournament = new Tournament();

            foreach (var file in dataFiles)
            {
                var fileType = Utils.GetFileType(file);

                var parser = parsers.FirstOrDefault(p => p.Name == fileType);

                if (parser == null)
                {
                    Console.WriteLine("Unsupported Sport Type...");
                    return;
                }

                var fileName = System.IO.Path.GetFileName(file);

                Console.WriteLine($"Parsing file {fileName}...");

                var (success, message) = parser.ProcessFile(file);
                if (!success)
                {
                    if (!string.IsNullOrEmpty(message))
                    {
                        Console.WriteLine(message);
                    }
                    else
                    {
                        Console.WriteLine($"The File \"{fileName}\" is invalid.");
                    }
                    return;
                }

                tournament.UpdateScores(parser.GetScores());
            }

            var mvp = tournament.GetMVP();
            var mvpScore = tournament.GetPlayerScore(mvp);

            Console.WriteLine($"The tournament MVP is {mvp} with a total of {mvpScore} points");
        }

        static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            var iType = typeof(ISportParser);

            foreach (string dll in Directory.GetFiles("Plugins", "*.dll"))
            {
                var assembly = Assembly.LoadFrom(dll);
                var types = assembly.GetTypes().Where(p => iType.IsAssignableFrom(p));
                foreach (var type in types)
                {
                    services.AddSingleton(iType, type);
                }
            }

            return services.BuildServiceProvider();
        }
    }
}
