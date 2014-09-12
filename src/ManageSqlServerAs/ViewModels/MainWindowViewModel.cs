using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using ManageSqlServerAs.Tools;
using Newtonsoft.Json;
using ReactiveUI;

namespace ManageSqlServerAs.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private readonly string _fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ApplicationLinks.xml");

        private ReactiveList<ApplicationLink> _applicationLinks = new ReactiveList<ApplicationLink>();

        public MainWindowViewModel()
        {
            Add = ReactiveCommand.Create();
            (Add as ReactiveCommand<object>).Subscribe(_ => AddImpl());

            Browse = ReactiveCommand.Create();
            (Browse as ReactiveCommand<object>).Subscribe(_ => BrowseImpl());

            Edit = ReactiveCommand.Create(this.WhenAny(x => x.SelectedLink, t => t.Value != null));
            (Edit as ReactiveCommand<object>).Subscribe(_ => EditImpl());

            CancelEdit = ReactiveCommand.Create();
            (CancelEdit as ReactiveCommand<object>).Subscribe(_ => { IsEditing = false; });

            Delete = ReactiveCommand.Create(this.WhenAny(x => x.SelectedLink, t => t.Value != null));
            (Delete as ReactiveCommand<object>).Subscribe(_ => DeleteImpl());

            Connect = ReactiveCommand.Create(this.WhenAny(x => x.SelectedLink, t => t.Value != null));
            (Connect as ReactiveCommand<object>).Subscribe(_ => ConnectImpl());

            SaveItem = ReactiveCommand.Create(this.WhenAny(
                x => x.InEditPath, 
                y => y.InEditTitle, 
                (p,t) => 
                    string.IsNullOrWhiteSpace(t.Value) == false && 
                    string.IsNullOrWhiteSpace(p.Value) == false && 
                    File.Exists(p.Value)));

            (SaveItem as ReactiveCommand<object>).Subscribe(_ => SaveItemImpl());


            using (ApplicationLinks.SuppressChangeNotifications())
            {
                LoadApplicationLinks();
            }

            ApplicationLinks.Changed.Throttle(TimeSpan.FromSeconds(1)).Subscribe(_ => SaveApplicationLinks());
        }


        private void BrowseImpl()
        {
            var initialDirectory = string.Empty;
            if (File.Exists(InEditPath))
            {
                initialDirectory = Path.GetDirectoryName(InEditPath);
            }
            
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".exe",
                Filter = "Executables|*.exe;*.bat|All Files|*.*",
                FileName = InEditPath,
                InitialDirectory = initialDirectory,
                CheckFileExists = true
            };

            // Show open file dialog box
            var result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                InEditPath = dlg.FileName;
            }
        }

        private void SaveItemImpl()
        {

            if (IsAddMode)
            {
                var newLink = new ApplicationLink
                {
                    Title = InEditTitle,
                    Path = InEditPath,
                    Parameters = InEditParameters,
                    DefaultUserName = InEditUserName
                };
                ApplicationLinks.Add(newLink);
            }
            else
            {

                SelectedLink.Title = InEditTitle;
                SelectedLink.Path = InEditPath;
                SelectedLink.Parameters = InEditParameters;
                SelectedLink.DefaultUserName = InEditUserName;
            }
            
            IsEditing = false;
        }

        private void ConnectImpl()
        {            
            var promptForWindowsCredentials = CredentialUi.PromptForWindowsCredentials(SelectedLink.Title, "Please enter the password to launch " + SelectedLink.Title, SelectedLink.DefaultUserName, "");
            if (promptForWindowsCredentials == null)
            {
                return;
            }

            try
            {
                Launcher.CreateProcess(
                    promptForWindowsCredentials.UserName, 
                    promptForWindowsCredentials.DomainName,
                    promptForWindowsCredentials.Password, 
                    SelectedLink.Path + " " + SelectedLink.Parameters);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }            
        }

        private void AddImpl()
        {
            IsEditing = true;
            InEditTitle = string.Empty;
            InEditPath = string.Empty;
            InEditParameters = string.Empty;
            InEditUserName = string.Empty;
            IsEditing = true;
            IsAddMode = true;
        }

        private void EditImpl()
        {
            InEditTitle = SelectedLink.Title;
            InEditPath = SelectedLink.Path;
            InEditParameters = SelectedLink.Parameters;
            InEditUserName = SelectedLink.DefaultUserName;
            IsEditing = true;
            IsAddMode = false;
        }

        private void DeleteImpl()
        {
            ApplicationLinks.Remove(SelectedLink);
        }

        public ReactiveList<ApplicationLink> ApplicationLinks
        {
            get { return _applicationLinks; }
            set { this.RaiseAndSetIfChanged(ref _applicationLinks, value); }
        }

        public ICommand Delete { get; private set; }
        public ICommand Add { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Connect { get; private set; }
        public ICommand SaveItem { get; private set; }
        public ICommand Browse { get; private set; }
        public ICommand CancelEdit { get; private set; }

        private ApplicationLink _selectedLink;
        private bool _isEditing;
        private bool _isAddMode;
        private string _inEditTitle;
        private string _inEditPath;
        private string _inEditParameters;
        private string _inEditUserName;

        public ApplicationLink SelectedLink
        {
            get { return _selectedLink; }
            set { this.RaiseAndSetIfChanged(ref _selectedLink, value); }
        }

        public bool IsEditing
        {
            get { return _isEditing; }
            set { this.RaiseAndSetIfChanged(ref _isEditing, value); }
        }

        public bool IsAddMode
        {
            get { return _isAddMode; }
            set { this.RaiseAndSetIfChanged(ref _isAddMode, value); }
        }

        public string InEditTitle
        {
            get { return _inEditTitle; }
            set { this.RaiseAndSetIfChanged(ref _inEditTitle, value); }
        }

        public string InEditPath
        {
            get { return _inEditPath; }
            set { this.RaiseAndSetIfChanged(ref _inEditPath, value); }
        }

        public string InEditParameters
        {
            get { return _inEditParameters; }
            set { this.RaiseAndSetIfChanged(ref _inEditParameters, value); }
        }

        public string InEditUserName
        {
            get { return _inEditUserName; }
            set { this.RaiseAndSetIfChanged(ref _inEditUserName, value); }
        }

        private void LoadApplicationLinks()
        {
            if (File.Exists(_fileName) == false)
            {
                ApplicationLinks = new ReactiveList<ApplicationLink>();
            }
            else
            {
                try
                {
                    ApplicationLinks = JsonConvert.DeserializeObject<ReactiveList<ApplicationLink>>(File.ReadAllText(_fileName));
                }
                catch (Exception)
                {
                    ApplicationLinks = new ReactiveList<ApplicationLink>();
                }
            }
        }

        private void SaveApplicationLinks()
        {
            Debug.WriteLine("Saving links");
            try
            {
                var serialized = JsonConvert.SerializeObject(ApplicationLinks);
                File.WriteAllText(_fileName, serialized);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving links", ex.Message);
            }
        }
    }
}