#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using WButton = global::System.Windows.Controls.Button;
using WGrid = global::System.Windows.Controls.Grid;
using WBorder = global::System.Windows.Controls.Border;
using WBrush = global::System.Windows.Media.Brush;
using WColumnDefinition = global::System.Windows.Controls.ColumnDefinition;
using WRowDefinition = global::System.Windows.Controls.RowDefinition;
using WVisibility = global::System.Windows.Visibility;
using WThickness = global::System.Windows.Thickness;
using WHorizontalAlignment = global::System.Windows.HorizontalAlignment;
using WVerticalAlignment = global::System.Windows.VerticalAlignment;
using WSolidColorBrush = global::System.Windows.Media.SolidColorBrush;
using WColor = global::System.Windows.Media.Color;
using WGridLength = global::System.Windows.GridLength;
using WGridUnitType = global::System.Windows.GridUnitType;
using WFontWeights = global::System.Windows.FontWeights;

namespace Microsoft.Maui.Handlers.WPF
{
	/// <summary>
	/// Shell container — provides flyout navigation, toolbar with back button, and content area.
	/// </summary>
	public class ShellContainerView : WGrid
	{
		readonly WGrid _flyoutPanel;
		readonly global::System.Windows.Controls.StackPanel _flyoutItems;
		readonly global::System.Windows.Controls.ContentControl _contentArea;
		readonly global::System.Windows.Controls.TabControl _tabControl;
		readonly WButton _hamburgerButton;
		readonly WButton _backButton;
		readonly global::System.Windows.Controls.TextBlock _titleLabel;
		readonly WBorder _flyoutOverlay;
		readonly global::System.Windows.Controls.DockPanel _toolbar;
		bool _flyoutOpen;

		public Action<ShellItem>? OnShellItemSelected { get; set; }
		public Action? OnBackButtonClicked { get; set; }

		public ShellContainerView()
		{
			ColumnDefinitions.Add(new WColumnDefinition { Width = WGridLength.Auto });
			ColumnDefinitions.Add(new WColumnDefinition { Width = new WGridLength(1, WGridUnitType.Star) });

			// Flyout panel
			_flyoutPanel = new WGrid
			{
				Width = 250,
				Background = new WSolidColorBrush(WColor.FromRgb(45, 45, 48)),
				Visibility = WVisibility.Collapsed,
			};
			_flyoutPanel.RowDefinitions.Add(new WRowDefinition { Height = new WGridLength(50) });
			_flyoutPanel.RowDefinitions.Add(new WRowDefinition { Height = new WGridLength(1, WGridUnitType.Star) });

			var headerPanel = new global::System.Windows.Controls.DockPanel { Margin = new WThickness(12, 8, 8, 8) };
			var closeBtn = new WButton { Content = "✕", Width = 30, Height = 30, HorizontalAlignment = WHorizontalAlignment.Right };
			closeBtn.Click += (s, e) => ToggleFlyout(false);
			global::System.Windows.Controls.DockPanel.SetDock(closeBtn, global::System.Windows.Controls.Dock.Right);
			headerPanel.Children.Add(closeBtn);
			headerPanel.Children.Add(new global::System.Windows.Controls.TextBlock { Text = "Menu", FontSize = 16, Foreground = global::System.Windows.Media.Brushes.White, VerticalAlignment = WVerticalAlignment.Center });
			SetRow(headerPanel, 0);
			_flyoutPanel.Children.Add(headerPanel);

			var scrollViewer = new global::System.Windows.Controls.ScrollViewer { VerticalScrollBarVisibility = global::System.Windows.Controls.ScrollBarVisibility.Auto };
			_flyoutItems = new global::System.Windows.Controls.StackPanel();
			scrollViewer.Content = _flyoutItems;
			SetRow(scrollViewer, 1);
			_flyoutPanel.Children.Add(scrollViewer);
			SetColumn(_flyoutPanel, 0);

			_flyoutOverlay = new WBorder
			{
				Background = new WSolidColorBrush(WColor.FromArgb(80, 0, 0, 0)),
				Visibility = WVisibility.Collapsed,
			};
			_flyoutOverlay.MouseLeftButtonDown += (s, e) => ToggleFlyout(false);

			// Main content grid
			var mainGrid = new WGrid();
			mainGrid.RowDefinitions.Add(new WRowDefinition { Height = new WGridLength(44) });
			mainGrid.RowDefinitions.Add(new WRowDefinition { Height = WGridLength.Auto });
			mainGrid.RowDefinitions.Add(new WRowDefinition { Height = new WGridLength(1, WGridUnitType.Star) });

			// Toolbar
			_toolbar = new global::System.Windows.Controls.DockPanel
			{
				Height = 44,
				Background = new WSolidColorBrush(WColor.FromRgb(240, 240, 240)),
			};

			_hamburgerButton = new WButton
			{
				Content = "☰", FontSize = 18, Width = 44, Height = 44,
				Visibility = WVisibility.Collapsed,
			};
			_hamburgerButton.Click += (s, e) => ToggleFlyout(!_flyoutOpen);
			global::System.Windows.Controls.DockPanel.SetDock(_hamburgerButton, global::System.Windows.Controls.Dock.Left);
			_toolbar.Children.Add(_hamburgerButton);

			_backButton = new WButton
			{
				Content = "← Back", Margin = new WThickness(4, 4, 4, 4),
				Padding = new WThickness(8, 2, 8, 2),
				VerticalAlignment = WVerticalAlignment.Center,
				Visibility = WVisibility.Collapsed,
			};
			_backButton.Click += (s, e) => OnBackButtonClicked?.Invoke();
			global::System.Windows.Controls.DockPanel.SetDock(_backButton, global::System.Windows.Controls.Dock.Left);
			_toolbar.Children.Add(_backButton);

			_titleLabel = new global::System.Windows.Controls.TextBlock
			{
				FontSize = 16, FontWeight = WFontWeights.SemiBold,
				VerticalAlignment = WVerticalAlignment.Center,
				Margin = new WThickness(8, 0, 0, 0),
			};
			_toolbar.Children.Add(_titleLabel);

			SetRow(_toolbar, 0);
			mainGrid.Children.Add(_toolbar);

			_tabControl = new global::System.Windows.Controls.TabControl { Visibility = WVisibility.Collapsed };
			_tabControl.SelectionChanged += TabControl_SelectionChanged;
			SetRow(_tabControl, 1);
			mainGrid.Children.Add(_tabControl);

			_contentArea = new global::System.Windows.Controls.ContentControl
			{
				HorizontalContentAlignment = WHorizontalAlignment.Stretch,
				VerticalContentAlignment = WVerticalAlignment.Stretch,
			};
			SetRow(_contentArea, 2);
			mainGrid.Children.Add(_contentArea);

			SetColumn(mainGrid, 1);
			Children.Add(_flyoutPanel);
			Children.Add(mainGrid);
		}

		void TabControl_SelectionChanged(object sender, global::System.Windows.Controls.SelectionChangedEventArgs e) { }

		public void ToggleFlyout(bool open)
		{
			_flyoutOpen = open;
			_flyoutPanel.Visibility = open ? WVisibility.Visible : WVisibility.Collapsed;
			_flyoutOverlay.Visibility = open ? WVisibility.Visible : WVisibility.Collapsed;
		}

		public void SetFlyoutBehavior(FlyoutBehavior behavior)
		{
			switch (behavior)
			{
				case FlyoutBehavior.Disabled:
					_hamburgerButton.Visibility = WVisibility.Collapsed;
					_flyoutPanel.Visibility = WVisibility.Collapsed;
					break;
				case FlyoutBehavior.Flyout:
					_hamburgerButton.Visibility = WVisibility.Visible;
					break;
				case FlyoutBehavior.Locked:
					_hamburgerButton.Visibility = WVisibility.Collapsed;
					_flyoutPanel.Visibility = WVisibility.Visible;
					break;
			}
		}

		public void SetFlyoutBackground(WBrush? brush)
		{
			if (brush != null) _flyoutPanel.Background = brush;
		}

		public void BuildFlyoutItems(Shell shell)
		{
			_flyoutItems.Children.Clear();
			_tabControl.Items.Clear();

			bool hasFlyoutItems = false;
			bool hasTabs = false;

			foreach (var item in shell.Items)
			{
				if (item.FlyoutItemIsVisible)
				{
					hasFlyoutItems = true;
					var capturedItem = item;
					var btn = new WButton
					{
						Content = item.Title ?? item.Route ?? "Item",
						HorizontalContentAlignment = WHorizontalAlignment.Left,
						Foreground = global::System.Windows.Media.Brushes.White,
						Background = global::System.Windows.Media.Brushes.Transparent,
						BorderThickness = new WThickness(0),
						Padding = new WThickness(16, 12, 16, 12),
						FontSize = 14, Tag = item,
					};
					btn.Click += (s, e) =>
					{
						OnShellItemSelected?.Invoke(capturedItem);
						ToggleFlyout(false);
					};
					_flyoutItems.Children.Add(btn);
				}

				if (item.Items.Count > 1)
				{
					hasTabs = true;
					foreach (var section in item.Items)
					{
						var tabItem = new global::System.Windows.Controls.TabItem { Header = section.Title ?? section.Route ?? "Tab", Tag = section };
						_tabControl.Items.Add(tabItem);
					}
				}
			}

			if (hasFlyoutItems)
				SetFlyoutBehavior(shell.FlyoutBehavior);
			else
				SetFlyoutBehavior(FlyoutBehavior.Disabled);

			_tabControl.Visibility = hasTabs ? WVisibility.Visible : WVisibility.Collapsed;
		}

		public void ShowPage(FrameworkElement? content, string? title, bool showBack)
		{
			_contentArea.Content = content;
			_titleLabel.Text = title ?? string.Empty;
			_backButton.Visibility = showBack ? WVisibility.Visible : WVisibility.Collapsed;
		}

		public void SetContent(FrameworkElement? content) => _contentArea.Content = content;
	}

	public partial class ShellHandler : WPFViewHandler<Shell, ShellContainerView>
	{
		public static IPropertyMapper<Shell, ShellHandler> Mapper =
			new PropertyMapper<Shell, ShellHandler>(ViewMapper)
			{
				[nameof(Shell.FlyoutBehavior)] = MapFlyoutBehavior,
				[nameof(Shell.FlyoutBackgroundColor)] = MapFlyoutBackground,
				[nameof(Shell.Items)] = MapItems,
				[nameof(Shell.CurrentItem)] = MapCurrentItem,
			};

		public static CommandMapper<Shell, ShellHandler> CommandMapper =
			new(ViewCommandMapper) { };

		public ShellHandler() : base(Mapper, CommandMapper) { }

		protected override ShellContainerView CreatePlatformView()
		{
			var container = new ShellContainerView();
			container.OnShellItemSelected = OnShellItemSelected;
			container.OnBackButtonClicked = OnBackButtonClicked;
			return container;
		}

		protected override void ConnectHandler(ShellContainerView platformView)
		{
			base.ConnectHandler(platformView);
			if (VirtualView != null)
			{
				VirtualView.Navigated += OnShellNavigated;
				VirtualView.Navigating += OnShellNavigating;
				VirtualView.PropertyChanged += OnShellPropertyChanged;
				if (MauiContext != null)
				{
					platformView.BuildFlyoutItems(VirtualView);
					ShowCurrentPage();
				}
			}
		}

		protected override void DisconnectHandler(ShellContainerView platformView)
		{
			if (VirtualView != null)
			{
				VirtualView.Navigated -= OnShellNavigated;
				VirtualView.Navigating -= OnShellNavigating;
				VirtualView.PropertyChanged -= OnShellPropertyChanged;
			}
			base.DisconnectHandler(platformView);
		}

		void OnShellNavigating(object? sender, ShellNavigatingEventArgs e)
		{
			// Nothing needed here yet, but we track this for debugging
		}

		void OnShellNavigated(object? sender, ShellNavigatedEventArgs e)
		{
			// Delay slightly to let MAUI finish updating CurrentPage
			PlatformView?.Dispatcher.InvokeAsync(() => ShowCurrentPage(), 
				System.Windows.Threading.DispatcherPriority.Background);
		}

		void OnShellPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "CurrentPage" || e.PropertyName == "CurrentItem")
			{
				PlatformView?.Dispatcher.InvokeAsync(() => ShowCurrentPage(),
					System.Windows.Threading.DispatcherPriority.Background);
			}
		}

		void ShowCurrentPage()
		{
			if (VirtualView == null || MauiContext == null) return;

			try
			{
				Microsoft.Maui.Controls.Page? currentPage = VirtualView.CurrentPage;
				var section = VirtualView.CurrentItem?.CurrentItem;

				// If CurrentPage is null, walk Shell hierarchy to find the page
				if (currentPage == null)
				{
					// Check navigation stack first (for pushed pages via GoToAsync)
					if (section?.Stack?.Count > 1)
						currentPage = section.Stack[section.Stack.Count - 1];

					// Fall back to ShellContent template
					if (currentPage == null)
					{
						var content = section?.CurrentItem;
						if (content != null)
						{
							try { currentPage = ((IShellContentController)content).Page; }
							catch { }

							if (currentPage == null)
							{
								currentPage = content.ContentTemplate?.CreateContent() as Microsoft.Maui.Controls.Page
									?? content.Content as Microsoft.Maui.Controls.Page;
							}
						}
					}
				}

				if (currentPage == null) return;

				var platformView = Microsoft.Maui.Platform.ElementExtensions.ToPlatform((IElement)currentPage, MauiContext);
				var title = currentPage.Title ?? VirtualView.CurrentItem?.Title ?? string.Empty;
				bool hasNavStack = section?.Stack?.Count > 1;

				PlatformView.ShowPage(platformView as FrameworkElement, title, hasNavStack);
			}
			catch { }
		}

		void OnShellItemSelected(ShellItem item)
		{
			if (VirtualView == null) return;
			VirtualView.CurrentItem = item;
		}

		async void OnBackButtonClicked()
		{
			if (VirtualView == null) return;

			try
			{
				await VirtualView.GoToAsync("..");
				// GoToAsync may not fire Navigated, so refresh manually
				ShowCurrentPage();
			}
			catch { }
		}

		static void MapFlyoutBehavior(ShellHandler handler, Shell shell)
			=> handler.PlatformView.SetFlyoutBehavior(shell.FlyoutBehavior);

		static void MapFlyoutBackground(ShellHandler handler, Shell shell)
		{
			if (shell.FlyoutBackgroundColor != null)
			{
				var c = shell.FlyoutBackgroundColor;
				handler.PlatformView.SetFlyoutBackground(new WSolidColorBrush(WColor.FromArgb(
					(byte)(c.Alpha * 255), (byte)(c.Red * 255),
					(byte)(c.Green * 255), (byte)(c.Blue * 255))));
			}
		}

		static void MapItems(ShellHandler handler, Shell shell)
		{
			handler.PlatformView.BuildFlyoutItems(shell);
			handler.ShowCurrentPage();
		}

		static void MapCurrentItem(ShellHandler handler, Shell shell)
			=> handler.ShowCurrentPage();
	}
}
