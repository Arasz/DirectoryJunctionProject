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

namespace DirectoryJunctionProject.ViewModel
{
    class DirJunctionViewModel: INotifyPropertyChanged
    {
        public string LinkName { get; set; } = "Link name";

        public string LinkDirectoryPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        private string _targetPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public string TargetPath
        {
            get
            {
                return _targetPath;
            }
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
        #endregion
    }
}
