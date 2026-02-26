using System.IO;
using Microsoft.Maui.Storage;

namespace Microsoft.Maui.Essentials.WPF
{
	public class WPFFileSystem : IFileSystem
	{
		public string AppDataDirectory =>
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				AppDomain.CurrentDomain.FriendlyName);

		public string CacheDirectory =>
			Path.Combine(Path.GetTempPath(), AppDomain.CurrentDomain.FriendlyName);

		public Task<System.IO.Stream> OpenAppPackageFileAsync(string filename)
		{
			var basePath = AppContext.BaseDirectory;
			var filePath = Path.Combine(basePath, filename);

			if (!File.Exists(filePath))
				throw new FileNotFoundException($"App package file not found: {filename}", filePath);

			return Task.FromResult<System.IO.Stream>(File.OpenRead(filePath));
		}

		public Task<bool> AppPackageFileExistsAsync(string filename)
		{
			var filePath = Path.Combine(AppContext.BaseDirectory, filename);
			return Task.FromResult(File.Exists(filePath));
		}
	}
}
