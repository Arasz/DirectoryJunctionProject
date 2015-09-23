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
    class DirJunctionViewModel: INotifyPropertyChanged
    {
        public string LinkName { get; set; } = "Link name";

        private string _linkDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
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

        public ICommand CreateJunctionCommand { get; private set; }

        private void CreateCreateJunctionCommand()
        {
            CreateJunctionCommand = new RelayCommand<object>(CreateJunctionExecute, CanExecuteCreateJunctionCommand);
        }

        public async void CreateJunctionExecute(object dummy)
        {
            try
            {
                string output = await CommandLineHelper.RunCmdAsync(@"mklink /J " + $"\"{LinkDirectoryPath}\\{LinkName}\" \"{TargetPath}\" ");
                Debug.Print(output);
            }
            catch(SystemException exception)
            {
                Debug.Print(exception.Message);
            }
        }

        public bool CanExecuteCreateJunctionCommand(object dummy)
        {
            return  Directory.Exists(TargetPath) &&
                    Directory.Exists(LinkDirectoryPath) &&
                    !Directory.Exists($"{LinkDirectoryPath}\\{LinkName}");
        }
        #endregion
    }
}
