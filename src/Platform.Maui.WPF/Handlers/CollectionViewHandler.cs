using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.Maui.Handlers;
using WBorder = System.Windows.Controls.Border;
using WGrid = System.Windows.Controls.Grid;
using WListBox = System.Windows.Controls.ListBox;
using WScrollViewer = System.Windows.Controls.ScrollViewer;

namespace Microsoft.Maui.Handlers.WPF
{
	public partial class CollectionViewHandler : WPFViewHandler<Microsoft.Maui.Controls.CollectionView, FrameworkElement>
	{
		MauiCollectionListBox? _listBox;
		WGrid? _rootGrid;
		TextBlock? _emptyView;

		public static readonly PropertyMapper<Microsoft.Maui.Controls.CollectionView, CollectionViewHandler> Mapper =
			new(ViewMapper)
			{
				[nameof(Microsoft.Maui.Controls.ItemsView.ItemsSource)] = MapItemsSource,
				[nameof(Microsoft.Maui.Controls.SelectableItemsView.SelectedItem)] = MapSelectedItem,
				[nameof(Microsoft.Maui.Controls.SelectableItemsView.SelectionMode)] = MapSelectionMode,
				[nameof(Microsoft.Maui.Controls.ItemsView.EmptyView)] = MapEmptyView,
			};

		public static readonly CommandMapper<Microsoft.Maui.Controls.CollectionView, CollectionViewHandler> CommandMapper = new(ViewCommandMapper);

		public CollectionViewHandler() : base(Mapper, CommandMapper) { }

		protected override FrameworkElement CreatePlatformView()
		{
			_rootGrid = new WGrid();
			_listBox = new MauiCollectionListBox
			{
				HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch,
				BorderThickness = new System.Windows.Thickness(0),
				Background = System.Windows.Media.Brushes.Transparent,
			};

			_listBox.SelectionChanged += OnSelectionChanged;

			_emptyView = new TextBlock
			{
				Text = "No items",
				HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
				VerticalAlignment = System.Windows.VerticalAlignment.Center,
				Foreground = System.Windows.Media.Brushes.Gray,
				FontSize = 16,
				Visibility = System.Windows.Visibility.Collapsed,
			};

			_rootGrid.Children.Add(_listBox);
			_rootGrid.Children.Add(_emptyView);

			return _rootGrid;
		}

		void OnSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (VirtualView == null || _listBox == null) return;

			// When MAUI SelectionMode is None, prevent WPF selection
			if (VirtualView.SelectionMode == Microsoft.Maui.Controls.SelectionMode.None)
			{
				_listBox.UnselectAll();
				return;
			}

			if (VirtualView.SelectionMode == Microsoft.Maui.Controls.SelectionMode.Single)
			{
				VirtualView.SelectedItem = _listBox.SelectedItem;
			}
			else if (VirtualView.SelectionMode == Microsoft.Maui.Controls.SelectionMode.Multiple)
			{
				var selected = new System.Collections.Generic.List<object>();
				foreach (var item in _listBox.SelectedItems)
					selected.Add(item);
				VirtualView.SelectedItems = selected;
			}
		}

		static void MapItemsSource(CollectionViewHandler handler, Microsoft.Maui.Controls.CollectionView view)
		{
			if (handler._listBox == null) return;

			handler._listBox.MauiTemplate = view.ItemTemplate;
			handler._listBox.MauiCollectionView = view;
			handler._listBox.ItemsSource = view.ItemsSource as IEnumerable;
			handler.UpdateEmptyView();
		}

		void UpdateEmptyView()
		{
			if (_listBox == null || _emptyView == null || VirtualView == null) return;

			bool hasItems = false;
			if (_listBox.ItemsSource is IEnumerable items)
			{
				var enumerator = items.GetEnumerator();
				hasItems = enumerator.MoveNext();
				(enumerator as IDisposable)?.Dispose();
			}

			_emptyView.Visibility = hasItems ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
			_listBox.Visibility = hasItems ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

			if (!hasItems && VirtualView.EmptyView is string s)
				_emptyView.Text = s;
			else if (!hasItems && VirtualView.EmptyView is View emptyMauiView)
				_emptyView.Text = emptyMauiView.ToString() ?? "No items";
		}

		static void MapSelectedItem(CollectionViewHandler handler, Microsoft.Maui.Controls.CollectionView view)
		{
			if (handler._listBox == null) return;
			if (handler._listBox.SelectedItem != view.SelectedItem)
				handler._listBox.SelectedItem = view.SelectedItem;
		}

		static void MapSelectionMode(CollectionViewHandler handler, Microsoft.Maui.Controls.CollectionView view)
		{
			if (handler._listBox == null) return;
			handler._listBox.SelectionMode = view.SelectionMode switch
			{
				Microsoft.Maui.Controls.SelectionMode.None => System.Windows.Controls.SelectionMode.Single,
				Microsoft.Maui.Controls.SelectionMode.Single => System.Windows.Controls.SelectionMode.Single,
				Microsoft.Maui.Controls.SelectionMode.Multiple => System.Windows.Controls.SelectionMode.Extended,
				_ => System.Windows.Controls.SelectionMode.Single,
			};
		}

		static void MapEmptyView(CollectionViewHandler handler, Microsoft.Maui.Controls.CollectionView view)
		{
			handler.UpdateEmptyView();
		}

		protected override void DisconnectHandler(FrameworkElement platformView)
		{
			if (_listBox != null)
				_listBox.SelectionChanged -= OnSelectionChanged;
			base.DisconnectHandler(platformView);
		}
	}

	/// <summary>
	/// Custom ListBox that creates MAUI-templated items via PrepareContainerForItemOverride.
	/// </summary>
	public class MauiCollectionListBox : WListBox
	{
		internal Microsoft.Maui.Controls.DataTemplate? MauiTemplate { get; set; }
		internal Microsoft.Maui.Controls.CollectionView? MauiCollectionView { get; set; }

		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);

			if (element is ListBoxItem lbi && MauiTemplate != null && MauiCollectionView != null)
			{
				lbi.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
				lbi.Padding = new System.Windows.Thickness(0);

				try
				{
					// Resolve DataTemplateSelector
					var resolvedTemplate = MauiTemplate;
					if (MauiTemplate is Microsoft.Maui.Controls.DataTemplateSelector selector)
						resolvedTemplate = selector.SelectTemplate(item, MauiCollectionView);

					if (resolvedTemplate == null) return;

					var content = resolvedTemplate.CreateContent() as View;
					if (content == null) return;

					content.BindingContext = item;

					var mauiContext = MauiCollectionView.Handler?.MauiContext;
					if (mauiContext == null) return;

					var platformView = Microsoft.Maui.Platform.ElementExtensions.ToPlatform((IElement)content, mauiContext);
					lbi.Content = platformView;
				}
				catch (Exception ex)
				{
					lbi.Content = new TextBlock
					{
						Text = item?.ToString() ?? "",
						TextWrapping = TextWrapping.Wrap,
						Padding = new System.Windows.Thickness(8, 4, 8, 4),
					};
					System.Diagnostics.Debug.WriteLine($"[CollectionView] Template error: {ex.Message}");
				}
			}
		}
	}
}
