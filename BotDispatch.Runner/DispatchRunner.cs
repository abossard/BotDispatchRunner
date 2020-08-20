using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Fuse.Dispatch;

namespace BotDispatch.Runner
{
    public class DispatchRunner
    {
        private readonly string workingDirectory;

        public DispatchRunner(string workingDirectory)
        {
            Directory.CreateDirectory(workingDirectory);
            this.workingDirectory = workingDirectory;
        }

        public async Task<ProcessAsyncHelper.ProcessResult> RunDispatchAsync(string arg)
        {
            var asm = Assembly.GetExecutingAssembly();
            var path = System.IO.Path.GetDirectoryName(asm.Location);
            var arguments = Path.Join(path, "Dispatch.dll") + " " + arg;
            return await ProcessAsyncHelper.ExecuteShellCommand(workingDirectory, "dotnet", arguments, 1000);
        }
    }
}