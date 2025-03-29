using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ReactiveUI;

namespace festival_muzica_avalonia.ViewModels
{
    public abstract class PaginatedViewModel<T> : ReactiveObject
    {
        private int _currentPage = 1;
        private int _itemsPerPage = 5;
        private int _totalPages;
        private int _totalItems;
        private ObservableCollection<T> _items = new();
        private ObservableCollection<T> _paginatedItems = new();
        private RelayCommand? _nextPageCommand;
        private RelayCommand? _previousPageCommand;

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    this.RaisePropertyChanged();
                    UpdatePaginatedItems();
                    this.RaisePropertyChanged(nameof(CanGoNext));
                    this.RaisePropertyChanged(nameof(CanGoPrevious));
                    _nextPageCommand?.RaiseCanExecuteChanged();
                    _previousPageCommand?.RaiseCanExecuteChanged();
                }
            }
        }

        public int ItemsPerPage
        {
            get => _itemsPerPage;
            set
            {
                if (_itemsPerPage != value)
                {
                    _itemsPerPage = value;
                    this.RaisePropertyChanged();
                    UpdateTotalPages();
                    UpdatePaginatedItems();
                }
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            set
            {
                if (_totalPages != value)
                {
                    _totalPages = value;
                    this.RaisePropertyChanged();
                    this.RaisePropertyChanged(nameof(CanGoNext));
                    this.RaisePropertyChanged(nameof(CanGoPrevious));
                }
            }
        }

        public ObservableCollection<T> PaginatedItems
        {
            get => _paginatedItems;
            set
            {
                if (_paginatedItems != value)
                {
                    _paginatedItems = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        protected ObservableCollection<T> Items
        {
            get => _items;
            set
            {
                if (_items != value)
                {
                    _items = value;
                    this.RaisePropertyChanged();
                    TotalItems = _items.Count;
                    UpdateTotalPages();
                    UpdatePaginatedItems();
                }
            }
        }

        public int TotalItems
        {
            get => _totalItems;
            set 
            {
                if (_totalItems != value)
                {
                    _totalItems = value;
                    this.RaisePropertyChanged();
                    UpdateTotalPages();
                }
            }
        }

        public bool CanGoNext => CurrentPage < TotalPages;
        public bool CanGoPrevious => CurrentPage > 1;

        public ICommand NextPageCommand => _nextPageCommand ??= new RelayCommand(
            execute: () => NextPage(),
            canExecute: () => CanGoNext
        );

        public ICommand PreviousPageCommand => _previousPageCommand ??= new RelayCommand(
            execute: () => PreviousPage(),
            canExecute: () => CanGoPrevious
        );

        private void NextPage()
        {
            if (CanGoNext)
            {
                CurrentPage++;
            }
        }

        private void PreviousPage()
        {
            if (CanGoPrevious)
            {
                CurrentPage--;
            }
        }

        private void UpdateTotalPages()
        {
            TotalPages = (int)Math.Ceiling(Items.Count / (double)ItemsPerPage);
        }

        private void UpdatePaginatedItems()
        {
            var items = Items.Skip((CurrentPage - 1) * ItemsPerPage)
                           .Take(ItemsPerPage)
                           .ToList();
            PaginatedItems = new ObservableCollection<T>(items);
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object? parameter) => _execute();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
} 