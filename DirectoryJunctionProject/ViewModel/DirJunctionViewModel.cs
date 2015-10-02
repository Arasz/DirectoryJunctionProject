using DirectoryJunctionProject.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Diagnostics;

namespace DirectoryJunctionProject.ViewModel
{
    //TODO: Write comments.
    //TODO: Use some more async operations
    //TODO: Write unit tests.
    //TODO: Write model for application

        /// <summary>
        /// View model of application
        /// </summary>
    class DirJunctionViewModel: INotifyPropertyChanged
    {
        /// <summary>
        /// Name of created directory link
        /// </summary>
        public string LinkName { get; set; } = "Link name";

        public bool OutputReady { get; set; } = false;

        private string _linkDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        /// <summary>
        /// Path to directory where link will be created
        /// </summary>
        public string LinkDirectoryPath
        {
            get { return _linkDirectoryPath; }
            set
            {
                _linkDirectoryPath = value;
                OnPropertyChanged(nameof(LinkDirectoryPath));
            }
        }

        private string _targetPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        /// <summary>
        /// Path to target (target - directory to which the link will be created )
        /// </summary>
        public string TargetPath
        {
            get
            { return _targetPath; }
            set
            {
                _targetPath = value;
                OnPropertyChanged(nameof(TargetPath));
            }
        }

        /// <summary>
        /// Dialog used to choose target and link directory
        /// </summary>
        private CommonFileDialog _folderDialog;

        public DirJunctionViewModel()
        {
            _folderDialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
                DefaultDirectory = TargetPath,
            };

            CreateSelectTargetCommand();
            CreateSelectLinkDirectoryCommand();
            CreateCreateJunctionCommand();
        }

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler eventHandler = PropertyChanged;
            eventHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion 


        #region Commands

        /// <summary>
        /// Command used to select target
        /// </summary>
        public ICommand SelectTargetCommand { get; private set; }

        private void CreateSelectTargetCommand()
        {
            SelectTargetCommand = new RelayCommand<object>(SelectTargetExecute);
        }

        public void SelectTargetExecute(object dummy)
        {
            if (_folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
                TargetPath = _folderDialog.FileName;
        }

        /// <summary>
        /// Command used to select link directory
        /// </summary>
        public ICommand SelectLinkDirectoryCommand { get; private set; }

        private void CreateSelectLinkDirectoryCommand()
        {
            SelectLinkDirectoryCommand = new RelayCommand<object>(SelectLinkDirectoryExecute);
        }

        public void SelectLinkDirectoryExecute(object dummy)
        {
            if (_folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
                LinkDirectoryPath = _folderDialog.FileName;
        }

        /// <summary>
        /// Command used to create directory junction
        /// </summary>
        public ICommand CreateJunctionCommand { get; private set; }

        private void CreateCreateJunctionCommand()
        {
            CreateJunctionCommand = new RelayCommand<object>(CreateJunctionExecute, CanExecuteCreateJunctionCommand);
        }

        public async void CreateJunctionExecute(object dummy)
        {
           string output = await CommandLineHelper.RunCmdAsync(@"mklink /J " + $"\"{LinkDirectoryPath}\\{LinkName}\" \"{TargetPath}\" ");
           OutputReady = true;
            OnPropertyChanged(nameof(OutputReady));
        }

        public bool CanExecuteCreateJunctionCommand(object dummy)
        {
            return Directory.Exists(TargetPath) &&
                    Directory.Exists(LinkDirectoryPath) &&
                    !string.IsNullOrEmpty(LinkName) &&
                    !Directory.Exists($"{LinkDirectoryPath}\\{LinkName}");
        }
        #endregion
    }
}
