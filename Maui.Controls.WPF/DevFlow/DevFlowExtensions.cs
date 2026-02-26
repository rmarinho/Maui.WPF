using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;
using MauiDevFlow.Agent.Core;

namespace Maui.Controls.WPF.DevFlow
{
	public static class DevFlowExtensions
	{
		/// <summary>
		/// Adds the MauiDevFlow Agent to the WPF MAUI app builder.
		/// The agent starts automatically when the first window activates.
		/// </summary>
		public static MauiAppBuilder AddMauiDevFlowAgent(this MauiAppBuilder builder, Action<AgentOptions>? configure = null)
		{
			var options = new AgentOptions();
			configure?.Invoke(options);

			// Try broker for port assignment
			BrokerRegistration? brokerReg = null;
			if (options.Port == AgentOptions.DefaultPort)
			{
				try
				{
					var platform = "WPF";
					var appName = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name ?? "unknown";
					var project = appName;
					var tfm = "net10.0-windows";
					brokerReg = new BrokerRegistration(project, tfm, platform, appName);
					var assignedPort = Task.Run(() => brokerReg.TryRegisterAsync(TimeSpan.FromSeconds(5))).GetAwaiter().GetResult();
					if (assignedPort.HasValue)
					{
						options.Port = assignedPort.Value;
						Console.WriteLine($"[MauiDevFlow] Broker assigned port {assignedPort.Value}");
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"[MauiDevFlow] Broker registration failed: {ex.Message}");
					brokerReg?.Dispose();
					brokerReg = null;
				}
			}

			var service = new WpfDevFlowAgentService(options);
			if (brokerReg != null)
			{
				brokerReg.CurrentPort = options.Port;
				service.SetBrokerRegistration(brokerReg);
			}
			builder.Services.AddSingleton<DevFlowAgentService>(service);

			// Start agent when MAUI app is ready (poll for Application.Current)
			Task.Run(async () =>
			{
				for (int i = 0; i < 60; i++)
				{
					await Task.Delay(500);
					var app = Application.Current;
					if (app != null)
					{
						app.Dispatcher.Dispatch(() => service.Start(app, app.Dispatcher));
						Console.WriteLine($"[MauiDevFlow] WPF Agent started on port {options.Port}");
						return;
					}
				}
				Console.WriteLine("[MauiDevFlow] Failed to start agent: Application.Current was null after 60 retries");
			});

			return builder;
		}
	}
}
