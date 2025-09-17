using NetworkService.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NetworkService.Views
{
    /// <summary>
    /// Interaction logic for NetworkEntitiesView.xaml
    /// </summary>
    public partial class NetworkEntitiesView : UserControl
    {
        public NetworkEntitiesViewModel _networkEntitiesViewModel;
        public NetworkEntitiesView(NetworkDisplayViewModel networkDisplayViewModel)
        {
            InitializeComponent();
            _networkEntitiesViewModel = new NetworkEntitiesViewModel(networkDisplayViewModel);
            DataContext = _networkEntitiesViewModel;
        }

        public NetworkEntitiesView() // parameterless constructor for designer
        {
            InitializeComponent();

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                return;

            _networkEntitiesViewModel = new NetworkEntitiesViewModel(null);
            DataContext = _networkEntitiesViewModel;
        }

        private void idTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }
    }
}
