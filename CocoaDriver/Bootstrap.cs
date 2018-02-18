using System;
using AppKit;
using Foundation;

public class Bootstrap {
	public static object CreateInstance ()
	{
		NSApplication.Init ();

		return new System.Windows.Forms.XplatUICocoa ();
	}
}
