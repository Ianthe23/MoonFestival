using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FestivalMuzica.Client.ViewModels;

namespace FestivalMuzica.Client.Views
{
    public partial class ShowsView : UserControl
    {
        public ShowsView()
        {
            InitializeComponent();
            if (Design.IsDesignMode)
                return;
        
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
} 