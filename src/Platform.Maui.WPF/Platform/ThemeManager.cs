using System;
using Microsoft.Win32;
using Microsoft.Maui.ApplicationModel;

namespace Microsoft.Maui.Platform.WPF
{
	/// <summary>
	/// Detects Windows dark/light theme, sets PlatformAppTheme on the MAUI Application,
	/// and fires RequestedThemeChanged so AppThemeBinding values resolve correctly.
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

		/// <summary>
		/// Must be called after the MAUI Application is created to set the initial PlatformAppTheme.
		/// This enables AppThemeBinding to resolve Light/Dark values correctly.
		/// </summary>
		public static void ApplyThemeToApplication()
		{
			var app = Microsoft.Maui.Controls.Application.Current;
			if (app == null) return;

			_currentTheme = GetCurrentTheme();
			SetPlatformAppTheme(app, _currentTheme);
		}

		static void SetPlatformAppTheme(Microsoft.Maui.Controls.Application app, AppTheme theme)
		{
			try
			{
				// PlatformAppTheme has a public setter at runtime but is hidden at compile time
				var prop = typeof(Microsoft.Maui.Controls.Application).GetProperty("PlatformAppTheme",
					System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
				prop?.SetValue(app, theme);
			}
			catch { }
		}

		static void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
		{
			if (e.Category == UserPreferenceCategory.General)
			{
				var newTheme = GetCurrentTheme();
				if (newTheme != _currentTheme)
				{
					_currentTheme = newTheme;
					var app = Microsoft.Maui.Controls.Application.Current;
					if (app != null)
					{
						SetPlatformAppTheme(app, newTheme);
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
