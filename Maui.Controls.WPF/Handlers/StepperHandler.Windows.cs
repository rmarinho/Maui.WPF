using System.Windows;
using WStackPanel = System.Windows.Controls.StackPanel;
using WButton = System.Windows.Controls.Button;
using WOrientation = System.Windows.Controls.Orientation;

namespace Microsoft.Maui.Handlers.WPF
{
	public partial class StepperHandler : WPFViewHandler<IStepper, WStackPanel>
	{
		WButton _minusButton = null!;
		WButton _plusButton = null!;

		protected override WStackPanel CreatePlatformView()
		{
			var panel = new WStackPanel { Orientation = WOrientation.Horizontal };
			_minusButton = new WButton { Content = "−", Width = 40, Height = 30 };
			_plusButton = new WButton { Content = "+", Width = 40, Height = 30 };
			panel.Children.Add(_minusButton);
			panel.Children.Add(_plusButton);
			return panel;
		}

		protected override void ConnectHandler(WStackPanel platformView)
		{
			base.ConnectHandler(platformView);
			_minusButton.Click += OnMinusClicked;
			_plusButton.Click += OnPlusClicked;
		}

		protected override void DisconnectHandler(WStackPanel platformView)
		{
			_minusButton.Click -= OnMinusClicked;
			_plusButton.Click -= OnPlusClicked;
			base.DisconnectHandler(platformView);
		}

		void OnMinusClicked(object sender, System.Windows.RoutedEventArgs e)
		{
			if (VirtualView == null) return;
			var increment = 1.0;
			var newValue = VirtualView.Value - increment;
			if (newValue >= VirtualView.Minimum)
				VirtualView.Value = newValue;
		}

		void OnPlusClicked(object sender, System.Windows.RoutedEventArgs e)
		{
			if (VirtualView == null) return;
			var increment = 1.0;
			var newValue = VirtualView.Value + increment;
			if (newValue <= VirtualView.Maximum)
				VirtualView.Value = newValue;
		}

		public static void MapValue(StepperHandler handler, IStepper stepper)
		{
		}

		public static void MapMinimum(StepperHandler handler, IStepper stepper)
		{
		}

		public static void MapMaximum(StepperHandler handler, IStepper stepper)
		{
		}
	}
}
