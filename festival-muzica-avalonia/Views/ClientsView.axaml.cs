using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace festival_muzica_avalonia.Views
{
    public partial class ClientsView : UserControl
    {
        public ClientsView()
        {
            InitializeComponent();
            if (Design.IsDesignMode)
                return;
        }
    }
}
