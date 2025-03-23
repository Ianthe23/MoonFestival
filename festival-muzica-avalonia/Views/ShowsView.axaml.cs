using Avalonia.Controls;
using Avalonia.Markup.Xaml;


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