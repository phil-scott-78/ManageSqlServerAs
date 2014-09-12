using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
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
using MahApps.Metro.Controls;
using ManageSqlServerAs.ViewModels;
using ReactiveUI;

namespace ManageSqlServerAs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindowViewModel ViewModel;

        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainWindowViewModel();
                        DataContext = ViewModel;
            ApplicationListBox.Events().MouseDoubleClick
                .InvokeCommand(this, v => v.ViewModel.Connect);
            ApplicationListBox.Events().KeyUp.Where(i => i.Key == Key.Enter)
                .InvokeCommand(this, v => v.ViewModel.Connect);
        }
    }
}