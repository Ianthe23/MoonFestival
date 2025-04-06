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
using FestivalMuzica.Client.Service;
using FestivalMuzica.Common.Models;
using FestivalMuzica.Client.ViewModels;
using Avalonia.Input;
using System.Linq;
using System.Collections.ObjectModel;

namespace FestivalMuzica.Client.Views
{
    public partial class MainWindow : Window
    {
        private readonly ServiceFestival _serviceFestival;
        private readonly SignalRService _signalRService;
        private ComboBox _showsComboBox;
        
        public MainWindow(ServiceFestival serviceFestival, SignalRService signalRService, Employee loggedEmployee)
        {
            InitializeComponent();
            if (Design.IsDesignMode)
                return;
                
            _serviceFestival = serviceFestival;
            _signalRService = signalRService;
            DataContext = new MainWindowViewModel(serviceFestival, signalRService, loggedEmployee);

            // Set the window icon
            if (Application.Current is App app)
            {
                Icon = app.GetAppIcon();
            }
            
            // Set up the ComboBox
            SetupShowsComboBox();
            
            // Force refresh when the window is activated
            this.Activated += (sender, e) => {
                Console.WriteLine("MainWindow activated, forcing data refresh");
                if (DataContext is MainWindowViewModel viewModel)
                {
                    // Force refresh all data
                    var stateService = FestivalStateService.GetOrInitialize(_serviceFestival, _signalRService);
                    stateService.ForceRefreshAll();
                    
                    // Refresh current view
                    if (viewModel.CurrentView is ShowsView)
                    {
                        viewModel.ShowShowsView();
                    }
                    else if (viewModel.CurrentView is TicketsView)
                    {
                        viewModel.ShowTicketsView();
                    }
                    else if (viewModel.CurrentView is ClientsView)
                    {
                        viewModel.ShowClientsView();
                    }
                }
            };
            
            // Ensure window has proper focus and can be resized/maximized
            this.Opened += (sender, e) => {
                Console.WriteLine("MainWindow opened, ensuring proper window state");
                
                // Make sure window is visible and active
                this.WindowState = WindowState.Normal;
                this.Topmost = true;
                
                // Allow user to resize after a brief delay
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => {
                    this.Topmost = false;
                    this.CanResize = true;
                    
                    // Force activation/focus
                    this.Activate();
                    
                    // Set up a timer to keep refreshing the UI
                    var timer = new System.Timers.Timer(2000); // 2-second refresh
                    timer.Elapsed += (s, args) => {
                        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => {
                            // Force refresh of state service
                            var stateService = FestivalStateService.GetOrInitialize(_serviceFestival, _signalRService);
                            
                            // Update window content
                            if (!this.IsVisible) return;
                            
                            try {
                                Console.WriteLine("[WINDOW TIMER] Forcing UI refresh");
                                
                                // Refresh data
                                stateService.ForceRefreshAll();
                                
                                // Force redraw of current view
                                if (DataContext is MainWindowViewModel viewModel)
                                {
                                    // Just a way to force the UI to redraw without changing content
                                    var currentView = viewModel.CurrentView;
                                    viewModel.CurrentView = null;
                                    viewModel.CurrentView = currentView;
                                }
                            }
                            catch (Exception ex) {
                                Console.WriteLine($"[WINDOW TIMER] Error refreshing UI: {ex.Message}");
                            }
                        }, Avalonia.Threading.DispatcherPriority.Background);
                    };
                    timer.AutoReset = true;
                    timer.Start();
                }, Avalonia.Threading.DispatcherPriority.Background);
            };
        }
        
        private void SetupShowsComboBox()
        {
            _showsComboBox = this.FindControl<ComboBox>("ShowsComboBox");
            try
            {
                if (_showsComboBox != null)
                {
                    // Load shows directly from service
                    var shows = _serviceFestival.getShows();
                    _showsComboBox.Items.Clear();
                    
                    // Add shows to ComboBox directly
                    foreach (var show in shows)
                    {
                        _showsComboBox.Items.Add(show);
                    }
                    
                    // Set up direct selection handler
                    _showsComboBox.SelectionChanged += (sender, e) =>
                    {
                        if (_showsComboBox.SelectedItem is Show selectedShow && 
                            DataContext is MainWindowViewModel viewModel)
                        {
                            viewModel.SelectedShow = selectedShow;
                            Console.WriteLine($"Selected show: {selectedShow.Name}");
                        }
                        else
                        {
                            Console.WriteLine($"Selection changed but item type is: {_showsComboBox.SelectedItem?.GetType().Name ?? "null"}");
                        }
                    };
                    
                    // Set up SignalR to update items when needed
                    var stateService = FestivalStateService.GetOrInitialize(_serviceFestival, _signalRService);
                    stateService.PropertyChanged += (sender, e) =>
                    {
                        if (e.PropertyName == nameof(FestivalStateService.Shows))
                        {
                            // Use background priority to update even when window is not in focus
                            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                            {
                                try
                                {
                                    Console.WriteLine("[BACKGROUND UPDATE] Updating ComboBox items");
                                    // Get current selection to maintain it if possible
                                    var currentSelection = _showsComboBox.SelectedItem as Show;
                                    
                                    // Update items
                                    var updatedShows = stateService.Shows.ToList();
                                    _showsComboBox.Items.Clear();
                                    foreach (var show in updatedShows)
                                    {
                                        _showsComboBox.Items.Add(show);
                                    }
                                    
                                    // Restore selection if possible
                                    if (currentSelection != null)
                                    {
                                        var matchingShow = updatedShows.FirstOrDefault(s => s.Id == currentSelection.Id);
                                        if (matchingShow != null)
                                        {
                                            _showsComboBox.SelectedItem = matchingShow;
                                        }
                                    }
                                    Console.WriteLine("[BACKGROUND UPDATE] ComboBox update complete");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"[BACKGROUND UPDATE] Error updating ComboBox: {ex.Message}");
                                }
                            }, Avalonia.Threading.DispatcherPriority.Background);
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting up ComboBox: {ex.Message}");
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow(_serviceFestival);
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
                    Show selectedShow = null;
                    
                    // Get the selected show directly from the ComboBox
                    if (_showsComboBox.SelectedItem is Show show)
                    {
                        selectedShow = show;
                    }
                    
                    if (string.IsNullOrEmpty(viewModel.ClientName) || string.IsNullOrEmpty(viewModel.NumberOfSeats))
                    {
                        await MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard("Error", "Please enter both client name and number of seats").ShowAsync();
                        return;
                    }

                    if (selectedShow == null)
                    {
                        await MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard("Error", "Please select a show").ShowAsync();
                        return;
                    }

                    Console.WriteLine($"Selling ticket for show: {selectedShow.Name}, Client: {viewModel.ClientName}, Seats: {viewModel.NumberOfSeats}");
                    _serviceFestival.sellTicket(selectedShow, viewModel.ClientName, viewModel.NumberOfSeats);
                    Console.WriteLine("Ticket sold successfully in database");
                    
                    // Instead of restarting SignalR, notify all views to update
                    try
                    {
                        // First, force refresh all data to make sure we have latest state
                        var stateService = FestivalStateService.GetOrInitialize(_serviceFestival, _signalRService);
                        Console.WriteLine("Forcing immediate UI update across all views");
                        stateService.ForceRefreshAll();
                        
                        // Explicitly notify all connected clients that data has changed
                        stateService.NotifyAllDataChanged();
                    }
                    catch (Exception refreshEx)
                    {
                        Console.WriteLine($"Error refreshing data: {refreshEx.Message}");
                    }
                    
                    // Clear inputs after successful sale
                    viewModel.ClientName = string.Empty;
                    viewModel.NumberOfSeats = string.Empty;
                    viewModel.SelectedShow = null;
                    _showsComboBox.SelectedItem = null;
                    
                    // Explicitly reload the current view to reflect changes
                    if (viewModel.CurrentView is FestivalMuzica.Client.Views.ShowsView)
                    {
                        Console.WriteLine("Reloading Shows view");
                        viewModel.ShowShowsView();
                    }
                    
                    await MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard("Success", "Ticket sold successfully").ShowAsync();
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error selling ticket: {ex.Message}");
                await MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard("Error", $"An error occurred: {ex.Message}").ShowAsync();
            }
        }
    }
}
