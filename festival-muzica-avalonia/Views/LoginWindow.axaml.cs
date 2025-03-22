using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System.Collections.Generic;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using festival_muzica_avalonia.Service;
using festival_muzica_avalonia.Models;
using System;

namespace festival_muzica_avalonia
{   
    public partial class LoginWindow : Window
    {
        private readonly ServiceFestival serviceFestival;
        public LoginWindow(ServiceFestival serviceFestival)
        {
            InitializeComponent();
            var props = new Dictionary<string, string>
            {
                ["ConnectionString"] = "Data Source=db.config;Version=3;"
            };
            this.serviceFestival = serviceFestival;
            
            // Set the window icon
            if (Application.Current is App app)
            {
                Icon = app.GetAppIcon();
            }
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string? username = UsernameTextBox.Text;
            string? password = PasswordTextBox.Text;
            
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard("Error", "Please enter both username and password").ShowAsync();
                return;
            }

            try
            {
                Employee? employee = serviceFestival.loginEmployee(username, password);
                if (employee != null)
                {
                    MainWindow mainWindow = new MainWindow(serviceFestival, employee);
                    mainWindow.Show();
                    Close();
                }
            } catch (Exception ex)
            {
                await MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard("Error", ex.Message).ShowAsync();
            }
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string? username = UsernameTextBox.Text;
            string? password = PasswordTextBox.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard("Error", "Please enter both username and password").ShowAsync();
            }

            try
            {
                Employee? employee = serviceFestival.registerEmployee(username, password);
                if (employee != null)
                {
                    await MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard("Success", "Registration successful").ShowAsync();
                }
            }
            catch (Exception ex)
            {
                await MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard("Error", ex.Message).ShowAsync();
            }
        }
    }
} 