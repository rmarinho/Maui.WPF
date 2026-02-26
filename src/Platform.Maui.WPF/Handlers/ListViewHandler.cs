using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Maui.Handlers;
using WListBox = System.Windows.Controls.ListBox;

namespace Microsoft.Maui.Handlers.WPF
{
	public partial class ListViewHandler : WPFViewHandler<Microsoft.Maui.Controls.ListView, WListBox>
	{
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

		protected override WListBox CreatePlatformView()
		{
			var lb = new WListBox
			{
				HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch,
				BorderThickness = new System.Windows.Thickness(0),
			};
			lb.SelectionChanged += OnSelectionChanged;
			return lb;
		}

		void OnSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (VirtualView != null && PlatformView != null)
				VirtualView.SelectedItem = PlatformView.SelectedItem;
		}

		static void MapItemsSource(ListViewHandler handler, Microsoft.Maui.Controls.ListView view)
		{
			if (handler.PlatformView == null) return;
			handler.PlatformView.ItemsSource = view.ItemsSource as IEnumerable;

			if (view.ItemTemplate != null)
			{
				handler.PlatformView.ItemTemplate = null;
				handler.PlatformView.ItemTemplateSelector = new MauiListViewTemplateSelector(view.ItemTemplate, view);
			}
		}

		static void MapSelectedItem(ListViewHandler handler, Microsoft.Maui.Controls.ListView view)
		{
			if (handler.PlatformView?.SelectedItem != view.SelectedItem)
				handler.PlatformView!.SelectedItem = view.SelectedItem;
		}

		static void MapHeader(ListViewHandler handler, Microsoft.Maui.Controls.ListView view) { }
		static void MapFooter(ListViewHandler handler, Microsoft.Maui.Controls.ListView view) { }

		static void MapSeparatorColor(ListViewHandler handler, Microsoft.Maui.Controls.ListView view)
		{
			// WPF ListBox items don't natively have separators; could be added via ItemContainerStyle
		}

		protected override void DisconnectHandler(WListBox platformView)
		{
			platformView.SelectionChanged -= OnSelectionChanged;
			base.DisconnectHandler(platformView);
		}

		class MauiListViewTemplateSelector : System.Windows.Controls.DataTemplateSelector
		{
			readonly Microsoft.Maui.Controls.DataTemplate _mauiTemplate;
			readonly Microsoft.Maui.Controls.ListView _listView;

			public MauiListViewTemplateSelector(Microsoft.Maui.Controls.DataTemplate template, Microsoft.Maui.Controls.ListView lv)
			{
				_mauiTemplate = template;
				_listView = lv;
			}

			public override System.Windows.DataTemplate SelectTemplate(object item, DependencyObject container)
			{
				var factory = new FrameworkElementFactory(typeof(MauiContentPresenter));
				factory.SetValue(MauiContentPresenter.MauiTemplateProperty, _mauiTemplate);
				factory.SetValue(MauiContentPresenter.MauiContextSourceProperty, (object?)null);
				return new System.Windows.DataTemplate { VisualTree = factory };
			}
		}
	}
}
