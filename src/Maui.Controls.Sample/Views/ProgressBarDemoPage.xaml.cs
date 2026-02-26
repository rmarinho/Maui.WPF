using System;
using Microsoft.Maui.Controls;
using Maui.Controls.Sample.WPF;

namespace Maui.Controls.Sample.WPF.Views;

public partial class ProgressBarDemoPage : ContentPage
{
	public ProgressBarDemoPage()
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
		DynamicProgressBar.Progress = e.NewValue;
		ProgressLabel.Text = $"Progress: {e.NewValue:F2}";
	}
}
