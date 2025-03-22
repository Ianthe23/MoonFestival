using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using festival_muzica_avalonia.Models;
using festival_muzica_avalonia.Service;

namespace festival_muzica_avalonia.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly ServiceFestival _service;
        private Employee? _loggedEmployee;
        private ObservableCollection<Show> _shows;

        public Employee? LoggedEmployee
        {
            get => _loggedEmployee;
            set
            {
                if (_loggedEmployee != value)
                {
                    _loggedEmployee = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Show> Shows
        {
            get => _shows;
            set
            {
                if (_shows != value)
                {
                    _shows = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainWindowViewModel(ServiceFestival service, Employee loggedEmployee)
        {
            _service = service;
            LoggedEmployee = loggedEmployee;
            LoadShows();
        }

        public void LoadShows()
        {
            var shows = _service.getShows();
            Shows = new ObservableCollection<Show>(shows);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}