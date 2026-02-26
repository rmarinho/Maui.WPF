using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Platform;
using PlatformView = System.Windows.Application;

namespace Microsoft.Maui.Handlers.WPF
{
	public partial class ApplicationHandler : ElementHandler<IApplication, System.Windows.Application>
	{

		internal const string TerminateCommandKey = "Terminate";

		public static IPropertyMapper<IApplication, ApplicationHandler> Mapper = new PropertyMapper<IApplication, ApplicationHandler>(ElementMapper)
		{
		};

		public static CommandMapper<IApplication, ApplicationHandler> CommandMapper = new(ElementCommandMapper)
		{
			[TerminateCommandKey] = MapTerminate,
			[nameof(IApplication.OpenWindow)] = MapOpenWindow,
			[nameof(IApplication.CloseWindow)] = MapCloseWindow,
		};

		ILogger<ApplicationHandler>? _logger;

		public ApplicationHandler()
			: base(Mapper, CommandMapper)
		{
		}

		public ApplicationHandler(IPropertyMapper? mapper)
			: base(mapper ?? Mapper, CommandMapper)
		{
		}

		public ApplicationHandler(IPropertyMapper? mapper, CommandMapper? commandMapper)
			: base(mapper ?? Mapper, commandMapper ?? CommandMapper)
		{
		}

		ILogger? Logger =>
			_logger ??= MauiContext?.Services.GetService<ILogger<ApplicationHandler>>();

		protected override PlatformView CreatePlatformElement()
		{
			var app = MauiContext?.Services.GetService<PlatformView>() ?? throw new InvalidOperationException($"MauiContext did not have a valid application.");

			// Wire lifecycle events
			Platform.WPF.LifecycleManager.RegisterLifecycleEvents(app, VirtualView);

			return app;
		}

		public static void MapTerminate(ApplicationHandler handler, IApplication application, object? args)
		{
			handler.PlatformView?.Shutdown();
		}

		public static void MapOpenWindow(ApplicationHandler handler, IApplication application, object? args)
		{
			//handler.PlatformView?.CreatePlatformWindow(application, args as OpenWindowRequest);
		}

		public static void MapCloseWindow(ApplicationHandler handler, IApplication application, object? args)
		{
			if (args is IWindow window)
			{
				(window.Handler?.PlatformView as System.Windows.Window)?.Close();
			}
		}
	}
}