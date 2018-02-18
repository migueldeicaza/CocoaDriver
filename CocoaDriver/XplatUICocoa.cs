using System;
using System.Collections;
using System.Threading;
using System.Drawing;
using AppKit;

namespace System.Windows.Forms {
	internal class XplatUICocoa : XplatUIDriver {
		internal override int CaptionHeight => throw new NotImplementedException ();

		internal override Size CursorSize => throw new NotImplementedException ();

		internal override bool DragFullWindows => throw new NotImplementedException ();

		internal override Size DragSize => throw new NotImplementedException ();

		internal override Size FrameBorderSize => throw new NotImplementedException ();

		internal override Size IconSize => throw new NotImplementedException ();

		internal override Size MaxWindowTrackSize => throw new NotImplementedException ();

		internal override bool MenuAccessKeysUnderlined => throw new NotImplementedException ();

		internal override Size MinimizedWindowSpacingSize => throw new NotImplementedException ();

		internal override Size MinimumWindowSize => throw new NotImplementedException ();

		internal override Size SmallIconSize => throw new NotImplementedException ();

		internal override int MouseButtonCount => throw new NotImplementedException ();

		internal override bool MouseButtonsSwapped => throw new NotImplementedException ();

		internal override bool MouseWheelPresent => throw new NotImplementedException ();

		internal override Rectangle VirtualScreen => throw new NotImplementedException ();

		internal override Rectangle WorkingArea => throw new NotImplementedException ();

		internal override Screen [] AllScreens => throw new NotImplementedException ();

		internal override bool ThemesEnabled => throw new NotImplementedException ();

		internal override int KeyboardSpeed => throw new NotImplementedException ();

		internal override int KeyboardDelay => throw new NotImplementedException ();

		internal override event EventHandler Idle;

		internal override void Activate (IntPtr handle)
		{
			throw new NotImplementedException ();
		}

		internal override void AudibleAlert (AlertType alert)
		{
			throw new NotImplementedException ();
		}

		internal override void BeginMoveResize (IntPtr handle)
		{
			throw new NotImplementedException ();
		}

		internal override bool CalculateWindowRect (ref Rectangle ClientRect, CreateParams cp, Menu menu, out Rectangle WindowRect)
		{
			throw new NotImplementedException ();
		}

		internal override void CaretVisible (IntPtr hwnd, bool visible)
		{
			throw new NotImplementedException ();
		}

		internal override void ClientToScreen (IntPtr hwnd, ref int x, ref int y)
		{
			throw new NotImplementedException ();
		}

		internal override int [] ClipboardAvailableFormats (IntPtr handle)
		{
			throw new NotImplementedException ();
		}

		internal override void ClipboardClose (IntPtr handle)
		{
			Console.WriteLine ("MISSING: ClipboardClose");
		}

		internal override int ClipboardGetID (IntPtr handle, string format)
		{
			Console.WriteLine ("MISSING: ClipboardGetID");
			return 0;
		}

		internal override IntPtr ClipboardOpen (bool primary_selection)
		{
			Console.WriteLine ("MISSING: ClipboardOpen");
			return IntPtr.Zero;

		}

		internal override object ClipboardRetrieve (IntPtr handle, int id, XplatUI.ClipboardToObject converter)
		{
			throw new NotImplementedException ();
		}

		internal override void ClipboardStore (IntPtr handle, object obj, int id, XplatUI.ObjectToClipboard converter, bool copy)
		{
			throw new NotImplementedException ();
		}

		internal override void CreateCaret (IntPtr hwnd, int width, int height)
		{
			throw new NotImplementedException ();
		}

		internal override IntPtr CreateWindow (CreateParams cp)
		{
			throw new NotImplementedException ();
		}

		internal override IntPtr CreateWindow (IntPtr Parent, int X, int Y, int Width, int Height)
		{
			throw new NotImplementedException ();
		}

		internal override IntPtr DefineCursor (Bitmap bitmap, Bitmap mask, Color cursor_pixel, Color mask_pixel, int xHotSpot, int yHotSpot)
		{
			throw new NotImplementedException ();
		}

		internal override IntPtr DefineStdCursor (StdCursor id)
		{
			throw new NotImplementedException ();
		}

		internal override Bitmap DefineStdCursorBitmap (StdCursor id)
		{
			throw new NotImplementedException ();
		}

		internal override IntPtr DefWndProc (ref Message msg)
		{
			throw new NotImplementedException ();
		}

		internal override void DestroyCaret (IntPtr hwnd)
		{
			throw new NotImplementedException ();
		}

		internal override void DestroyCursor (IntPtr cursor)
		{
			throw new NotImplementedException ();
		}

		internal override void DestroyWindow (IntPtr handle)
		{
			throw new NotImplementedException ();
		}

		internal override IntPtr DispatchMessage (ref MSG msg)
		{
			throw new NotImplementedException ();
		}

		internal override void DoEvents ()
		{
			throw new NotImplementedException ();
		}

		internal override void DrawReversibleFrame (Rectangle rectangle, Color backColor, FrameStyle style)
		{
			throw new NotImplementedException ();
		}

		internal override void DrawReversibleLine (Point start, Point end, Color backColor)
		{
			throw new NotImplementedException ();
		}

		internal override void DrawReversibleRectangle (IntPtr handle, Rectangle rect, int line_width)
		{
			throw new NotImplementedException ();
		}

		internal override void EnableThemes ()
		{
			throw new NotImplementedException ();
		}

		internal override void EnableWindow (IntPtr handle, bool Enable)
		{
			throw new NotImplementedException ();
		}

		internal override void EndLoop (Thread thread)
		{
			throw new NotImplementedException ();
		}

		internal override void FillReversibleRectangle (Rectangle rectangle, Color backColor)
		{
			throw new NotImplementedException ();
		}

		internal override IntPtr GetActive ()
		{
			throw new NotImplementedException ();
		}

		internal override SizeF GetAutoScaleSize (Font font)
		{
			throw new NotImplementedException ();
		}

		internal override Region GetClipRegion (IntPtr hwnd)
		{
			throw new NotImplementedException ();
		}

		internal override void GetCursorInfo (IntPtr cursor, out int width, out int height, out int hotspot_x, out int hotspot_y)
		{
			throw new NotImplementedException ();
		}

		internal override void GetCursorPos (IntPtr hwnd, out int x, out int y)
		{
			throw new NotImplementedException ();
		}

		internal override void GetDisplaySize (out Size size)
		{
			throw new NotImplementedException ();
		}

		internal override IntPtr GetFocus ()
		{
			throw new NotImplementedException ();
		}

		internal override bool GetFontMetrics (Graphics g, Font font, out int ascent, out int descent)
		{
			throw new NotImplementedException ();
		}

		internal override Point GetMenuOrigin (IntPtr hwnd)
		{
			throw new NotImplementedException ();
		}

		internal override bool GetMessage (object queue_id, ref MSG msg, IntPtr hWnd, int wFilterMin, int wFilterMax)
		{
			throw new NotImplementedException ();
		}

		internal override IntPtr GetParent (IntPtr handle)
		{
			throw new NotImplementedException ();
		}

		internal override IntPtr GetPreviousWindow (IntPtr hwnd)
		{
			throw new NotImplementedException ();
		}

		internal override bool GetText (IntPtr handle, out string text)
		{
			throw new NotImplementedException ();
		}

		internal override void GetWindowPos (IntPtr handle, bool is_toplevel, out int x, out int y, out int width, out int height, out int client_width, out int client_height)
		{
			throw new NotImplementedException ();
		}

		internal override FormWindowState GetWindowState (IntPtr handle)
		{
			throw new NotImplementedException ();
		}

		internal override double GetWindowTransparency (IntPtr handle)
		{
			throw new NotImplementedException ();
		}

		internal override void GrabInfo (out IntPtr hwnd, out bool GrabConfined, out Rectangle GrabArea)
		{
			throw new NotImplementedException ();
		}

		internal override void GrabWindow (IntPtr hwnd, IntPtr ConfineToHwnd)
		{
			throw new NotImplementedException ();
		}

		internal override void HandleException (Exception e)
		{
			throw new NotImplementedException ();
		}

		internal override IntPtr InitializeDriver ()
		{
			return  IntPtr.Zero;
		}

		internal override void Invalidate (IntPtr handle, Rectangle rc, bool clear)
		{
			throw new NotImplementedException ();
		}

		internal override void InvalidateNC (IntPtr handle)
		{
			throw new NotImplementedException ();
		}

		internal override bool IsEnabled (IntPtr handle)
		{
			throw new NotImplementedException ();
		}

		internal override bool IsVisible (IntPtr handle)
		{
			throw new NotImplementedException ();
		}

		internal override void KillTimer (Timer timer)
		{
			throw new NotImplementedException ();
		}

		internal override void MenuToScreen (IntPtr hwnd, ref int x, ref int y)
		{
			throw new NotImplementedException ();
		}

		internal override void OverrideCursor (IntPtr cursor)
		{
			throw new NotImplementedException ();
		}

		internal override void PaintEventEnd (ref Message msg, IntPtr handle, bool client)
		{
			throw new NotImplementedException ();
		}

		internal override PaintEventArgs PaintEventStart (ref Message msg, IntPtr handle, bool client)
		{
			throw new NotImplementedException ();
		}

		internal override bool PeekMessage (object queue_id, ref MSG msg, IntPtr hWnd, int wFilterMin, int wFilterMax, uint flags)
		{
			throw new NotImplementedException ();
		}

		internal override bool PostMessage (IntPtr hwnd, Msg message, IntPtr wParam, IntPtr lParam)
		{
			throw new NotImplementedException ();
		}

		internal override void PostQuitMessage (int exitCode)
		{
			throw new NotImplementedException ();
		}

		internal override void RaiseIdle (EventArgs e)
		{
			throw new NotImplementedException ();
		}

		internal override void RequestAdditionalWM_NCMessages (IntPtr hwnd, bool hover, bool leave)
		{
			throw new NotImplementedException ();
		}

		internal override void RequestNCRecalc (IntPtr hwnd)
		{
			throw new NotImplementedException ();
		}

		internal override void ResetMouseHover (IntPtr hwnd)
		{
			throw new NotImplementedException ();
		}

		internal override void ScreenToClient (IntPtr hwnd, ref int x, ref int y)
		{
			throw new NotImplementedException ();
		}

		internal override void ScreenToMenu (IntPtr hwnd, ref int x, ref int y)
		{
			throw new NotImplementedException ();
		}

		internal override void ScrollWindow (IntPtr hwnd, Rectangle rectangle, int XAmount, int YAmount, bool with_children)
		{
			throw new NotImplementedException ();
		}

		internal override void ScrollWindow (IntPtr hwnd, int XAmount, int YAmount, bool with_children)
		{
			throw new NotImplementedException ();
		}

		internal override void SendAsyncMethod (AsyncMethodData method)
		{
			throw new NotImplementedException ();
		}

		internal override int SendInput (IntPtr hwnd, Queue keys)
		{
			throw new NotImplementedException ();
		}

		internal override IntPtr SendMessage (IntPtr hwnd, Msg message, IntPtr wParam, IntPtr lParam)
		{
			throw new NotImplementedException ();
		}

		internal override void SetBorderStyle (IntPtr handle, FormBorderStyle border_style)
		{
			throw new NotImplementedException ();
		}

		internal override void SetCaretPos (IntPtr hwnd, int x, int y)
		{
			throw new NotImplementedException ();
		}

		internal override void SetClipRegion (IntPtr hwnd, Region region)
		{
			throw new NotImplementedException ();
		}

		internal override void SetCursor (IntPtr hwnd, IntPtr cursor)
		{
			throw new NotImplementedException ();
		}

		internal override void SetCursorPos (IntPtr hwnd, int x, int y)
		{
			throw new NotImplementedException ();
		}

		internal override void SetFocus (IntPtr hwnd)
		{
			throw new NotImplementedException ();
		}

		internal override void SetIcon (IntPtr handle, Icon icon)
		{
			throw new NotImplementedException ();
		}

		internal override void SetMenu (IntPtr handle, Menu menu)
		{
			throw new NotImplementedException ();
		}

		internal override void SetModal (IntPtr handle, bool Modal)
		{
			throw new NotImplementedException ();
		}

		internal override bool SetOwner (IntPtr hWnd, IntPtr hWndOwner)
		{
			throw new NotImplementedException ();
		}

		internal override IntPtr SetParent (IntPtr handle, IntPtr parent)
		{
			throw new NotImplementedException ();
		}

		internal override void SetTimer (Timer timer)
		{
			throw new NotImplementedException ();
		}

		internal override bool SetTopmost (IntPtr hWnd, bool Enabled)
		{
			throw new NotImplementedException ();
		}

		internal override bool SetVisible (IntPtr handle, bool visible, bool activate)
		{
			throw new NotImplementedException ();
		}

		internal override void SetWindowMinMax (IntPtr handle, Rectangle maximized, Size min, Size max)
		{
			throw new NotImplementedException ();
		}

		internal override void SetWindowPos (IntPtr handle, int x, int y, int width, int height)
		{
			throw new NotImplementedException ();
		}

		internal override void SetWindowState (IntPtr handle, FormWindowState state)
		{
			throw new NotImplementedException ();
		}

		internal override void SetWindowStyle (IntPtr handle, CreateParams cp)
		{
			throw new NotImplementedException ();
		}

		internal override void SetWindowTransparency (IntPtr handle, double transparency, Color key)
		{
			throw new NotImplementedException ();
		}

		internal override bool SetZOrder (IntPtr hWnd, IntPtr AfterhWnd, bool Top, bool Bottom)
		{
			throw new NotImplementedException ();
		}

		internal override void ShowCursor (bool show)
		{
			throw new NotImplementedException ();
		}

		internal override void ShutdownDriver (IntPtr token)
		{
			throw new NotImplementedException ();
		}

		internal override object StartLoop (Thread thread)
		{
			throw new NotImplementedException ();
		}

		internal override TransparencySupport SupportsTransparency ()
		{
			throw new NotImplementedException ();
		}

		internal override bool SystrayAdd (IntPtr hwnd, string tip, Icon icon, out ToolTip tt)
		{
			throw new NotImplementedException ();
		}

		internal override void SystrayBalloon (IntPtr hwnd, int timeout, string title, string text, ToolTipIcon icon)
		{
			throw new NotImplementedException ();
		}

		internal override bool SystrayChange (IntPtr hwnd, string tip, Icon icon, ref ToolTip tt)
		{
			throw new NotImplementedException ();
		}

		internal override void SystrayRemove (IntPtr hwnd, ref ToolTip tt)
		{
			throw new NotImplementedException ();
		}

		internal override bool Text (IntPtr handle, string text)
		{
			throw new NotImplementedException ();
		}

		internal override bool TranslateMessage (ref MSG msg)
		{
			throw new NotImplementedException ();
		}

		internal override void UngrabWindow (IntPtr hwnd)
		{
			throw new NotImplementedException ();
		}

		internal override void UpdateWindow (IntPtr handle)
		{
			throw new NotImplementedException ();
		}
	}
}
