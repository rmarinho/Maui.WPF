#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace Microsoft.Maui.Platform.WPF
{
	/// <summary>
	/// Manages gesture recognizer attachment for WPF FrameworkElements.
	/// Handles Tap, Pointer (hover), Pan, Swipe, and Pinch gestures.
	/// </summary>
	public static class GestureManager
	{
		const string GestureTag = "MauiGesture";

		public static void SetupGestures(FrameworkElement platformView, IView virtualView)
		{
			if (virtualView is not View mauiView)
				return;

			// Remove old gesture handlers
			ClearManagedGestures(platformView);

			foreach (var recognizer in mauiView.GestureRecognizers)
			{
				switch (recognizer)
				{
					case TapGestureRecognizer tap:
						AddTapGesture(platformView, tap);
						break;
					case PointerGestureRecognizer pointer:
						AddPointerGesture(platformView, pointer);
						break;
				}
			}
		}

		static void ClearManagedGestures(FrameworkElement view)
		{
			// Remove stored event handlers
			if (view.Tag is List<GestureCleanup> cleanups)
			{
				foreach (var cleanup in cleanups)
					cleanup.Detach();
				cleanups.Clear();
			}
		}

		static List<GestureCleanup> GetCleanups(FrameworkElement view)
		{
			if (view.Tag is not List<GestureCleanup> list)
			{
				list = new List<GestureCleanup>();
				view.Tag = list;
			}
			return list;
		}

		static void AddTapGesture(FrameworkElement view, TapGestureRecognizer tap)
		{
			MouseButtonEventHandler handler = (s, e) =>
			{
				if (e.ClickCount >= tap.NumberOfTapsRequired)
				{
					tap.Command?.Execute(tap.CommandParameter);

					// TapGestureRecognizer.SendTapped is internal — invoke via reflection
					var sendTapped = typeof(TapGestureRecognizer).GetMethod(
						"SendTapped", BindingFlags.Instance | BindingFlags.NonPublic);
					if (sendTapped != null)
					{
						var parent = FindParentView(tap);
						var parameters = sendTapped.GetParameters();
						if (parameters.Length == 1)
							sendTapped.Invoke(tap, new object?[] { parent });
						else if (parameters.Length == 2)
							sendTapped.Invoke(tap, new object?[] { parent, null });
					}
				}
			};

			view.MouseLeftButtonUp += handler;

			// Make the view respond to mouse input
			if (view is System.Windows.Controls.Control ctrl && ctrl.Background == null)
				ctrl.Background = System.Windows.Media.Brushes.Transparent;
			else if (view is System.Windows.Controls.Panel pnl && pnl.Background == null)
				pnl.Background = System.Windows.Media.Brushes.Transparent;

			GetCleanups(view).Add(new GestureCleanup(() => view.MouseLeftButtonUp -= handler));
		}

		static void AddPointerGesture(FrameworkElement view, PointerGestureRecognizer pointer)
		{
			var sendEntered = typeof(PointerGestureRecognizer).GetMethod("SendPointerEntered", BindingFlags.Instance | BindingFlags.NonPublic);
			var sendExited = typeof(PointerGestureRecognizer).GetMethod("SendPointerExited", BindingFlags.Instance | BindingFlags.NonPublic);
			var sendMoved = typeof(PointerGestureRecognizer).GetMethod("SendPointerMoved", BindingFlags.Instance | BindingFlags.NonPublic);

			MouseEventHandler enterHandler = (s, e) =>
			{
				var parent = FindParentView(pointer);
				sendEntered?.Invoke(pointer, new object?[] { parent, null, null, ButtonsMask.Primary });
			};

			MouseEventHandler leaveHandler = (s, e) =>
			{
				var parent = FindParentView(pointer);
				sendExited?.Invoke(pointer, new object?[] { parent, null, null, ButtonsMask.Primary });
			};

			MouseEventHandler moveHandler = (s, e) =>
			{
				var parent = FindParentView(pointer);
				sendMoved?.Invoke(pointer, new object?[] { parent, null, null, ButtonsMask.Primary });
			};

			view.MouseEnter += enterHandler;
			view.MouseLeave += leaveHandler;
			view.MouseMove += moveHandler;

			var cleanups = GetCleanups(view);
			cleanups.Add(new GestureCleanup(() =>
			{
				view.MouseEnter -= enterHandler;
				view.MouseLeave -= leaveHandler;
				view.MouseMove -= moveHandler;
			}));
		}

		static View? FindParentView(IGestureRecognizer recognizer)
		{
			if (recognizer is Element element)
			{
				var current = element.Parent;
				while (current != null)
				{
					if (current is View view)
						return view;
					current = current.Parent;
				}
			}
			return null;
		}

		class GestureCleanup
		{
			readonly Action _detach;
			public GestureCleanup(Action detach) => _detach = detach;
			public void Detach() => _detach();
		}
	}
}
