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
    /// Interaction logic for NetworkDisplayView.xaml
    /// </summary>
    public partial class NetworkDisplayView : UserControl
    {
        public NetworkDisplayViewModel _networkDisplayViewModel;
        public NetworkDisplayView(MainWindowViewModel mainWindow)
        {
            InitializeComponent();
            _networkDisplayViewModel = new NetworkDisplayViewModel(mainWindow);
            DataContext = _networkDisplayViewModel;
        }
        public NetworkDisplayView() // parameterless constructor for designer
        {
            InitializeComponent();

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                return;

            _networkDisplayViewModel = new NetworkDisplayViewModel(null);
            DataContext = _networkDisplayViewModel;
        }
    }
}
