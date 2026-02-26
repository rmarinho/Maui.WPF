using System.Reflection;
using Microsoft.Maui.ApplicationModel;

namespace Microsoft.Maui.Essentials.WPF
{
	public class WPFAppInfo : IAppInfo
	{
		public string PackageName => AppDomain.CurrentDomain.FriendlyName;
		public string Name => Assembly.GetEntryAssembly()?.GetName().Name ?? "WPF App";
		public string VersionString => Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "1.0.0";
		public Version Version => Assembly.GetEntryAssembly()?.GetName().Version ?? new Version(1, 0, 0);
		public string BuildString => VersionString;
		public LayoutDirection RequestedLayoutDirection => LayoutDirection.LeftToRight;
		public AppTheme RequestedTheme => AppTheme.Light;
		public AppPackagingModel PackagingModel => AppPackagingModel.Unpackaged;

		public void ShowSettingsUI() { }
	}
}
