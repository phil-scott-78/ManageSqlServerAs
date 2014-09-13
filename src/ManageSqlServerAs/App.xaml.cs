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
            try
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            MainWindow = new MainWindow();
            if (e.Args.Length < 1)
            {
                MainWindow.Show();
                return;
            }

            FindAndLaunchByHash(((MainWindow) MainWindow).ViewModel, e.Args[0]);
            Current.Shutdown();
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // put your tracing or logging code here (I put a message box as an example)
            MessageBox.Show(e.ExceptionObject.ToString());
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
