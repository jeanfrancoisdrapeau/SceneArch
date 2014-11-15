using System;
using System.Collections.Concurrent;
using System.Diagnostics;

using MonoMac.CoreFoundation;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace SceneArch
{
	public class ExtractTask
	{
		public string filename;
		public string workingdir;
		public string args;

		public bool done = false;

		public BlockingCollection<string> messages = new BlockingCollection<string>();

		public ExtractTask(string filename, string workingdir, string args)
		{
			this.filename = filename;
			this.workingdir = workingdir;
			this.args = args;
		}

		public void Task()
		{
			Process execProcess;
			execProcess = new Process();
			execProcess.StartInfo.FileName = filename;
			execProcess.StartInfo.WorkingDirectory = workingdir;
			execProcess.StartInfo.Arguments = args;
			// Set UseShellExecute to false for redirection.
			execProcess.StartInfo.CreateNoWindow = true;
			execProcess.StartInfo.UseShellExecute = false;

			// Redirect the standard output of the sort command.  
			// This stream is read asynchronously using an event handler.
			execProcess.StartInfo.RedirectStandardOutput = true;

			// Set our event handler to asynchronously read the sort output.
			execProcess.OutputDataReceived += new DataReceivedEventHandler(consoleOutputHandler);

			// Redirect standard input as well.  This stream
			// is used synchronously.
			execProcess.StartInfo.RedirectStandardInput = true;

			// Start the process.
			execProcess.Start();

			// Start the asynchronous read of the sort output stream.
			execProcess.BeginOutputReadLine();

			while (!execProcess.HasExited) {
				NSRunLoop.Current.RunUntil(DateTime.Now.AddMilliseconds(100)); // This keeps your form responsive by processing events
			}
		}

		private void consoleOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
		{
			messages.Add (outLine.Data + "\n");
		}
	}
}

