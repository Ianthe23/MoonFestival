using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using System;
using System.Collections.Generic;
using Avalonia.Interactivity;
using festival_muzica_avalonia.Service;
using festival_muzica_avalonia.Models;
using festival_muzica_avalonia.ViewModels;
using Avalonia.Input;

namespace festival_muzica_avalonia
{
    public partial class MainWindow : Window
    {
        private readonly ServiceFestival serviceFestival;
        public MainWindow(ServiceFestival serviceFestival, Employee loggedEmployee)
        {
            InitializeComponent();
            this.serviceFestival = serviceFestival;
            DataContext = new MainWindowViewModel(serviceFestival, loggedEmployee);
            
            // Set the window icon
            if (Application.Current is App app)
            {
                Icon = app.GetAppIcon();
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow(serviceFestival);
            loginWindow.Show();
            Close();
        }

        private void ShowsTextBlock_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                // Handle Shows navigation
                // TODO: Implement shows view
            }
        }

        private void TicketsTextBlock_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                // Handle Tickets navigation
                // TODO: Implement tickets view
            }
        }

        private void ReportsTextBlock_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                // Handle Reports navigation
                // TODO: Implement reports view
            }
        }
    }
}
