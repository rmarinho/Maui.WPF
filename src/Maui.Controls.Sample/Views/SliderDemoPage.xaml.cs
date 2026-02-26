using System;
using Microsoft.Maui.Controls;
using Maui.Controls.Sample.WPF;

namespace Maui.Controls.Sample.WPF.Views;

public partial class SliderDemoPage : ContentPage
{
	public SliderDemoPage()
	{
		InitializeComponent();
	}

	void OnBackClicked(object sender, EventArgs e)
	{
		if (Application.Current?.Windows.Count > 0)
			Application.Current.Windows[0].Page = new MainPage();
	}

	void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
	{
		SliderValueLabel.Text = $"Value: {e.NewValue:F1}";
	}
}
