using System;
using System.Runtime.Serialization;
using ReactiveUI;

namespace ManageSqlServerAs.ViewModels
{
    [Serializable]
    public class ApplicationLink:ReactiveObject
    {
        private string _title;
        private string _path;
        private string _parameters;
        private string _defaultUserName;

        [DataMember]
        public string Title
        {
            get { return _title; }
            set { this.RaiseAndSetIfChanged(ref _title, value); }
        }

        [DataMember]
        public string Path
        {
            get { return _path; }
            set { this.RaiseAndSetIfChanged(ref _path, value); }
        }

        [DataMember]
        public string Parameters
        {
            get { return _parameters; }
            set { this.RaiseAndSetIfChanged(ref _parameters, value); }
        }

        [DataMember]
        public string DefaultUserName
        {
            get { return _defaultUserName; }
            set { this.RaiseAndSetIfChanged(ref _defaultUserName, value); }
        }
    }
}