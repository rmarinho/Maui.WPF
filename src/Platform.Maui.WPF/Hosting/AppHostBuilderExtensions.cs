using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui.Controls.Handlers;
using Microsoft.Maui.Controls.Handlers.Items;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Handlers.WPF;
using Microsoft.Maui.Platform.WPF;
using Microsoft.Extensions.Logging;

namespace Microsoft.Maui.Controls.Hosting.WPF
{
	public static partial class AppHostBuilderExtensions
	{
		public static MauiAppBuilder UseMauiAppWPF<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TApp>(this MauiAppBuilder builder)
			where TApp : class, IApplication
		{
			builder.UseMauiApp<TApp>();
			builder.SetupDefaults();
			return builder;
		}

		public static MauiAppBuilder UseMauiAppWPF<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TApp>(this MauiAppBuilder builder, Func<IServiceProvider, TApp> implementationFactory)
			where TApp : class, IApplication
		{
			builder.UseMauiApp<TApp>();
			builder.SetupDefaults();
			return builder;
		}

		static IMauiHandlersCollection AddMauiControlsHandlers(this IMauiHandlersCollection handlersCollection)
		{
			handlersCollection.AddHandler<Application, ApplicationHandler>();
			handlersCollection.AddHandler<Window, WindowHandler>();
			handlersCollection.AddHandler<Label, LabelHandler>();
			handlersCollection.AddHandler<ContentPage, PageHandler>();
			handlersCollection.AddHandler<Layout, LayoutHandler>();
			handlersCollection.AddHandler<Button, ButtonHandler>();
			handlersCollection.AddHandler<Entry, EntryHandler>();
			handlersCollection.AddHandler<Editor, EditorHandler>();
			handlersCollection.AddHandler<Image, ImageHandler>();
			handlersCollection.AddHandler<ScrollView, ScrollViewHandler>();
			handlersCollection.AddHandler<ActivityIndicator, ActivityIndicatorHandler>();
			handlersCollection.AddHandler<ProgressBar, ProgressBarHandler>();
			handlersCollection.AddHandler<Slider, SliderHandler>();
			handlersCollection.AddHandler<Switch, SwitchHandler>();
			handlersCollection.AddHandler<Picker, PickerHandler>();
			handlersCollection.AddHandler<DatePicker, DatePickerHandler>();
			handlersCollection.AddHandler<Stepper, StepperHandler>();
			handlersCollection.AddHandler<CheckBox, CheckBoxHandler>();
			handlersCollection.AddHandler<SearchBar, SearchBarHandler>();
			handlersCollection.AddHandler<TimePicker, TimePickerHandler>();
			handlersCollection.AddHandler<Border, BorderHandler>();
			handlersCollection.AddHandler<ContentView, ContentViewHandler>();
			handlersCollection.AddHandler<BoxView, ShapeViewHandler>();
			handlersCollection.AddHandler<Microsoft.Maui.Controls.Shapes.Rectangle, ShapeViewHandler>();
			handlersCollection.AddHandler<Microsoft.Maui.Controls.Shapes.RoundRectangle, ShapeViewHandler>();
			handlersCollection.AddHandler<Microsoft.Maui.Controls.Shapes.Ellipse, ShapeViewHandler>();
			handlersCollection.AddHandler<Microsoft.Maui.Controls.Shapes.Line, ShapeViewHandler>();
			handlersCollection.AddHandler<AspNetCore.Components.WebView.Maui.BlazorWebView, AspNetCore.Components.WebView.Maui.WPF.BlazorWebViewHandler>();

			return handlersCollection;
		}

		static MauiAppBuilder SetupDefaults(this MauiAppBuilder builder)
		{


#pragma warning disable CS0612, CA1416 // Type or member is obsolete, 'ResourcesProvider' is unsupported on: 'iOS' 14.0 and later
			DependencyService.Register<Platform.WPF.ResourcesProvider>();


			//DependencyService.Register<PlatformSizeService>();
			//DependencyService.Register<FontNamedSizeService>();


			builder.Services.AddSingleton<IDispatcherProvider>(svc =>
				// the DispatcherProvider might have already been initialized, so ensure that we are grabbing the
				// Current and putting it in the DI container.
				new WPFDispatcherProvider());

			builder.Services.AddScoped(svc =>
			{
				var provider = svc.GetRequiredService<IDispatcherProvider>();
				if (DispatcherProvider.SetCurrent(provider))
					svc.GetService<ILogger<Dispatcher>>()?.LogWarning("Replaced an existing DispatcherProvider with one from the service provider.");

				return Dispatcher.GetForCurrentThread()!;
			});

			builder.ConfigureImageSourceHandlers();
			builder
				.ConfigureMauiHandlers(handlers =>
				{
					handlers.AddMauiControlsHandlers();
				});

			//builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IMauiInitializeService, MauiControlsInitializer>());

			builder.RemapForControls();

			return builder;
		}

		class MauiControlsInitializer : IMauiInitializeService
		{
			public void Initialize(IServiceProvider services)
			{
			}
		}


		internal static MauiAppBuilder ConfigureImageSourceHandlers(this MauiAppBuilder builder)
		{
			//builder.ConfigureImageSources(services =>
			//{
			//	services.AddService<FileImageSource>(svcs => new FileImageSourceService(svcs.CreateLogger<FileImageSourceService>()));
			//	services.AddService<FontImageSource>(svcs => new FontImageSourceService(svcs.GetRequiredService<IFontManager>(), svcs.CreateLogger<FontImageSourceService>()));
			//	services.AddService<StreamImageSource>(svcs => new StreamImageSourceService(svcs.CreateLogger<StreamImageSourceService>()));
			//	services.AddService<UriImageSource>(svcs => new UriImageSourceService(svcs.CreateLogger<UriImageSourceService>()));
			//});

			return builder;
		}

		internal static MauiAppBuilder RemapForControls(this MauiAppBuilder builder)
		{
			// Override base ViewMapper entries that target WinUI types
			// so they work with WPF FrameworkElement instead
			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping(nameof(IView.Width), (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.FrameworkElement fe)
				{
					double w = view.Width;
					if (w >= 0)
						fe.Width = w;
					// Don't set NaN — preserve default from CreatePlatformView
				}
			});

			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping(nameof(IView.Height), (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.FrameworkElement fe)
				{
					double h = view.Height;
					if (h >= 0)
						fe.Height = h;
					// Don't set NaN — preserve default from CreatePlatformView
				}
			});

			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping(nameof(IView.MinimumWidth), (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.FrameworkElement fe && !double.IsNaN(view.MinimumWidth) && view.MinimumWidth >= 0)
					fe.MinWidth = view.MinimumWidth;
			});

			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping(nameof(IView.MinimumHeight), (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.FrameworkElement fe && !double.IsNaN(view.MinimumHeight) && view.MinimumHeight >= 0)
					fe.MinHeight = view.MinimumHeight;
			});

			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping(nameof(IView.MaximumWidth), (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.FrameworkElement fe && !double.IsNaN(view.MaximumWidth) && !double.IsInfinity(view.MaximumWidth))
					fe.MaxWidth = view.MaximumWidth;
			});

			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping(nameof(IView.MaximumHeight), (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.FrameworkElement fe && !double.IsNaN(view.MaximumHeight) && !double.IsInfinity(view.MaximumHeight))
					fe.MaxHeight = view.MaximumHeight;
			});

			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping(nameof(IView.Opacity), (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.UIElement ue)
					ue.Opacity = view.Opacity;
			});

			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping(nameof(IView.Visibility), (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.UIElement ue)
				{
					ue.Visibility = view.Visibility switch
					{
						Visibility.Collapsed => System.Windows.Visibility.Collapsed,
						Visibility.Hidden => System.Windows.Visibility.Hidden,
						_ => System.Windows.Visibility.Visible,
					};
				}
			});

			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping(nameof(IView.IsEnabled), (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.UIElement ue)
					ue.IsEnabled = view.IsEnabled;
			});

			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping(nameof(IView.Background), (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.Controls.Control control && view.Background is SolidPaint sp && sp.Color != null)
				{
					var c = sp.Color;
					control.Background = new System.Windows.Media.SolidColorBrush(
						System.Windows.Media.Color.FromArgb(
							(byte)(c.Alpha * 255), (byte)(c.Red * 255),
							(byte)(c.Green * 255), (byte)(c.Blue * 255)));
				}
				else if (handler.PlatformView is System.Windows.Controls.Panel panel && view.Background is SolidPaint sp2 && sp2.Color != null)
				{
					var c = sp2.Color;
					panel.Background = new System.Windows.Media.SolidColorBrush(
						System.Windows.Media.Color.FromArgb(
							(byte)(c.Alpha * 255), (byte)(c.Red * 255),
							(byte)(c.Green * 255), (byte)(c.Blue * 255)));
				}
			});

			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping(nameof(IView.Margin), (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.FrameworkElement fe)
				{
					var m = view.Margin;
					fe.Margin = new System.Windows.Thickness(m.Left, m.Top, m.Right, m.Bottom);
				}
			});

			return builder;
		}
	}
}
