using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FestivalMuzica.Client.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using Material.Styles.Themes;
using Avalonia.Platform;
using System.IO;
using Avalonia.Media.Imaging;
using Avalonia.Controls;
using FestivalMuzica.Client.Views;
using Microsoft.Extensions.DependencyInjection;

namespace FestivalMuzica.Client;

public partial class App : Application
{
    private WindowIcon? _appIcon;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        
        // Try to set the application icon
        try 
        {
            using var stream = File.OpenRead("Assets/ticket.png");
            var bitmap = new Bitmap(stream);
            _appIcon = new WindowIcon(bitmap);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading app icon: {ex.Message}");
        }
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            try
            {
                // Get services from DI container
                var serviceFestival = Program.ServiceProvider.GetRequiredService<ServiceFestival>();
                
                // Create login window with the service
                desktop.MainWindow = new LoginWindow(serviceFestival);
                
                // Set the icon for the main window
                if (_appIcon != null)
                {
                    desktop.MainWindow.Icon = _appIcon;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing application: {ex.Message}");
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    public WindowIcon? GetAppIcon()
    {
        return _appIcon;
    }
}