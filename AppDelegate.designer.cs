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
	[Register ("AppDelegate")]
	partial class AppDelegate
	{
		[Outlet]
		MonoMac.AppKit.NSMenuItem mnuOpen { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (mnuOpen != null) {
				mnuOpen.Dispose ();
				mnuOpen = null;
			}
		}
	}
}
