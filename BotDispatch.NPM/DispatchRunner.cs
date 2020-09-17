using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace BotDispatch.NPM
{
    public class DispatchRunner
    {
        private readonly string _workingDirectory;

        public DispatchRunner(string workingDirectory)
        {
            Directory.CreateDirectory(workingDirectory);
            _workingDirectory = workingDirectory;
        }

        public async Task<ProcessAsyncHelper.ProcessResult> RunDispatchAsync(string arg)
        {
            var asm = Assembly.GetExecutingAssembly();
            var path = Path.GetDirectoryName(asm.Location);
            var possibleDispatchLocations = new[]
            {
                Path.Combine(path ?? "./", "Dispatch.dll"),
                Path.Combine(path ?? "./", "publish", "Dispatch.dll"),
                Path.Combine(path ?? "./", "..", "DispatchTool", "Dispatch.dll"),
                Path.Combine(path ?? "./", "DispatchTool", "Dispatch.dll"),
            };

            string GetValidPathOrFail()
            {
                foreach (var possibleDispatchLocation in possibleDispatchLocations)
                {
                    if (File.Exists(possibleDispatchLocation))
                    {
                        return possibleDispatchLocation;
                    }
                }

                throw new FileNotFoundException($"Dispatch.dll is not found in these locations {possibleDispatchLocations}");
            }

            var arguments = GetValidPathOrFail() + " " + arg;
            return await ProcessAsyncHelper.ExecuteShellCommand(_workingDirectory, "dotnet", arguments, 4000);
        }
    }
}