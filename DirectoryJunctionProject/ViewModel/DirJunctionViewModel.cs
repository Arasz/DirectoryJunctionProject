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
using DirectoryJunctionProject.Model;
using System.Xml.Serialization;

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

        private DirJunctionModel _model;

        /// <summary>
        /// Dialog used to choose target and link directory
        /// </summary>
        private CommonFileDialog _folderDialog;

        /// <summary>
        /// Name of created directory link
        /// </summary>
        public string LinkName
        {           
            get { return _model.LinkName; }
            set { _model.LinkName = value; }
        }


        public bool OutputReady
        {
            get { return _model.OutputReady; }
            set { _model.OutputReady = value; }
        }

        public string CmdLineFeedback
        {
            get { return _model.CmdLineFeedback; }
            private set { _model.CmdLineFeedback = value; }
        }

        /// <summary>
        /// Path to directory where link will be created
        /// </summary>
        public string LinkDirectoryPath
        {
            get { return _model.LinkDirectoryPath; }
            set
            {
                _model.LinkDirectoryPath = value;
                OnPropertyChanged(nameof(LinkDirectoryPath));
            }
        }

        /// <summary>
        /// Path to target (target - directory to which the link will be created )
        /// </summary>
        public string TargetPath
        {
            get
            { return _model.TargetPath; }
            set
            {
                _model.TargetPath = value;
                OnPropertyChanged(nameof(TargetPath));
            }
        }

        public DirJunctionViewModel()
        {
            Deserialize();
            if(_model == null)
            {
                _model = new DirJunctionModel()
                {
                    LinkName = "NewLink",
                    LinkDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer),
                    TargetPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    OutputReady = false,
                    CmdLineFeedback = "",
                };
            }

            _folderDialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
                DefaultDirectory = TargetPath,
            };

            CreateSelectTargetCommand();
            CreateSelectLinkDirectoryCommand();
            CreateCreateJunctionCommand();
            CreatePopupClickedCommand();
            CreateWindowClosedCommand();
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
            CmdLineFeedback = await  CommandLineHelper.RunCmdAsync(@"mklink /J " + $"\"{LinkDirectoryPath}\\{LinkName} \" \"{TargetPath} \" ");
            OutputReady = !OutputReady;
            OnPropertyChanged(nameof(OutputReady));
            OnPropertyChanged(nameof(CmdLineFeedback));
        }

        public bool CanExecuteCreateJunctionCommand(object dummy)
        {
            return Directory.Exists(TargetPath) &&
                    Directory.Exists(LinkDirectoryPath) &&
                    !string.IsNullOrEmpty(LinkName) &&
                    !Directory.Exists($"{LinkDirectoryPath}\\{LinkName}");
        }

        /// <summary>
        /// Command which services click on pop-up button
        /// </summary>
        public ICommand PopupClickedCommand { get; private set; }

        public void CreatePopupClickedCommand()
        {
            PopupClickedCommand = new RelayCommand<object>(PopupClickedCommandExecute);
        }

        public void PopupClickedCommandExecute(object dummy)
        {
            OutputReady = !OutputReady;
            OnPropertyChanged(nameof(OutputReady));
        }
        /// <summary>
        /// Command which services serialization on window close
        /// </summary>
        public ICommand WindowClosedCommand { get; private set; }

        public void CreateWindowClosedCommand()
        {
            WindowClosedCommand = new RelayCommand<object>(WindowsClosedExecute);
        }

        public void WindowsClosedExecute(object dummy)
        {
            Serialize();
        }

        #endregion

        #region Serialization

        readonly private string _serializationFilePath =Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\state";

        private void Serialize()
        {
            using (Stream serializationStream = new FileStream(_serializationFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(DirJunctionModel));
                serializer.Serialize(serializationStream, _model);
            }

        }

        private void Deserialize()
        {
            if(File.Exists(_serializationFilePath))
            {
                using (Stream deserializationStream = new FileStream(_serializationFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(DirJunctionModel));
                    _model = (DirJunctionModel)serializer.Deserialize(deserializationStream);
                }
            }
        }


        #endregion
    }
}
