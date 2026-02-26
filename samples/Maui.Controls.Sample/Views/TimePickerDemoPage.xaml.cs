using System;
using Microsoft.Maui.Controls;
using Maui.Controls.Sample.WPF;

namespace Maui.Controls.Sample.WPF.Views;

public partial class TimePickerDemoPage : ContentPage
{
	public TimePickerDemoPage()
	{
		InitializeComponent();
	}

	void OnBackClicked(object sender, EventArgs e)
	{
		if (Application.Current?.Windows.Count > 0)
			Application.Current.Windows[0].Page = new MainPage();
	}

	void OnTimePickerPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(TimePicker.Time))
		{
			SelectedTimeLabel.Text = $"Selected: {DefaultTimePicker.Time:hh\\:mm\\:ss}";
		}
	}
}
