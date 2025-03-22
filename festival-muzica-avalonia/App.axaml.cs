using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using festival_muzica_avalonia.Service;
using festival_muzica_avalonia.Repository;
using festival_muzica_avalonia.Models;
using festival_muzica_avalonia.Sql;
using System;
using System.Collections.Generic;
using System.Configuration;
using Material.Styles.Themes;
using Avalonia.Platform;
using System.IO;
using Avalonia.Media.Imaging;
using Avalonia.Controls;

namespace festival_muzica_avalonia;

public partial class App : Application
{
    private WindowIcon? _appIcon;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        
        // Set the application icon
        using var stream = new FileStream("Assets/ticket.png", FileMode.Open);
        var bitmap = new Bitmap(stream);
        _appIcon = new WindowIcon(bitmap);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            IDictionary<string, string> props = new Dictionary<string, string>();
            string? connectionString = GetConnectionStringByName("festival-csharp");
            if (connectionString == null)
            {
                throw new InvalidOperationException("Connection string not found in configuration");
            }
            props.Add("ConnectionString", connectionString);

            IClientRepo clientDataBaseRepo = new ClientDatabaseRepo(props);
            IEmployeeRepo employeeDataBaseRepo = new EmployeeDatabaseRepo(props);
            IShowRepo showDataBaseRepo = new ShowDatabaseRepo(props);
            ITicketRepo ticketDataBaseRepo = new TicketDatabaseRepo(props);

            ServiceFestival serviceFestival = new ServiceFestival(props, clientDataBaseRepo, employeeDataBaseRepo, showDataBaseRepo, ticketDataBaseRepo);
            
            desktop.MainWindow = new LoginWindow(serviceFestival);
            
            // Set the icon for the main window
            if (_appIcon != null)
            {
                desktop.MainWindow.Icon = _appIcon;
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    public WindowIcon? GetAppIcon()
    {
        return _appIcon;
    }

    private static string? GetConnectionStringByName(string name)
    {
        string? returnValue = null;
        ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[name];

        if (settings != null)
        {
            returnValue = settings.ConnectionString;
        }

        return returnValue;
    }
}