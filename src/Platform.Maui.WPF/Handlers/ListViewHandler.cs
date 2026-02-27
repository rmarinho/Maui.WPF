using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Maui.Handlers;
using WListBox = System.Windows.Controls.ListBox;

namespace Microsoft.Maui.Handlers.WPF
{
	public partial class ListViewHandler : WPFViewHandler<Microsoft.Maui.Controls.ListView, FrameworkElement>
	{
		MauiListViewListBox? _listBox;

		public static readonly PropertyMapper<Microsoft.Maui.Controls.ListView, ListViewHandler> Mapper =
			new(ViewMapper)
			{
				[nameof(Microsoft.Maui.Controls.ListView.ItemsSource)] = MapItemsSource,
				[nameof(Microsoft.Maui.Controls.ListView.SelectedItem)] = MapSelectedItem,
				[nameof(Microsoft.Maui.Controls.ListView.Header)] = MapHeader,
				[nameof(Microsoft.Maui.Controls.ListView.Footer)] = MapFooter,
				[nameof(Microsoft.Maui.Controls.ListView.SeparatorColor)] = MapSeparatorColor,
			};

		public ListViewHandler() : base(Mapper) { }

		protected override FrameworkElement CreatePlatformView()
		{
			_listBox = new MauiListViewListBox
			{
				HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch,
				BorderThickness = new System.Windows.Thickness(0),
			};
			_listBox.SelectionChanged += OnSelectionChanged;
			return _listBox;
		}

		void OnSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (VirtualView != null && _listBox != null)
				VirtualView.SelectedItem = _listBox.SelectedItem;
		}

		static void MapItemsSource(ListViewHandler handler, Microsoft.Maui.Controls.ListView view)
		{
			if (handler._listBox == null) return;
			handler._listBox.MauiTemplate = view.ItemTemplate;
			handler._listBox.MauiListView = view;
			handler._listBox.ItemsSource = view.ItemsSource as IEnumerable;
		}

		static void MapSelectedItem(ListViewHandler handler, Microsoft.Maui.Controls.ListView view)
		{
			if (handler._listBox?.SelectedItem != view.SelectedItem)
				handler._listBox!.SelectedItem = view.SelectedItem;
		}

		static void MapHeader(ListViewHandler handler, Microsoft.Maui.Controls.ListView view) { }
		static void MapFooter(ListViewHandler handler, Microsoft.Maui.Controls.ListView view) { }

		static void MapSeparatorColor(ListViewHandler handler, Microsoft.Maui.Controls.ListView view) { }

		protected override void DisconnectHandler(FrameworkElement platformView)
		{
			if (_listBox != null)
				_listBox.SelectionChanged -= OnSelectionChanged;
			base.DisconnectHandler(platformView);
		}
	}

	public class MauiListViewListBox : WListBox
	{
		internal Microsoft.Maui.Controls.DataTemplate? MauiTemplate { get; set; }
		internal Microsoft.Maui.Controls.ListView? MauiListView { get; set; }

		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);

			if (element is ListBoxItem lbi && MauiTemplate != null && MauiListView != null)
			{
				lbi.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
				lbi.Padding = new System.Windows.Thickness(0);

				try
				{
					var resolvedTemplate = MauiTemplate;
					if (MauiTemplate is Microsoft.Maui.Controls.DataTemplateSelector selector)
						resolvedTemplate = selector.SelectTemplate(item, MauiListView);

					if (resolvedTemplate == null) return;

					var content = resolvedTemplate.CreateContent() as View;
					if (content == null) return;

					content.BindingContext = item;

					var mauiContext = MauiListView.Handler?.MauiContext;
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
					System.Diagnostics.Debug.WriteLine($"[ListView] Template error: {ex.Message}");
				}
			}
		}
	}
}
