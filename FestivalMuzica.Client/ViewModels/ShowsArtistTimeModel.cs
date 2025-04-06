using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FestivalMuzica.Common.Models;
using FestivalMuzica.Client.Service;
using MsBox.Avalonia;
using Avalonia.Controls;
using System.Linq;
using System.Collections.Generic;

namespace FestivalMuzica.Client.ViewModels
{
    public class ShowsArtistTimeModel : BaseViewModel
    {
        private readonly ServiceFestival _service;
        private ObservableCollection<Show> _shows;
        private string _artist;
        private string _time;

        public string Artist
        {
            get => _artist;
            set
            {
                if (_artist != value)
                {
                    _artist = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Time
        {
            get => _time;
            set
            {
                if (_time != value)
                {
                    _time = value;
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

        public ShowsArtistTimeModel(ServiceFestival service, string artist, string time)
        {
            if (!Design.IsDesignMode)
            {
                _service = service;
                Artist = artist;
                Time = time;
                LoadShows();
            }
        }

        public void LoadShows()
        {
            try
            {
                var shows = _service.getShowsByArtistAndTime(Artist, Time);
                Shows = new ObservableCollection<Show>(shows);
            }
            catch (ServiceException ex)
            {
                Console.WriteLine($"Error loading shows: {ex.Message}");
                Shows = new ObservableCollection<Show>();
                MessageBoxManager.GetMessageBoxStandard("Error", ex.Message).ShowAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading shows: {ex.Message}");
                Shows = new ObservableCollection<Show>();
                MessageBoxManager.GetMessageBoxStandard("Error", "An error occurred while loading shows").ShowAsync();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 