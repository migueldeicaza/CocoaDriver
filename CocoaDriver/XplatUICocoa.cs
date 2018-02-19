//
// XplatUICocoa - Cocoa driver for Mono's Windows.Forms
//
// This is a Cocoa/Xamarin.Mac-ification of the Carbon Driver by Geoff Norton
//
// Authors:
//    Miguel de Icaza (Cocoa version)
//    Geoff Norton (original Carbon driver)
//
using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using System.Drawing;
using AppKit;
using CoreGraphics;
using Foundation;
using System.Runtime.InteropServices;

namespace System.Windows.Forms {
	internal class XplatUICocoa : XplatUIDriver {
		// TODO;
		int MenuBarHeight;
		Dictionary<IntPtr, NSWindow> WindowMapping = new Dictionary<IntPtr, NSWindow> ();
		Dictionary<NSWindow, IntPtr> HandleMapping = new Dictionary<NSWindow, IntPtr> ();
		List<NSWindow> UtilityWindows = new List<NSWindow> ();
		List<Timer> TimerList = new List<Timer> ();
		static bool in_doevents;
		static readonly object queuelock = new object ();
		Queue MessageQueue = new Queue ();

		public XplatUICocoa ()
		{
			MenuBarHeight = 20;

		}

		internal override int CaptionHeight => throw new NotImplementedException ();

		internal override Size CursorSize => throw new NotImplementedException ();

		internal override bool DragFullWindows => throw new NotImplementedException ();

		internal override Size DragSize => throw new NotImplementedException ();

		internal override Size FrameBorderSize => throw new NotImplementedException ();

		internal override Size IconSize => throw new NotImplementedException ();

		internal override Size MaxWindowTrackSize => throw new NotImplementedException ();

		internal override bool MenuAccessKeysUnderlined => false;

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
			WindowRect = Hwnd.GetWindowRectangle (cp, menu, ClientRect);
			return true;
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

		private void WaitForHwndMessage (Hwnd hwnd, Msg message)
		{
			MSG msg = new MSG ();

			bool done = false;
			do {
				if (GetMessage (null, ref msg, IntPtr.Zero, 0, 0)) {
					if ((Msg)msg.message == Msg.WM_QUIT) {
						PostQuitMessage (0);
						done = true;
					} else {
						if (msg.hwnd == hwnd.Handle) {
							if ((Msg)msg.message == message)
								break;
							else if ((Msg)msg.message == Msg.WM_DESTROY)
								done = true;
						}

						TranslateMessage (ref msg);
						DispatchMessage (ref msg);
					}
				}
			} while (!done);
		}

		private void SendParentNotify (IntPtr child, Msg cause, int x, int y)
		{
			Hwnd hwnd;

			if (child == IntPtr.Zero) {
				return;
			}

			hwnd = Hwnd.GetObjectFromWindow (child);

			if (hwnd == null)
				return;
			if (hwnd.Handle == IntPtr.Zero)
				return;
			if (ExStyleSet ((int)hwnd.initial_ex_style, WindowExStyles.WS_EX_NOPARENTNOTIFY))
				return;
			if (hwnd.Parent == null)
				return;

			if (hwnd.Parent.Handle == IntPtr.Zero)
				return;

			if (cause == Msg.WM_CREATE || cause == Msg.WM_DESTROY)
				SendMessage (hwnd.Parent.Handle, Msg.WM_PARENTNOTIFY, Control.MakeParam ((int)cause, 0), child);
			else
				SendMessage (hwnd.Parent.Handle, Msg.WM_PARENTNOTIFY, Control.MakeParam ((int)cause, 0), Control.MakeParam (x, y));

			SendParentNotify (hwnd.Parent.Handle, cause, x, y);
		}

		private bool StyleSet (int s, WindowStyles ws)
		{
			return (s & (int)ws) == (int)ws;
		}

		private bool ExStyleSet (int ex, WindowExStyles exws)
		{
			return (ex & (int)exws) == (int)exws;
		}

		private void DeriveStyles (int Style, int ExStyle, out FormBorderStyle border_style, out bool border_static, out TitleStyle title_style, out int caption_height, out int tool_caption_height)
		{

			caption_height = 0;
			tool_caption_height = 0;
			border_static = false;

			if (StyleSet (Style, WindowStyles.WS_CHILD)) {
				if (ExStyleSet (ExStyle, WindowExStyles.WS_EX_CLIENTEDGE)) {
					border_style = FormBorderStyle.Fixed3D;
				} else if (ExStyleSet (ExStyle, WindowExStyles.WS_EX_STATICEDGE)) {
					border_style = FormBorderStyle.Fixed3D;
					border_static = true;
				} else if (!StyleSet (Style, WindowStyles.WS_BORDER)) {
					border_style = FormBorderStyle.None;
				} else {
					border_style = FormBorderStyle.FixedSingle;
				}
				title_style = TitleStyle.None;

				if (StyleSet (Style, WindowStyles.WS_CAPTION)) {
					caption_height = 0;
					if (ExStyleSet (ExStyle, WindowExStyles.WS_EX_TOOLWINDOW)) {
						title_style = TitleStyle.Tool;
					} else {
						title_style = TitleStyle.Normal;
					}
				}

				if (ExStyleSet (ExStyle, WindowExStyles.WS_EX_MDICHILD)) {
					caption_height = 0;

					if (StyleSet (Style, WindowStyles.WS_OVERLAPPEDWINDOW) ||
						ExStyleSet (ExStyle, WindowExStyles.WS_EX_TOOLWINDOW)) {
						border_style = (FormBorderStyle)0xFFFF;
					} else {
						border_style = FormBorderStyle.None;
					}
				}

			} else {
				title_style = TitleStyle.None;
				if (StyleSet (Style, WindowStyles.WS_CAPTION)) {
					if (ExStyleSet (ExStyle, WindowExStyles.WS_EX_TOOLWINDOW)) {
						title_style = TitleStyle.Tool;
					} else {
						title_style = TitleStyle.Normal;
					}
				}

				border_style = FormBorderStyle.None;

				if (StyleSet (Style, WindowStyles.WS_THICKFRAME)) {
					if (ExStyleSet (ExStyle, WindowExStyles.WS_EX_TOOLWINDOW)) {
						border_style = FormBorderStyle.SizableToolWindow;
					} else {
						border_style = FormBorderStyle.Sizable;
					}
				} else {
					if (StyleSet (Style, WindowStyles.WS_CAPTION)) {
						if (ExStyleSet (ExStyle, WindowExStyles.WS_EX_CLIENTEDGE)) {
							border_style = FormBorderStyle.Fixed3D;
						} else if (ExStyleSet (ExStyle, WindowExStyles.WS_EX_STATICEDGE)) {
							border_style = FormBorderStyle.Fixed3D;
							border_static = true;
						} else if (ExStyleSet (ExStyle, WindowExStyles.WS_EX_DLGMODALFRAME)) {
							border_style = FormBorderStyle.FixedDialog;
						} else if (ExStyleSet (ExStyle, WindowExStyles.WS_EX_TOOLWINDOW)) {
							border_style = FormBorderStyle.FixedToolWindow;
						} else if (StyleSet (Style, WindowStyles.WS_BORDER)) {
							border_style = FormBorderStyle.FixedSingle;
						}
					} else {
						if (StyleSet (Style, WindowStyles.WS_BORDER)) {
							border_style = FormBorderStyle.FixedSingle;
						}
					}
				}
			}
		}

		private void SetHwndStyles (Hwnd hwnd, CreateParams cp)
		{
			DeriveStyles (cp.Style, cp.ExStyle, out hwnd.border_style, out hwnd.border_static, out hwnd.title_style, out hwnd.caption_height, out hwnd.tool_caption_height);
		}


		internal static CGRect TranslateClientRectangleToQuartzClientRectangle (Hwnd hwnd)
		{
			return TranslateClientRectangleToQuartzClientRectangle (hwnd, Control.FromHandle (hwnd.Handle));
		}

		internal static CGRect TranslateClientRectangleToQuartzClientRectangle (Hwnd hwnd, Control ctrl)
		{
			/* From XplatUIX11
			 * If this is a form with no window manager, X is handling all the border and caption painting
			 * so remove that from the area (since the area we set of the window here is the part of the window 
			 * we're painting in only)
			 */
			var crect = hwnd.ClientRect;
			CGRect rect = new CGRect (crect.X, crect.Y, crect.Width, crect.Height);
			Form form = ctrl as Form;
			CreateParams cp = null;

			if (form != null)
				cp = form.GetCreateParams ();

			if (form != null && (form.window_manager == null || cp.IsSet (WindowExStyles.WS_EX_TOOLWINDOW))) {
				Hwnd.Borders borders = Hwnd.GetBorders (cp, null);
				CGRect qrect = rect;

				qrect.Y -= borders.top;
				qrect.X -= borders.left;
				qrect.Width += borders.left + borders.right;
				qrect.Height += borders.top + borders.bottom;

				rect = qrect;
			}

			if (rect.Width < 1 || rect.Height < 1) {
				rect.Width = 1;
				rect.Height = 1;
				rect.X = -5;
				rect.Y = -5;
			}

			return rect;
		}


		internal static CGSize TranslateWindowSizeToQuartzWindowSize (CreateParams cp)
		{
			return TranslateWindowSizeToCocoaWindowSize (cp, new CGSize (cp.Width, cp.Height));
		}

		internal static CGSize TranslateWindowSizeToCocoaWindowSize (CreateParams cp, CGSize size)
		{
			/* From XplatUIX11
			 * If this is a form with no window manager, X is handling all the border and caption painting
			 * so remove that from the area (since the area we set of the window here is the part of the window 
			 * we're painting in only)
			 */
			Form form = cp.control as Form;
			if (form != null && (form.window_manager == null || cp.IsSet (WindowExStyles.WS_EX_TOOLWINDOW))) {
				Hwnd.Borders borders = Hwnd.GetBorders (cp, null);
				CGSize qsize = size;

				qsize.Width -= borders.left + borders.right;
				qsize.Height -= borders.top + borders.bottom;

				size = qsize;
			}

			if (size.Height == 0)
				size.Height = 1;
			if (size.Width == 0)
				size.Width = 1;
			return size;
		}

		internal override IntPtr CreateWindow (CreateParams cp)
		{
			Hwnd hwnd;
			Hwnd parent_hwnd = null;
			int X;
			int Y;
			int Width;
			int Height;
			IntPtr ParentHandle;
			NSWindow window;
			NSView WholeWindow;
			NSView ClientWindow;
			IntPtr WholeWindowTracking;
			IntPtr ClientWindowTracking;

			hwnd = new Hwnd ();

			X = cp.X;
			Y = cp.Y;
			Width = cp.Width;
			Height = cp.Height;
			ParentHandle = IntPtr.Zero;
			window = null;
			WholeWindow = null;
			ClientWindow = null;
			WholeWindowTracking = IntPtr.Zero;
			ClientWindowTracking = IntPtr.Zero;

			if (Width < 1) Width = 1;
			if (Height < 1) Height = 1;

			if (cp.Parent != IntPtr.Zero) {
				parent_hwnd = Hwnd.ObjectFromHandle (cp.Parent);
				ParentHandle = parent_hwnd.client_window;
			} else {
				if (StyleSet (cp.Style, WindowStyles.WS_CHILD)) {
					// HIViewFindByID (HIViewGetRoot (FosterParent), new Carbon.HIViewID (Carbon.EventHandler.kEventClassWindow, 1), ref ParentHandle);
					Console.WriteLine ("CreateWindow WS_CHILD not implemented");
				}
			}

			Point next;
			if (cp.control is Form) {
				next = Hwnd.GetNextStackedFormLocation (cp, parent_hwnd);
				X = next.X;
				Y = next.Y;
			}

			hwnd.x = X;
			hwnd.y = Y;
			hwnd.width = Width;
			hwnd.height = Height;
			hwnd.Parent = Hwnd.ObjectFromHandle (cp.Parent);
			hwnd.initial_style = cp.WindowStyle;
			hwnd.initial_ex_style = cp.WindowExStyle;
			hwnd.visible = false;

			if (StyleSet (cp.Style, WindowStyles.WS_DISABLED)) {
				hwnd.enabled = false;
			}

			ClientWindow = null;

			CGSize QWindowSize = TranslateWindowSizeToQuartzWindowSize (cp);
			CGRect QClientRect = TranslateClientRectangleToQuartzClientRectangle (hwnd, cp.control);

			SetHwndStyles (hwnd, cp);
			/* FIXME */
			if (ParentHandle == IntPtr.Zero) {
				NSWindowStyle style = 0;
				IntPtr WindowView = IntPtr.Zero;
				IntPtr GrowBox = IntPtr.Zero;

				// Carbon.WindowAttributes attributes = Carbon.WindowAttributes.kWindowCompositingAttribute | Carbon.WindowAttributes.kWindowStandardHandlerAttribute;
				if (StyleSet (cp.Style, WindowStyles.WS_MINIMIZEBOX))
					style |= NSWindowStyle.Miniaturizable;

				if (StyleSet (cp.Style, WindowStyles.WS_MAXIMIZEBOX))
					style |= NSWindowStyle.Resizable;

				if (StyleSet (cp.Style, WindowStyles.WS_SYSMENU))
					style |= NSWindowStyle.Closable;

				if (StyleSet (cp.Style, WindowStyles.WS_CAPTION))
					style |= NSWindowStyle.Titled;


				if (hwnd.border_style == FormBorderStyle.FixedToolWindow)
					style |= NSWindowStyle.Utility;
				else if (hwnd.border_style == FormBorderStyle.SizableToolWindow)
					style |= NSWindowStyle.Resizable;

				CGRect rect = StyleSet (cp.Style, WindowStyles.WS_POPUP)
					? new CGRect ((short)X, (short)(Y), (short)(X + QWindowSize.Width), (short)(Y + QWindowSize.Height))
					: new CGRect ((short)X, (short)(Y + MenuBarHeight), (short)(X + QWindowSize.Width), (short)(Y + MenuBarHeight + QWindowSize.Height));

				window = new NSWindow (rect, style, NSBackingStore.Buffered, false);

#if false
				Carbon.EventHandler.InstallWindowHandler (window);
				HIViewFindByID (HIViewGetRoot (window), new Carbon.HIViewID (Carbon.EventHandler.kEventClassWindow, 1), ref WindowView);
				HIViewFindByID (HIViewGetRoot (window), new Carbon.HIViewID (Carbon.EventHandler.kEventClassWindow, 7), ref GrowBox);
				HIGrowBoxViewSetTransparent (GrowBox, true);
				SetAutomaticControlDragTrackingEnabledForWindow (window, true);
				ParentHandle = WindowView;
#endif
			}

#if false
			HIObjectCreate (__CFStringMakeConstantString ("com.novell.mwfview"), 0, ref WholeWindow);
			HIObjectCreate (__CFStringMakeConstantString ("com.novell.mwfview"), 0, ref ClientWindow);

			Carbon.EventHandler.InstallControlHandler (WholeWindow);
			Carbon.EventHandler.InstallControlHandler (ClientWindow);

			// Enable embedding on controls
			HIViewChangeFeatures (WholeWindow, 1 << 1, 0);
			HIViewChangeFeatures (ClientWindow, 1 << 1, 0);

			HIViewNewTrackingArea (WholeWindow, IntPtr.Zero, (UInt64)WholeWindow, ref WholeWindowTracking);
			HIViewNewTrackingArea (ClientWindow, IntPtr.Zero, (UInt64)ClientWindow, ref ClientWindowTracking);
			Carbon.HIRect WholeRect;
			if (window != IntPtr.Zero) {
				WholeRect = new Carbon.HIRect (0, 0, QWindowSize.Width, QWindowSize.Height);
			} else {
				WholeRect = new Carbon.HIRect (X, Y, QWindowSize.Width, QWindowSize.Height);
			}
			Carbon.HIRect ClientRect = new Carbon.HIRect (QClientRect.X, QClientRect.Y, QClientRect.Width, QClientRect.Height);
			HIViewSetFrame (WholeWindow, ref WholeRect);
			HIViewSetFrame (ClientWindow, ref ClientRect);

			HIViewAddSubview (ParentHandle, WholeWindow);
			HIViewAddSubview (WholeWindow, ClientWindow);

			hwnd.WholeWindow = WholeWindow;
			hwnd.ClientWindow = ClientWindow;
#endif
			CGRect WholeRect;
			if (window != null)
				WholeRect = new CGRect (0, 0, QWindowSize.Width, QWindowSize.Height);
			else
				WholeRect = new CGRect (X, Y, QWindowSize.Width, QWindowSize.Height);

			CGRect ClientRect = new CGRect (QClientRect.X, QClientRect.Y, QClientRect.Width, QClientRect.Height);
			WholeWindow = new NSView (WholeRect);
			ClientWindow = new NSView (ClientRect);

			window.ContentView.AddSubview (WholeWindow);
			window.ContentView.AddSubview (ClientWindow);

			hwnd.WholeWindow = WholeWindow.Handle;
			hwnd.ClientWindow = ClientWindow.Handle;

			if (window != null) {
				WindowMapping [hwnd.Handle] = window;
				HandleMapping [window] = hwnd.Handle;
				if (hwnd.border_style == FormBorderStyle.FixedToolWindow || hwnd.border_style == FormBorderStyle.SizableToolWindow) {
					UtilityWindows.Add (window);
				}
			}

#if false
			// Allow dnd on controls
			Dnd.SetAllowDrop (hwnd, true);
#endif
			window.Title = cp.Caption;

			SendMessage (hwnd.Handle, Msg.WM_CREATE, (IntPtr)1, IntPtr.Zero /* XXX unused */);
			SendParentNotify (hwnd.Handle, Msg.WM_CREATE, int.MaxValue, int.MaxValue);

			if (StyleSet (cp.Style, WindowStyles.WS_VISIBLE)) {
				if (window != null) {
					if (Control.FromHandle (hwnd.Handle) is Form) {
						Form f = Control.FromHandle (hwnd.Handle) as Form;
						if (f.WindowState == FormWindowState.Normal) {
							SendMessage (hwnd.Handle, Msg.WM_SHOWWINDOW, (IntPtr)1, IntPtr.Zero);
						}
					}
					window.OrderFront (window);
					WaitForHwndMessage (hwnd, Msg.WM_SHOWWINDOW);
				}
#if false
				HIViewSetVisible (WholeWindow, true);
				HIViewSetVisible (ClientWindow, true);
				hwnd.visible = true;
#endif
				if (!(Control.FromHandle (hwnd.Handle) is Form)) {
					SendMessage (hwnd.Handle, Msg.WM_SHOWWINDOW, (IntPtr)1, IntPtr.Zero);
				}
			}

			if (StyleSet (cp.Style, WindowStyles.WS_MINIMIZE)) {
				window.Miniaturize (window);
			} else if (StyleSet (cp.Style, WindowStyles.WS_MAXIMIZE)) {
				// TODO
			}

			return hwnd.Handle;
		}

		internal override IntPtr CreateWindow (IntPtr Parent, int X, int Y, int Width, int Height)
		{
			CreateParams create_params = new CreateParams ();

			create_params.Caption = "";
			create_params.X = X;
			create_params.Y = Y;
			create_params.Width = Width;
			create_params.Height = Height;

			create_params.ClassName = XplatUI.GetDefaultClassName (GetType ());
			create_params.ClassStyle = 0;
			create_params.ExStyle = 0;
			create_params.Parent = IntPtr.Zero;
			create_params.Param = 0;

			return CreateWindow (create_params);
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

		private void CheckTimers (DateTime now)
		{
			lock (TimerList) {
				int count = TimerList.Count;
				if (count == 0)
					return;
				for (int i = 0; i < TimerList.Count; i++) {
					Timer timer = (Timer)TimerList [i];
					if (timer.Enabled && timer.Expires <= now) {
						// Timer ticks:
						//  - Before MainForm.OnLoad if DoEvents () is called.
						//  - After MainForm.OnLoad if not.
						//
						if (in_doevents ||
						    (Application.MWFThread.Current.Context != null &&
						     Application.MWFThread.Current.Context.MainForm != null &&
						     Application.MWFThread.Current.Context.MainForm.IsLoaded)) {
							timer.FireTick ();
							timer.Update (now);
						}
					}
				}
			}
		}

		internal override bool GetMessage (object queue_id, ref MSG msg, IntPtr hWnd, int wFilterMin, int wFilterMax)
		{
			//IntPtr target = GetEventDispatcherTarget ();
			CheckTimers (DateTime.UtcNow);
			var evt = NSApplication.SharedApplication.NextEvent (NSEventMask.AnyEvent, NSDate.DistantPast, "NSDefaultRunLoopMode", deqFlag: true);

			if (evt != null)
				NSApplication.SharedApplication.SendEvent (evt);

#if false
			object queueobj;
		loop:
			lock (queuelock) {
				if (MessageQueue.Count <= 0) {
					if (Idle != null)
						Idle (this, EventArgs.Empty);
					else if (TimerList.Count == 0) {
						ReceiveNextEvent (0, IntPtr.Zero, 0.15, true, ref evtRef);
						if (evtRef != IntPtr.Zero && target != IntPtr.Zero) {
							SendEventToEventTarget (evtRef, target);
							ReleaseEvent (evtRef);
						}
					} else {
						ReceiveNextEvent (0, IntPtr.Zero, NextTimeout (), true, ref evtRef);
						if (evtRef != IntPtr.Zero && target != IntPtr.Zero) {
							SendEventToEventTarget (evtRef, target);
							ReleaseEvent (evtRef);
						}
					}
					msg.hwnd = IntPtr.Zero;
					msg.message = Msg.WM_ENTERIDLE;
					return GetMessageResult;
				}
				queueobj = MessageQueue.Dequeue ();
			}
			if (queueobj is GCHandle) {
				XplatUIDriverSupport.ExecuteClientMessage ((GCHandle)queueobj);
				goto loop;
			} else {
				msg = (MSG)queueobj;
			}
			return GetMessageResult;
#endif
		
				throw new Exception ("Not Implemented");
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
			return NativeWindow.WndProc (hwnd, message, wParam, lParam);
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
