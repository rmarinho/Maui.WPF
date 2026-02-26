using System;
using Microsoft.Win32;
using Microsoft.Maui.ApplicationModel;

namespace Microsoft.Maui.Platform.WPF
{
	/// <summary>
	/// Detects Windows dark/light theme and fires RequestedThemeChanged.
	/// </summary>
	public static class ThemeManager
	{
		static AppTheme _currentTheme = AppTheme.Unspecified;

		public static AppTheme GetCurrentTheme()
		{
			try
			{
				using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize");
				if (key != null)
				{
					var value = key.GetValue("AppsUseLightTheme");
					if (value is int i)
						return i == 0 ? AppTheme.Dark : AppTheme.Light;
				}
			}
			catch { }
			return AppTheme.Light;
		}

		public static void Initialize()
		{
			_currentTheme = GetCurrentTheme();

			// Listen for system theme changes
			SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
		}

		static void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
		{
			if (e.Category == UserPreferenceCategory.General)
			{
				var newTheme = GetCurrentTheme();
				if (newTheme != _currentTheme)
				{
					_currentTheme = newTheme;
					// Fire the MAUI theme changed event via the Application
					var app = Microsoft.Maui.Controls.Application.Current;
					if (app != null)
					{
						try
						{
							// Use reflection to set the platform app theme
							var prop = typeof(Microsoft.Maui.Controls.Application).GetProperty("PlatformAppTheme",
								System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
							// PlatformAppTheme is read-only, but we can trigger the change event
							var method = typeof(Microsoft.Maui.Controls.Application).GetMethod("TriggerThemeChanged",
								System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
							method?.Invoke(app, null);
						}
						catch { }
					}
				}
			}
		}

		public static void Shutdown()
		{
			SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
		}
	}
}
