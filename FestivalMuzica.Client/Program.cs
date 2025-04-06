using Avalonia;
using Avalonia.ReactiveUI;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using FestivalMuzica.Networking.Connection;
using FestivalMuzica.Client.Service;
using log4net;
using System.IO;
using System.Reflection;
using log4net.Config;
using FestivalMuzica.Common.Service;
using System.Threading;
using Avalonia.Threading;
namespace FestivalMuzica.Client;

internal class Program
{
    private static readonly ILog log = LogManager.GetLogger(typeof(Program));
    
    // Expose the ServiceProvider for use in App.axaml.cs
    public static IServiceProvider ServiceProvider { get; private set; }
    
    [STAThread]
    public static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var services = new ServiceCollection();

        // Configure NetworkClient
        var networkConfig = configuration.GetSection("NetworkConfig");
        var host = networkConfig["ServerHost"] ?? "localhost";
        var port = int.Parse(networkConfig["ServerPort"] ?? "55555");
        var signalRPort = int.Parse(networkConfig["SignalRPort"] ?? "5000");

        // Register services
        services.AddSingleton<NetworkClient>(_ => new NetworkClient(host, port));
        services.AddSingleton<FestivalMuzica.Client.Service.IService<long>, ServiceFestival>();
        services.AddSingleton<ServiceFestival>(); // Also register as concrete type
        
        // Register SignalR service
        services.AddSingleton<SignalRService>(_ => {
            var connectionId = Guid.NewGuid().ToString("N"); // Generate unique ID for each client
            Console.WriteLine($"Creating SignalR service with unique client ID: {connectionId}");
            var service = new SignalRService(host, signalRPort, connectionId);
            return service;
        });

        ServiceProvider = services.BuildServiceProvider();

        try
        {
            // Clear existing logs
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string projectRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));
            string logsPath = Path.Combine(projectRoot, "logs");

            if (Directory.Exists(logsPath))
            {
                foreach (var file in Directory.GetFiles(logsPath, "*.log"))
                {
                    File.Delete(file);
                }
            }

            Directory.CreateDirectory(logsPath);
            GlobalContext.Properties["LogPath"] = logsPath;

            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            string configFilePath = Path.Combine(baseDir, "log4net.config");
            log.Info($"Loading log4net configuration from: {configFilePath}");
            
            if (!File.Exists(configFilePath)) {
                log.Error($"Configuration file not found at: {configFilePath}");
                return;
            }
            
            XmlConfigurator.Configure(logRepository, new FileInfo(configFilePath));
            log.Info("Successfully configured log4net");

            // Test logging
            log.Debug("Debug message - testing log4net configuration");
            log.Info("Info message - testing log4net configuration");
            log.Warn("Warning message - testing log4net configuration");
            log.Error("Error message - testing log4net configuration");
            log.Fatal("Fatal message - testing log4net configuration");

            log.Info("Starting the application...");

            // Start SignalR connection
            var signalRService = ServiceProvider.GetRequiredService<SignalRService>();
            var serviceFestival = ServiceProvider.GetRequiredService<ServiceFestival>();

            // Set up a background process to keep syncing data, even when app is not focused
            var backgroundThread = new Thread(() => {
                try {
                    log.Info("Starting background refresh thread");
                    var stateService = FestivalStateService.GetOrInitialize(serviceFestival, signalRService);
                    
                    // Start the SignalR connection
                    signalRService.StartConnectionAsync().GetAwaiter().GetResult();
                    log.Info($"SignalR service connected to {host}:{signalRPort}");
                    
                    // Forever loop to ensure refreshes happen
                    while (true) {
                        try {
                            // Every 3 seconds, check connection and refresh if needed
                            Thread.Sleep(3000);
                            
                            // Ensure SignalR is connected, reconnect if needed
                            if (!signalRService.IsConnected) {
                                log.Info("Background thread detected SignalR disconnection, reconnecting...");
                                signalRService.RestartConnectionAsync().GetAwaiter().GetResult();
                            }
                            
                            // Force data refresh from server
                            stateService.ForceRefreshAll();
                            log.Debug("Background refresh performed");
                            
                            // Force UI update via dispatcher
                            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => {
                                stateService.NotifyAllDataChanged();
                            }, Avalonia.Threading.DispatcherPriority.Background);
                        }
                        catch (Exception ex) {
                            log.Error($"Background refresh error: {ex.Message}");
                            // Small delay before retry
                            Thread.Sleep(1000);
                        }
                    }
                }
                catch (Exception ex) {
                    log.Fatal($"Fatal error in background thread: {ex.Message}");
                }
            });

            // Start it as a background thread so it doesn't prevent app exit
            backgroundThread.IsBackground = true;
            backgroundThread.Start();

            log.Info($"SignalR service configured to connect to {host}:{signalRPort}");

            // Note: FestivalStateService will be initialized in the MainWindowViewModel
            // when the main window is created after successful login

            // Start Avalonia UI
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Application error: {ex}");
            throw;
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<FestivalMuzica.Client.App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI()
            .WithInterFont();
}
