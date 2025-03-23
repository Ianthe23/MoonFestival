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
            if (Design.IsDesignMode)
                return;
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
                if (DataContext is MainWindowViewModel viewModel)
                {
                    viewModel.ShowShowsView();
                }
            }
        }

        private void TicketsTextBlock_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                if (DataContext is MainWindowViewModel viewModel)
                {
                    viewModel.ShowTicketsView();
                }
            }
        }

        private void ReportsTextBlock_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                if (DataContext is MainWindowViewModel viewModel)
                {
                    viewModel.ShowClientsView();
                }
            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DataContext is MainWindowViewModel viewModel)
                {
                    if (string.IsNullOrEmpty(viewModel.Artist) || string.IsNullOrEmpty(viewModel.Time))
                    {
                        await MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard("Error", "Please enter both artist and time").ShowAsync();
                        return;
                    }

                    viewModel.ShowShowsByArtistTimeView(viewModel.Artist, viewModel.Time);
                }
            }
            catch (Exception ex)
            {
                await MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard("Error", $"An error occurred: {ex.Message}").ShowAsync();
            }
        }
        
        private async void SellButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DataContext is MainWindowViewModel viewModel)
                {
                    if (string.IsNullOrEmpty(viewModel.ClientName) || string.IsNullOrEmpty(viewModel.NumberOfSeats))
                    {
                        await MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard("Error", "Please enter both client name and number of seats").ShowAsync();
                        return;
                    }

                    if (viewModel.SelectedShow == null)
                    {
                        await MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard("Error", "Please select a show").ShowAsync();
                        return;
                    }

                    serviceFestival.sellTicket(viewModel.SelectedShow, viewModel.ClientName, viewModel.NumberOfSeats);
                    viewModel.LoadShows();
                    viewModel.LoadTickets();
                    viewModel.LoadClients();
                    viewModel.ShowShowsView();
                    viewModel.ClientName = string.Empty;
                    viewModel.NumberOfSeats = string.Empty;
                    viewModel.SelectedShow = null;
                    
                    await MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard("Success", "Ticket sold successfully").ShowAsync();
                }                
            }
            catch (Exception ex)
            {
                await MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard("Error", $"An error occurred: {ex.Message}").ShowAsync();
            }
        }
    }
}
