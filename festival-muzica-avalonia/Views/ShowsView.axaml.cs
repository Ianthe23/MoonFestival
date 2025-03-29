using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using festival_muzica_avalonia.ViewModels;

namespace festival_muzica_avalonia.Views
{
    public partial class ShowsView : UserControl
    {
        public ShowsView()
        {
            InitializeComponent();
            if (Design.IsDesignMode)
                return;
        
        }
    }
} 