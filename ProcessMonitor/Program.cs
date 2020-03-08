using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessMonitor
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {   
            using var cancelTokenSource = new CancellationTokenSource();

            Console.CancelKeyPress += (s, e) =>
            {
                cancelTokenSource.Cancel();
            };

            var services = new ServiceCollection()
                .AddSingleton<IProcessService, ProcessService>()
                .AddSingleton<ISleepService, SleepService>()
                .BuildServiceProvider();
            
            using var app = new CommandLineApplication<ProcessMonitorApp>();
            app.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(services);
            try
            {
#if DEBUG
                var res = await app.ExecuteAsync(new string[] {"notepad", "5", "1"}, cancelTokenSource.Token);
                Console.ReadKey(); 
#else
                var res = await app.ExecuteAsync(args, cancelTokenSource.Token);
#endif
                return res;
            }
            catch(CommandParsingException e)
            {
                Console.Error.WriteLine(e.Message);
                if(e is UnrecognizedCommandParsingException ue && ue.NearestMatches.Any())
                {
                    Console.Error.WriteLine("\nDid you mean this?");
                    Console.Error.WriteLine($"    {ue.NearestMatches.First()}");
                }
                return 1;
            }
        }
    }
}
