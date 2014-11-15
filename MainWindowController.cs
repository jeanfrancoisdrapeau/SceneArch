using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Timers;
using MonoMac.CoreFoundation;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace SceneArch
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{
		#region Constructors

		public string startupFilename = string.Empty;
		public bool doItNow = false;

		int LinesAdded = 0;

		private ExtractTask extractTask;

		System.Timers.Timer _timerUi;

		// Called when created from unmanaged code
		public MainWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		// Call to load from the XIB/NIB file
		public MainWindowController () : base ("MainWindow")
		{
			Initialize ();
		}
		public MainWindowController (string filename, bool doitnow) : base ("MainWindow")
		{
			Initialize ();
			this.startupFilename = filename;
			this.doItNow = doitnow;
		}
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new MainWindow Window {
			get {
				return (MainWindow)base.Window;
			}
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			Window.Title += " " + NSBundle.MainBundle.InfoDictionary ["CFBundleVersion"];

			btnExtract.Activated += event_btnExtract_Activated;
			btnBrowseFile.Activated += event_btnBrowseFile_Activated;
		}

		public override void WindowDidLoad ()
		{
			base.WindowDidLoad ();

			_timerUi = new System.Timers.Timer ();
			_timerUi.Interval = 100;
			_timerUi.Elapsed += new System.Timers.ElapsedEventHandler(timerElapsedUi);

			if (!string.IsNullOrEmpty (startupFilename)) {
				txtFilename.StringValue = startupFilename;
				if (doItNow) {
					doExtract ();
				}
			}
		}

		void timerElapsedUi (object sender, ElapsedEventArgs e)
		{
			InvokeOnMainThread (delegate {

				if (LinesAdded > 500)
				{
					txtConsoleOutput.Value = string.Empty;
					LinesAdded = 0;
				}

				if (extractTask != null)
				{
					while (extractTask.messages.Count > 0)
					{
						txtConsoleOutput.Value += extractTask.messages.Take();
						txtConsoleOutput.ScrollRangeToVisible(new NSRange(txtConsoleOutput.Value.Length, 0));
						LinesAdded += 1;
					}
				}
			
				if (extractTask != null) {
					if (extractTask.done) {
						extractTask = null;
						_timerUi.Stop ();

						if (doItNow) {
							txtConsoleOutput.Value += "\n\nThe application will close in 3 secs...";
							txtConsoleOutput.ScrollRangeToVisible(new NSRange(txtConsoleOutput.Value.Length, 0));
							NSRunLoop.Current.RunUntil (DateTime.Now.AddSeconds (3));
							NSApplication.SharedApplication.Terminate (this);
						}
					}
				}
			});
		}

		public void event_btnBrowseFile_Activated(object sender, EventArgs e)
		{
			var openPanel = new NSOpenPanel();
			openPanel.ReleasedWhenClosed = true;
			openPanel.Prompt = "Select file";
			openPanel.AllowedFileTypes = new string[] {"zip","rar"};
			openPanel.AllowsMultipleSelection = false;
			openPanel.CanChooseDirectories = false;
			openPanel.CanCreateDirectories = false;

			var result = openPanel.RunModal();
			if (result == 1) {
				txtFilename.StringValue = openPanel.Url.Path;
			} else {
				return;
			}
		}

		public void event_btnExtract_Activated(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty (txtFilename.StringValue)) {
				var alert = new NSAlert {
					MessageText = "Input file and/or output directory is empty.",
					AlertStyle = NSAlertStyle.Informational
				};

				alert.AddButton ("Ok");

				alert.RunModal();
				return;
			}

			doExtract ();
		}

		private void doExtract ()
		{
			string ext = Path.GetExtension (txtFilename.StringValue).ToUpper();
			string extdir = Path.GetDirectoryName (txtFilename.StringValue) + "/" + Path.GetFileNameWithoutExtension(txtFilename.StringValue) + "/";
			string arguments = string.Empty;
			string filename = string.Empty;
			string pathres = System.IO.Path.GetDirectoryName (System.Reflection.Assembly.GetExecutingAssembly().Location) +
				"/../Resources/Resources/";
			switch (ext) {
			case ".ZIP":
				//filename = "/usr/bin/unzip";

				var alert = new NSAlert {
					MessageText = "Extract only this file or all the ZIP files in the directory?",
					AlertStyle = NSAlertStyle.Critical
				};

				alert.AddButton ("This file only");
				alert.AddButton ("All the files");

				string fileToExtract = "\\*.zip";
				if (alert.RunModal () == (int)NSAlertButtonReturn.First)
					fileToExtract = "\"" + txtFilename.StringValue + "\"";
					
				filename = pathres + "unzip";
				arguments = "-u -o " + fileToExtract + " -d \"" + extdir + "\"";
				break;
			case ".001":
			case ".RAR":
				filename = pathres + "unrar";
				arguments = "x -y \"" + txtFilename.StringValue + "\" \"" + extdir + "\"";
				break;
			}

			string workingdir = Path.GetDirectoryName (txtFilename.StringValue);
			extractTask = new ExtractTask (filename, workingdir, arguments);

			_timerUi.Start ();

			extractTask.messages.Add ("-------- STARTED\n");
			extractTask.messages.Add ("-------- Using: " + filename + "\n");

			var bw = new BackgroundWorker();

			bw.DoWork += (sender, args) => {
				Console.WriteLine("Worker has started");
				extractTask.Task();
			};

			bw.RunWorkerCompleted += (sender, args) => {
				Console.WriteLine("Worker has completed");
				extractTask.messages.Add("-------- DONE!\n");
				extractTask.messages.Add("-------- Extracted to: " + extdir);
				extractTask.done = true;
			};

			bw.RunWorkerAsync();
		}
	}
}

