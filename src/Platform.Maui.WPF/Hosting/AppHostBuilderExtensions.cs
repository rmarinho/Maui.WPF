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

			// Navigation handlers
			handlersCollection.AddHandler<NavigationPage, NavigationViewHandler>();
			handlersCollection.AddHandler<TabbedPage, TabbedViewHandler>();
			handlersCollection.AddHandler<FlyoutPage, FlyoutViewHandler>();

			// Collection handlers
			handlersCollection.AddHandler<CollectionView, Microsoft.Maui.Handlers.WPF.CollectionViewHandler>();

			// Additional control handlers
			handlersCollection.AddHandler<RadioButton, RadioButtonHandler>();
			handlersCollection.AddHandler<ImageButton, ImageButtonHandler>();

			// WebView
			handlersCollection.AddHandler<Microsoft.Maui.Controls.WebView, WebViewHandler>();

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

			// Register WPF-specific animation ticker (runs on dispatcher thread)
			builder.Services.AddSingleton<Microsoft.Maui.Animations.Ticker>(new Microsoft.Maui.Platform.WPF.WPFTicker());

			// Register alert/prompt/action sheet handler
			Microsoft.Maui.Platform.WPF.WPFAlertManagerSubscription.Register(builder.Services);

			// Initialize theme detection
			Microsoft.Maui.Platform.WPF.ThemeManager.Initialize();

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

			// Transforms — TranslationX/Y, Rotation, Scale, Anchor
			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping(nameof(IView.TranslationX), (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.FrameworkElement fe)
					UpdateTransformGroup(fe, view);
			});
			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping(nameof(IView.TranslationY), (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.FrameworkElement fe)
					UpdateTransformGroup(fe, view);
			});
			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping(nameof(IView.Rotation), (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.FrameworkElement fe)
					UpdateTransformGroup(fe, view);
			});
			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping(nameof(IView.RotationX), (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.FrameworkElement fe)
					UpdateTransformGroup(fe, view);
			});
			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping(nameof(IView.RotationY), (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.FrameworkElement fe)
					UpdateTransformGroup(fe, view);
			});
			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping(nameof(IView.Scale), (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.FrameworkElement fe)
					UpdateTransformGroup(fe, view);
			});
			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping(nameof(IView.ScaleX), (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.FrameworkElement fe)
					UpdateTransformGroup(fe, view);
			});
			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping(nameof(IView.ScaleY), (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.FrameworkElement fe)
					UpdateTransformGroup(fe, view);
			});
			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping(nameof(IView.AnchorX), (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.FrameworkElement fe)
					UpdateTransformGroup(fe, view);
			});
			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping(nameof(IView.AnchorY), (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.FrameworkElement fe)
					UpdateTransformGroup(fe, view);
			});

			// Shadow → DropShadowEffect
			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping(nameof(IView.Shadow), (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.UIElement ue)
				{
					var shadow = view.Shadow;
					if (shadow != null && shadow.Paint is SolidPaint sp && sp.Color != null)
					{
						var c = sp.Color;
						ue.Effect = new System.Windows.Media.Effects.DropShadowEffect
						{
							Color = System.Windows.Media.Color.FromArgb(
								(byte)(c.Alpha * 255), (byte)(c.Red * 255),
								(byte)(c.Green * 255), (byte)(c.Blue * 255)),
							BlurRadius = Math.Abs(shadow.Radius),
							ShadowDepth = Math.Sqrt(shadow.Offset.X * shadow.Offset.X + shadow.Offset.Y * shadow.Offset.Y),
							Direction = Math.Atan2(-shadow.Offset.Y, shadow.Offset.X) * 180.0 / Math.PI,
							Opacity = shadow.Opacity
						};
					}
					else
					{
						ue.Effect = null;
					}
				}
			});

			// Clip → UIElement.Clip
			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping(nameof(IView.Clip), (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.UIElement ue)
				{
					var clipShape = view.Clip;
					if (clipShape != null)
					{
						try
						{
							var w = view.Frame.Width > 0 ? view.Frame.Width : 100;
							var h = view.Frame.Height > 0 ? view.Frame.Height : 100;
							var path = clipShape.PathForBounds(new Graphics.Rect(0, 0, w, h));
							if (path != null)
							{
								var pathStr = path.ToDefinitionString();
								if (!string.IsNullOrEmpty(pathStr))
									ue.Clip = System.Windows.Media.Geometry.Parse(pathStr);
							}
						}
						catch { }
					}
					else
					{
						ue.Clip = null;
					}
				}
			});

			// InputTransparent → IsHitTestVisible
			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping(nameof(IView.InputTransparent), (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.UIElement ue)
					ue.IsHitTestVisible = !view.InputTransparent;
			});

			// FlowDirection
			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping(nameof(IView.FlowDirection), (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.FrameworkElement fe)
				{
					fe.FlowDirection = view.FlowDirection == Microsoft.Maui.FlowDirection.RightToLeft
						? System.Windows.FlowDirection.RightToLeft
						: System.Windows.FlowDirection.LeftToRight;
				}
			});

			// AutomationId
			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping(nameof(IView.AutomationId), (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.UIElement ue && !string.IsNullOrEmpty(view.AutomationId))
					System.Windows.Automation.AutomationProperties.SetAutomationId(ue, view.AutomationId);
			});

			// Semantics → Accessibility
			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping(nameof(IView.Semantics), (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.UIElement ue && view.Semantics != null)
				{
					if (!string.IsNullOrEmpty(view.Semantics.Description))
						System.Windows.Automation.AutomationProperties.SetName(ue, view.Semantics.Description);
					if (!string.IsNullOrEmpty(view.Semantics.Hint))
						System.Windows.Automation.AutomationProperties.SetHelpText(ue, view.Semantics.Hint);
				}
			});

			// ToolTip
			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping("ToolTip", (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.FrameworkElement fe)
				{
					object? tip = null;
					try
					{
						tip = ToolTipProperties.GetText(view as BindableObject);
					}
					catch { }
					if (tip is string s && !string.IsNullOrEmpty(s))
						fe.ToolTip = s;
					else
						fe.ToolTip = null;
				}
			});

			// Wire gesture recognizers to WPF mouse events
			Microsoft.Maui.Handlers.ViewHandler.ViewMapper.ModifyMapping("GestureRecognizers", (handler, view, _) =>
			{
				if (handler.PlatformView is System.Windows.FrameworkElement fe)
				{
					Microsoft.Maui.Platform.WPF.GestureManager.SetupGestures(fe, view);
				}
			});

			return builder;
		}

		static void UpdateTransformGroup(System.Windows.FrameworkElement fe, IView view)
		{
			var group = fe.RenderTransform as System.Windows.Media.TransformGroup;
			if (group == null)
			{
				group = new System.Windows.Media.TransformGroup();
				group.Children.Add(new System.Windows.Media.ScaleTransform());
				group.Children.Add(new System.Windows.Media.RotateTransform());
				group.Children.Add(new System.Windows.Media.TranslateTransform());
				fe.RenderTransform = group;
			}

			if (group.Children[0] is System.Windows.Media.ScaleTransform scale)
			{
				scale.ScaleX = view.Scale * view.ScaleX;
				scale.ScaleY = view.Scale * view.ScaleY;
			}
			if (group.Children[1] is System.Windows.Media.RotateTransform rotate)
			{
				rotate.Angle = view.Rotation;
			}
			if (group.Children[2] is System.Windows.Media.TranslateTransform translate)
			{
				translate.X = view.TranslationX;
				translate.Y = view.TranslationY;
			}

			fe.RenderTransformOrigin = new System.Windows.Point(view.AnchorX, view.AnchorY);
		}

	}
}
