// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace SceneArch
{
	[Register ("MainWindow")]
	partial class MainWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}

	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSButton btnBrowseFile { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnExtract { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextView txtConsoleOutput { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtFilename { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnBrowseFile != null) {
				btnBrowseFile.Dispose ();
				btnBrowseFile = null;
			}

			if (btnExtract != null) {
				btnExtract.Dispose ();
				btnExtract = null;
			}

			if (txtConsoleOutput != null) {
				txtConsoleOutput.Dispose ();
				txtConsoleOutput = null;
			}

			if (txtFilename != null) {
				txtFilename.Dispose ();
				txtFilename = null;
			}
		}
	}
}
