using Avalonia;
using System;
using log4net;
using System.IO;
using System.Reflection;
using log4net.Config;

namespace festival_muzica_avalonia;

class Program
{
    private static readonly ILog log = LogManager.GetLogger(typeof(Program));
    
    [STAThread]
    public static void Main(string[] args)
    {
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

            // Start Avalonia UI
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Application error: {ex}");
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
