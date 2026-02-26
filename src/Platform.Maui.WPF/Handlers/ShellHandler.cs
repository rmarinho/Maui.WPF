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
using WButton = System.Windows.Controls.Button;
using WGrid = System.Windows.Controls.Grid;
using WBorder = System.Windows.Controls.Border;
using WBrush = System.Windows.Media.Brush;

namespace Microsoft.Maui.Handlers.WPF
{
	/// <summary>
	/// Shell handler — provides flyout navigation, tabs, and URI-based routing.
	/// Maps MAUI Shell → WPF Grid with optional flyout panel + TabControl + content area.
	/// </summary>
	public class ShellContainerView : WGrid
	{
		readonly WGrid _flyoutPanel;
		readonly StackPanel _flyoutItems;
		readonly ContentControl _contentArea;
		readonly TabControl _tabControl;
		readonly WButton _hamburgerButton;
		readonly WBorder _flyoutOverlay;
		bool _flyoutOpen;

		public Action<ShellItem>? OnShellItemSelected { get; set; }
		public Action? OnFlyoutToggled { get; set; }

		public ShellContainerView()
		{
			// Two columns: flyout (auto/hidden) + main content
			ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition { Width = System.Windows.GridLength.Auto });
			ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });

			// Flyout panel
			_flyoutPanel = new WGrid
			{
				Width = 250,
				Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(45, 45, 48)),
				Visibility = System.Windows.Visibility.Collapsed,
			};
			_flyoutPanel.RowDefinitions.Add(new System.Windows.Controls.RowDefinition { Height = new System.Windows.GridLength(50) });
			_flyoutPanel.RowDefinitions.Add(new System.Windows.Controls.RowDefinition { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });

			// Flyout header area
			var headerPanel = new DockPanel { Margin = new System.Windows.Thickness(12, 8, 8, 8) };
			var closeBtn = new WButton { Content = "✕", Width = 30, Height = 30, HorizontalAlignment = System.Windows.HorizontalAlignment.Right };
			closeBtn.Click += (s, e) => ToggleFlyout(false);
			DockPanel.SetDock(closeBtn, Dock.Right);
			headerPanel.Children.Add(closeBtn);
			headerPanel.Children.Add(new TextBlock { Text = "Menu", FontSize = 16, Foreground = System.Windows.Media.Brushes.White, VerticalAlignment = System.Windows.VerticalAlignment.Center });
			SetRow(headerPanel, 0);
			_flyoutPanel.Children.Add(headerPanel);

			// Flyout items list
			var scrollViewer = new ScrollViewer { VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto };
			_flyoutItems = new StackPanel();
			scrollViewer.Content = _flyoutItems;
			SetRow(scrollViewer, 1);
			_flyoutPanel.Children.Add(scrollViewer);
			SetColumn(_flyoutPanel, 0);

			// Semi-transparent overlay
			_flyoutOverlay = new WBorder
			{
				Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(80, 0, 0, 0)),
				Visibility = System.Windows.Visibility.Collapsed,
			};
			_flyoutOverlay.MouseLeftButtonDown += (s, e) => ToggleFlyout(false);

			// Main content grid
			var mainGrid = new WGrid();
			mainGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition { Height = new System.Windows.GridLength(44) });
			mainGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition { Height = System.Windows.GridLength.Auto });
			mainGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });

			// Toolbar with hamburger
			var toolbar = new DockPanel
			{
				Height = 44,
				Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(240, 240, 240)),
			};
			_hamburgerButton = new WButton
			{
				Content = "☰",
				FontSize = 18,
				Width = 44,
				Height = 44,
				Visibility = System.Windows.Visibility.Collapsed,
			};
			_hamburgerButton.Click += (s, e) => ToggleFlyout(!_flyoutOpen);
			DockPanel.SetDock(_hamburgerButton, Dock.Left);
			toolbar.Children.Add(_hamburgerButton);
			toolbar.Children.Add(new TextBlock()); // spacer
			SetRow(toolbar, 0);
			mainGrid.Children.Add(toolbar);

			// Tab control
			_tabControl = new TabControl
			{
				Visibility = System.Windows.Visibility.Collapsed,
			};
			_tabControl.SelectionChanged += TabControl_SelectionChanged;
			SetRow(_tabControl, 1);
			mainGrid.Children.Add(_tabControl);

			// Content area
			_contentArea = new ContentControl
			{
				HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch,
				VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch,
			};
			SetRow(_contentArea, 2);
			mainGrid.Children.Add(_contentArea);

			SetColumn(mainGrid, 1);
			Children.Add(_flyoutPanel);
			Children.Add(mainGrid);
		}

		void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			// TabItem.Tag contains the ShellSection
			if (_tabControl.SelectedItem is TabItem tabItem && tabItem.Tag is ShellSection section)
			{
				var page = section.CurrentItem?.ContentTemplate?.CreateContent() as Microsoft.Maui.Controls.Page;
				if (page == null && section.CurrentItem is ShellContent content)
				{
					page = content.ContentTemplate?.CreateContent() as Microsoft.Maui.Controls.Page;
				}
			}
		}

		public void ToggleFlyout(bool open)
		{
			_flyoutOpen = open;
			_flyoutPanel.Visibility = open ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
			_flyoutOverlay.Visibility = open ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
			OnFlyoutToggled?.Invoke();
		}

		public void SetFlyoutBehavior(FlyoutBehavior behavior)
		{
			switch (behavior)
			{
				case FlyoutBehavior.Disabled:
					_hamburgerButton.Visibility = System.Windows.Visibility.Collapsed;
					_flyoutPanel.Visibility = System.Windows.Visibility.Collapsed;
					break;
				case FlyoutBehavior.Flyout:
					_hamburgerButton.Visibility = System.Windows.Visibility.Visible;
					break;
				case FlyoutBehavior.Locked:
					_hamburgerButton.Visibility = System.Windows.Visibility.Collapsed;
					_flyoutPanel.Visibility = System.Windows.Visibility.Visible;
					break;
			}
		}

		public void SetFlyoutBackground(WBrush? brush)
		{
			if (brush != null)
				_flyoutPanel.Background = brush;
		}

		public void BuildShellItems(Shell shell, IMauiContext mauiContext)
		{
			_flyoutItems.Children.Clear();
			_tabControl.Items.Clear();

			bool hasFlyoutItems = false;
			bool hasTabs = false;

			foreach (var item in shell.Items)
			{
				// Add to flyout
				if (item.FlyoutItemIsVisible)
				{
					hasFlyoutItems = true;
					var btn = new WButton
					{
						Content = item.Title ?? item.Route ?? "Item",
						HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left,
						Foreground = System.Windows.Media.Brushes.White,
						Background = System.Windows.Media.Brushes.Transparent,
						BorderThickness = new System.Windows.Thickness(0),
						Padding = new System.Windows.Thickness(16, 12, 16, 12),
						FontSize = 14,
						Tag = item,
					};
					btn.Click += (s, e) =>
					{
						OnShellItemSelected?.Invoke(item);
						ToggleFlyout(false);
					};
					_flyoutItems.Children.Add(btn);
				}

				// If this item has sections, create tabs
				if (item.Items.Count > 1)
				{
					hasTabs = true;
					foreach (var section in item.Items)
					{
						var tabItem = new TabItem
						{
							Header = section.Title ?? section.Route ?? "Tab",
							Tag = section,
						};
						_tabControl.Items.Add(tabItem);
					}
				}
			}

			if (hasFlyoutItems)
				SetFlyoutBehavior(shell.FlyoutBehavior);
			else
				SetFlyoutBehavior(FlyoutBehavior.Disabled);

			_tabControl.Visibility = hasTabs ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

			// Show first item's content
			var firstItem = shell.Items.FirstOrDefault();
			if (firstItem != null)
				ShowShellItem(firstItem, mauiContext);
		}

		public void ShowShellItem(ShellItem item, IMauiContext mauiContext)
		{
			try
			{
				var section = item.Items.FirstOrDefault();
				var content = section?.Items.FirstOrDefault();
				if (content != null)
				{
					var page = content.ContentTemplate?.CreateContent() as Microsoft.Maui.Controls.Page
						?? content.Content as Microsoft.Maui.Controls.Page;

					if (page != null)
					{
						var platformView = Microsoft.Maui.Platform.ElementExtensions.ToPlatform((IElement)page, mauiContext);
						_contentArea.Content = platformView;
					}
				}
			}
			catch { }
		}

		public void SetContent(FrameworkElement? content)
		{
			_contentArea.Content = content;
		}
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
			new(ViewCommandMapper)
			{
			};

		public ShellHandler() : base(Mapper, CommandMapper) { }

		protected override ShellContainerView CreatePlatformView()
		{
			var container = new ShellContainerView();
			container.OnShellItemSelected = OnShellItemSelected;
			return container;
		}

		protected override void ConnectHandler(ShellContainerView platformView)
		{
			base.ConnectHandler(platformView);
			if (VirtualView != null && MauiContext != null)
				platformView.BuildShellItems(VirtualView, MauiContext);
		}

		void OnShellItemSelected(ShellItem item)
		{
			if (VirtualView != null)
				VirtualView.CurrentItem = item;
		}

		static void MapFlyoutBehavior(ShellHandler handler, Shell shell)
		{
			handler.PlatformView.SetFlyoutBehavior(shell.FlyoutBehavior);
		}

		static void MapFlyoutBackground(ShellHandler handler, Shell shell)
		{
			if (shell.FlyoutBackgroundColor != null)
			{
				var c = shell.FlyoutBackgroundColor;
				handler.PlatformView.SetFlyoutBackground(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(
					(byte)(c.Alpha * 255), (byte)(c.Red * 255),
					(byte)(c.Green * 255), (byte)(c.Blue * 255))));
			}
		}

		static void MapItems(ShellHandler handler, Shell shell)
		{
			if (handler.MauiContext != null)
				handler.PlatformView.BuildShellItems(shell, handler.MauiContext);
		}

		static void MapCurrentItem(ShellHandler handler, Shell shell)
		{
			if (shell.CurrentItem != null && handler.MauiContext != null)
				handler.PlatformView.ShowShellItem(shell.CurrentItem, handler.MauiContext);
		}
	}
}


