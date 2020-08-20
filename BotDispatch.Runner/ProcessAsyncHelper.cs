﻿namespace BotDispatch.Runner
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Threading.Tasks;
    /// <summary>
    /// Adapted and modernized from https://gist.github.com/AlexMAS/276eed492bc989e13dcce7c78b9e179d#file-processasynchelper-cs
    /// </summary>
    public static class ProcessAsyncHelper
    {
        public static async Task<ProcessResult> ExecuteShellCommand(string workingDirectory, string command, string arguments, int timeout)
        {
            var result = new ProcessResult();

            using var process = new Process
            {
                StartInfo =
                {
                    FileName = command,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDirectory
                }
            };

            var outputBuilder = new StringBuilder();
            var outputCloseEvent = new TaskCompletionSource<bool>();

            process.OutputDataReceived += (s, e) =>
            {
                // The output stream has been closed i.e. the process has terminated
                if (e.Data == null)
                {
                    outputCloseEvent.SetResult(true);
                }
                else
                {
                    outputBuilder.AppendLine(e.Data);
                }
            };

            var errorBuilder = new StringBuilder();
            var errorCloseEvent = new TaskCompletionSource<bool>();

            process.ErrorDataReceived += (s, e) =>
            {
                // The error stream has been closed i.e. the process has terminated
                if (e.Data == null)
                {
                    errorCloseEvent.SetResult(true);
                }
                else
                {
                    errorBuilder.AppendLine(e.Data);
                }
            };

            bool isStarted;

            try
            {
                isStarted = process.Start();
            }
            catch (Exception error)
            {
                // Usually it occurs when an executable file is not found or is not executable

                result.Completed = true;
                result.ExitCode = -1;
                result.Output = error.Message;

                isStarted = false;
            }

            if (!isStarted) return result;
            // Reads the output stream first and then waits because deadlocks are possible
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // Creates task to wait for process exit using timeout
            var waitForExit = WaitForExitAsync(process, timeout);

            // Create task to wait for process exit and closing all output streams
            var processTask = Task.WhenAll(waitForExit, outputCloseEvent.Task, errorCloseEvent.Task);

            // Waits process completion and then checks it was not completed by timeout
            if (await Task.WhenAny(Task.Delay(timeout), processTask) == processTask && waitForExit.Result)
            {
                result.Completed = true;
                result.ExitCode = process.ExitCode;
                result.Output = outputBuilder.ToString();
                result.ErrorOutput = errorBuilder.ToString();
            }
            else
            {
                try
                {
                    // Kill hung process
                    process.Kill();
                }
                catch
                {
                    // ignored
                }
            }

            return result;
        }


        private static Task<bool> WaitForExitAsync(Process process, int timeout)
        {
            return Task.Run(() => process.WaitForExit(timeout));
        }


        public struct ProcessResult
        {
            public bool Completed;
            public int? ExitCode;
            public string Output;
            public string ErrorOutput;
        }
    }
}