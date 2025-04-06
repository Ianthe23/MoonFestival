using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FestivalMuzica.Client.Service;
using FestivalMuzica.Common.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FestivalMuzica.Client.Views
{
    public partial class LoginWindow : Window
    {
        private readonly ServiceFestival _serviceFestival;
        
        public LoginWindow(ServiceFestival serviceFestival)
        {
            InitializeComponent();
            if (Design.IsDesignMode)
                return;
                
            _serviceFestival = serviceFestival;

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

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var usernameTextBox = this.FindControl<TextBox>("UsernameTextBox");
            var passwordTextBox = this.FindControl<TextBox>("PasswordTextBox");

            string username = usernameTextBox.Text;
            string password = passwordTextBox.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                var messageBox = MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard("Error", "Username and password are required");
                await messageBox.ShowAsync();
                return;
            }

            try
            {
                var employee = _serviceFestival.loginEmployee(username, password);
                if (employee != null)
                {
                    // Get SignalRService from the service provider
                    var signalRService = Program.ServiceProvider.GetRequiredService<SignalRService>();
                    
                    // Pass both services to MainWindow and ensure it's properly configured
                    var mainWindow = new MainWindow(_serviceFestival, signalRService, employee);
                    
                    // Ensure window displays correctly
                    mainWindow.WindowState = WindowState.Normal;
                    mainWindow.CanResize = true;
                    mainWindow.ShowInTaskbar = true;
                    
                    // Show main window first before closing the login window
                    mainWindow.Show();
                    mainWindow.Activate(); // Force window activation
                    
                    // Close this window after showing main window
                    Close();
                }
                else
                {
                    var messageBox = MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard("Error", "Invalid username or password");
                    await messageBox.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                var messageBox = MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard("Authentication Error", ex.Message);
                await messageBox.ShowAsync();
            }
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton_Click(sender, new RoutedEventArgs());
            }
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var usernameTextBox = this.FindControl<TextBox>("UsernameTextBox");
            var passwordTextBox = this.FindControl<TextBox>("PasswordTextBox");

            string username = usernameTextBox.Text;
            string password = passwordTextBox.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                var messageBox = MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard("Error", "Username and password are required");
                await messageBox.ShowAsync();
                return;
            }

            try
            {
                var employee = _serviceFestival.registerEmployee(username, password);
                if (employee != null)
                {
                    var messageBox = MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard("Success", "Registration successful");
                    await messageBox.ShowAsync();
                }
                else
                {
                    var messageBox = MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard("Error", "Registration failed. User may already exist.");
                    await messageBox.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                var messageBox = MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard("Error", ex.Message);
                await messageBox.ShowAsync();
            }
        }
    }
} 