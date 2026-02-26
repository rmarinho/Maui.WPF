using WSlider = System.Windows.Controls.Slider;

namespace Microsoft.Maui.Handlers.WPF
{
	public partial class SliderHandler : WPFViewHandler<ISlider, WSlider>
	{
		protected override WSlider CreatePlatformView()
		{
			return new WSlider
			{
				Minimum = 0,
				Maximum = 1,
				MinWidth = 100,
			};
		}

		protected override void ConnectHandler(WSlider platformView)
		{
			base.ConnectHandler(platformView);
			platformView.ValueChanged += OnValueChanged;
		}

		protected override void DisconnectHandler(WSlider platformView)
		{
			platformView.ValueChanged -= OnValueChanged;
			base.DisconnectHandler(platformView);
		}

		void OnValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
		{
			VirtualView?.DragStarted();
			VirtualView?.DragCompleted();
		}

		public static void MapValue(SliderHandler handler, ISlider slider)
		{
			if (handler.PlatformView.Value != slider.Value)
				handler.PlatformView.Value = slider.Value;
		}

		public static void MapMinimum(SliderHandler handler, ISlider slider)
		{
			handler.PlatformView.Minimum = slider.Minimum;
		}

		public static void MapMaximum(SliderHandler handler, ISlider slider)
		{
			handler.PlatformView.Maximum = slider.Maximum;
		}

		public static void MapMinimumTrackColor(SliderHandler handler, ISlider slider)
		{
		}

		public static void MapMaximumTrackColor(SliderHandler handler, ISlider slider)
		{
		}

		public static void MapThumbColor(SliderHandler handler, ISlider slider)
		{
		}
	}
}
