using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace SceneArch
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MainWindowController mainWindowController;

		bool launchedFromFile = false;
		string fileName = string.Empty;

		public AppDelegate ()
		{
		}

		public override void FinishedLaunching (NSObject notification)
		{
			if (launchedFromFile)
				mainWindowController = new MainWindowController (fileName, true);
			else
				mainWindowController = new MainWindowController ();

			mainWindowController.Window.MakeKeyAndOrderFront (this);

			mnuOpen.Activated += event_mnuOpen_Activated;
		}

		public void event_mnuOpen_Activated(object sender, EventArgs e)
		{
			var openPanel = new NSOpenPanel();
			openPanel.ReleasedWhenClosed = true;
			openPanel.Prompt = "Select file";
			openPanel.AllowedFileTypes = new string[] {"zip","rar","001"};
			openPanel.AllowsMultipleSelection = false;
			openPanel.CanChooseDirectories = false;
			openPanel.CanCreateDirectories = false;

			var result = openPanel.RunModal();
			if (result == 1) {
				mainWindowController = new MainWindowController (openPanel.Url.Path, false);
				mainWindowController.Window.MakeKeyAndOrderFront (this);
			} else {
				return;
			}
		}

		public override bool OpenFile (NSApplication sender, string filename)
		{
			if (!filename.ToLower().EndsWith(".zip") && 
				!filename.ToLower().EndsWith(".rar") &&
				!filename.ToLower().EndsWith(".001"))
				return false;

			launchedFromFile = true;
			fileName = filename;

			return true;
		}
	}
}

