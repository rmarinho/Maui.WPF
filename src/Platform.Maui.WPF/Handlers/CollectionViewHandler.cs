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
		WListBox? _listBox;
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
			_listBox = new WListBox
			{
				HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch,
				BorderThickness = new System.Windows.Thickness(0),
				Background = System.Windows.Media.Brushes.Transparent,
			};

			// Support horizontal layout via ItemsPanel
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

			handler._listBox.ItemsSource = view.ItemsSource as IEnumerable;
			handler.UpdateEmptyView();

			// Set up item template if available
			handler.UpdateItemTemplate();
		}

		void UpdateItemTemplate()
		{
			if (_listBox == null || VirtualView == null) return;

			var template = VirtualView.ItemTemplate;
			if (template != null)
			{
				_listBox.ItemTemplate = null; // Clear WPF template — we'll use converter
				_listBox.ItemTemplateSelector = new MauiDataTemplateSelector(template, VirtualView);
			}
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
				Microsoft.Maui.Controls.SelectionMode.None => System.Windows.Controls.SelectionMode.Single, // WPF has no "none" — we disable selection handling
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

		// DataTemplateSelector that uses MAUI DataTemplate to create WPF content
		class MauiDataTemplateSelector : System.Windows.Controls.DataTemplateSelector
		{
			readonly Microsoft.Maui.Controls.DataTemplate _mauiTemplate;
			readonly Microsoft.Maui.Controls.CollectionView _collectionView;

			public MauiDataTemplateSelector(Microsoft.Maui.Controls.DataTemplate template, Microsoft.Maui.Controls.CollectionView cv)
			{
				_mauiTemplate = template;
				_collectionView = cv;
			}

			public override System.Windows.DataTemplate SelectTemplate(object item, DependencyObject container)
			{
				// Create a WPF DataTemplate that hosts MAUI content
				var factory = new FrameworkElementFactory(typeof(MauiContentPresenter));
				factory.SetValue(MauiContentPresenter.MauiTemplateProperty, _mauiTemplate);
				factory.SetValue(MauiContentPresenter.MauiContextSourceProperty, _collectionView);

				return new System.Windows.DataTemplate { VisualTree = factory };
			}
		}
	}

	// A WPF control that hosts MAUI templated content
	public class MauiContentPresenter : ContentControl
	{
		public static readonly DependencyProperty MauiTemplateProperty =
			DependencyProperty.Register("MauiTemplate", typeof(Microsoft.Maui.Controls.DataTemplate),
				typeof(MauiContentPresenter), new PropertyMetadata(null));

		public static readonly DependencyProperty MauiContextSourceProperty =
			DependencyProperty.Register("MauiContextSource", typeof(Microsoft.Maui.Controls.CollectionView),
				typeof(MauiContentPresenter), new PropertyMetadata(null));

		public MauiContentPresenter()
		{
			DataContextChanged += OnDataContextChanged;
		}

		void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			try
			{
				var template = GetValue(MauiTemplateProperty) as Microsoft.Maui.Controls.DataTemplate;
				var cv = GetValue(MauiContextSourceProperty) as Microsoft.Maui.Controls.CollectionView;
				if (template == null || cv == null || e.NewValue == null) return;

				var content = template.CreateContent() as View;
				if (content == null) return;

				content.BindingContext = e.NewValue;

				var mauiContext = cv.Handler?.MauiContext;
				if (mauiContext == null) return;

				var platformView = Microsoft.Maui.Platform.ElementExtensions.ToPlatform((IElement)content, mauiContext);
				Content = platformView;
			}
			catch
			{
				// Fallback: display as text
				Content = new TextBlock { Text = e.NewValue?.ToString() ?? "" };
			}
		}
	}
}
