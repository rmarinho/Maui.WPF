using System;

namespace Maui.Controls.Sample.WPF;

public partial class MainPage
{
	int _count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	void OnCounterClicked(object? sender, EventArgs e)
	{
		_count++;
		CounterBtn.Text = $"Click me - Count: {_count}";
	}
}
