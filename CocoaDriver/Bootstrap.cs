using System;
public class Bootstrap {
	public static object CreateInstance ()
	{
		return new System.Windows.Forms.XplatUICocoa ();
	}
}
