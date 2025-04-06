using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using FestivalMuzica.Client.ViewModels;

namespace FestivalMuzica.Client.Views
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
