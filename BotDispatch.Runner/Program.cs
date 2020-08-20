using System;
using System.IO;
using System.Threading.Tasks;

namespace BotDispatch.Runner
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var workingDirectory = Path.Join(Path.GetTempPath(), "dispatch");
            var dispatchRunner = new DispatchRunner(workingDirectory);
            var result = await dispatchRunner.RunDispatchAsync("-h");

            Console.WriteLine(result.Output);
            Console.WriteLine(result.ErrorOutput);
        }
    }
}
