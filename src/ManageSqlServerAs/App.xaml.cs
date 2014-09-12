using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using ManageSqlServerAs.ViewModels;
using Microsoft.Shell;

namespace ManageSqlServerAs
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : ISingleInstanceApp
    {
        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance("ManageSqlServerAs"))
            {
                var application = new App();
                application.Init();
                application.Run();
                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            MainWindow = new MainWindow();

            if (e.Args.Length < 1)
            {
                MainWindow.Show();
                return;
            }

            FindAndLaunchByHash(((MainWindow) MainWindow).ViewModel, e.Args[0]);
            Current.Shutdown();
        }

        public void Init()
        {
            InitializeComponent();
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            if (args == null || args.Count < 2)
                return false;

            FindAndLaunchByHash(((MainWindow) MainWindow).ViewModel, args[1]);
            return true;
        }

        private static void FindAndLaunchByHash(MainWindowViewModel viewModel, string hash)
        {
            ApplicationLink applicationLink = viewModel.ApplicationLinks.FirstOrDefault(i => i.GetHashCode().ToString(CultureInfo.InvariantCulture) == hash);
            if (applicationLink == null)
            {
                return;
            }

            viewModel.SelectedLink = applicationLink;
            viewModel.Connect.Execute(null);
        }
    }
}
