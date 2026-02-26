using System;
using Microsoft.Maui.Controls;
using Maui.Controls.Sample.WPF;

namespace Maui.Controls.Sample.WPF.Views;

public partial class StepperDemoPage : ContentPage
{
	public StepperDemoPage()
	{
		InitializeComponent();
	}

	void OnBackClicked(object sender, EventArgs e)
	{
		if (Application.Current?.Windows.Count > 0)
			Application.Current.Windows[0].Page = new MainPage();
	}

	void OnIntStepperChanged(object sender, ValueChangedEventArgs e)
	{
		IntStepperLabel.Text = $"Value: {e.NewValue}";
	}

	void OnHalfStepperChanged(object sender, ValueChangedEventArgs e)
	{
		HalfStepperLabel.Text = $"Value: {e.NewValue:F1}";
	}
}
