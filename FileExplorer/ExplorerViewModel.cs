using FileExplorer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer
{
    public class ExplorerViewModel : INotifyPropertyChanged
    {

        public ExplorerViewModel()
        {
            Objects = FolderOperations.StartPage();
            PrivateCurrentDirectory = "Home";
        }


        //Fields

        private Stack<string> backHistory;
        private ObservableCollection<RelayCommand> commands;
        private FileExplorerObject clipboard;
        private string currentDirectory; //для текстбокса и метода Go
        private Stack<string> forwardHistory;
        private ObservableCollection<FileExplorerObject> objects;
        private string privateCurrentDirectory; // для всего остального
        private bool renameTextBoxShowed;
        private FileExplorerObject selectedObject;


        //Properties

        private Stack<string> BackHistory
        {
            get => backHistory ?? (backHistory = new Stack<string>());
            set=> backHistory = value;
        }
        public ObservableCollection<RelayCommand> Commands
        {
            get => commands;
            set
            {
                commands = value;
                OnPropertyChanged(nameof(Commands));
            }
        }
        public string CurrentDirectory
        {
            get => currentDirectory;
            set
            {
                currentDirectory = value;
                OnPropertyChanged(nameof(CurrentDirectory));
            }
        }
        private Stack<string> ForwardHistory
        {
            get => forwardHistory ?? (forwardHistory = new Stack<string>());
            set => forwardHistory = value;
        }
        public ObservableCollection<FileExplorerObject> Objects
        {
            get => objects;
            set
            {
                objects = value;
                OnPropertyChanged(nameof(Objects));
            }

        }
        private string PrivateCurrentDirectory
        {
            get => privateCurrentDirectory;
            set
            {
                privateCurrentDirectory = value;
                CurrentDirectory = value;
            }
        }
        public bool RenameTextBoxShowed
        {
            get => renameTextBoxShowed;
            set
            {
                renameTextBoxShowed = value;
                OnPropertyChanged(nameof(RenameTextBoxShowed));
            }
        }
        public FileExplorerObject SelectedObject
        {
            get => selectedObject;
            set
            {
                selectedObject = value;
                OnPropertyChanged(nameof(SelectedObject));
            }
        }


        // Commands

        private RelayCommand back;
        private RelayCommand clearClipboard;
        private RelayCommand copy;
        private RelayCommand createFile;
        private RelayCommand createFolder;
        private RelayCommand delete;
        private RelayCommand forward;
        private RelayCommand go;
        private RelayCommand home;
        private RelayCommand move;
        private RelayCommand open;
        private RelayCommand paste;
        private RelayCommand rename;


        public RelayCommand Back
        {
            get
            {
                return back ?? (back = new RelayCommand("Назад",
                    obj =>
                    {
                        ForwardHistory.Push(PrivateCurrentDirectory);
                        PrivateCurrentDirectory = BackHistory.Pop();
                        Objects = FolderOperations.Open(PrivateCurrentDirectory);
                    },
                    obj => BackHistory.Any()));
            }
        }
        public RelayCommand ClearClipboard
        {
            get
            {
                return clearClipboard ?? (clearClipboard = new RelayCommand("Отмена",
                    obj =>
                    {
                        clipboard = null;
                    },
                    obj => clipboard != null));
            }
        }
        public RelayCommand Copy
        {
            get
            {
                return copy ?? (copy = new RelayCommand("Копировать",
                    obj =>
                    {
                        clipboard = SelectedObject;
                    },
                    obj => SelectedObject != null && clipboard == null && PrivateCurrentDirectory != "Home"));
            }
        }
        public RelayCommand CreateFile
        {
            get
            {
                return createFile ?? (createFile = new RelayCommand("Создать файл",
                    obj =>
                    {
                        FileOperations.Create(PrivateCurrentDirectory + "\\newfile");
                        Objects = FolderOperations.Open(PrivateCurrentDirectory);
                    },
                    obj => PrivateCurrentDirectory != "Home"));
            }
        }
        public RelayCommand CreateFolder
        {
            get
            {
                return createFolder ?? (createFolder = new RelayCommand("Создать файл",
                    obj =>
                    {
                        FolderOperations.Create(PrivateCurrentDirectory + "\\New Folder");
                        Objects = FolderOperations.Open(PrivateCurrentDirectory);

                    },
                    obj => PrivateCurrentDirectory != "Home"));
            }
        }
        public RelayCommand Delete
        {
            get
            {
                return delete ?? (delete = new RelayCommand("Удалить",
                    obj =>
                    {
                        if (SelectedObject.Type == Models.Type.File)
                            FileOperations.Delete(SelectedObject.Path);
                        else
                            FolderOperations.Delete(SelectedObject.Path);
                        SelectedObject = null;
                        Objects = FolderOperations.Open(PrivateCurrentDirectory);
                    },
                    obj => SelectedObject != null && PrivateCurrentDirectory != "Home"));
            }
        }
        public RelayCommand Forward
        {
            get
            {
                return forward ?? (forward = new RelayCommand("Вперед",
                    obj =>
                    {
                        BackHistory.Push(PrivateCurrentDirectory);
                        PrivateCurrentDirectory = ForwardHistory.Pop();
                        Objects = FolderOperations.Open(PrivateCurrentDirectory);
                    },
                    obj => ForwardHistory.Any()));
            }
        }
        public RelayCommand Go
        {
            get
            {
                return go ?? (go = new RelayCommand("Перейти",
                    obj =>
                    {
                        if (Directory.Exists(CurrentDirectory) || CurrentDirectory == "Home")
                        {
                            BackHistory.Push(PrivateCurrentDirectory);
                            Objects = FolderOperations.Open(CurrentDirectory);
                        }
                    },
                    obj => !string.IsNullOrWhiteSpace(CurrentDirectory)));
            }
        }
        public RelayCommand Home
        {
            get
            {
                return home ?? (home = new RelayCommand("Вперед",
                    obj =>
                    {
                        Objects = FolderOperations.StartPage();
                        PrivateCurrentDirectory = "Home";
                    },
                    obj => true));
            }
        }
        public RelayCommand Move
        {
            get
            {
                return move ?? (move = new RelayCommand("Переместить",
                    obj =>
                    {
                        if (clipboard.Type == Models.Type.File)
                            FileOperations.Move(clipboard.Path, $"{PrivateCurrentDirectory}\\{clipboard.Name}");
                        else
                            FolderOperations.Move(clipboard.Path, $"{PrivateCurrentDirectory}\\{clipboard.Name}");
                        clipboard = null;
                        Objects = FolderOperations.Open(PrivateCurrentDirectory);
                    },
                    obj => clipboard != null && PrivateCurrentDirectory != "Home"));
            }
        }
        public RelayCommand Open
        {
            get
            {
                return open ?? (open = new RelayCommand("Открыть",
                    obj =>
                    {
                        if (SelectedObject.Type == Models.Type.File)
                            FileOperations.Open(SelectedObject.Path);
                        else
                        {
                            BackHistory.Push(PrivateCurrentDirectory);
                            PrivateCurrentDirectory = SelectedObject.Path;
                            Objects = FolderOperations.Open(SelectedObject.Path);
                        }
                        SelectedObject = null;
                    },
                    obj => SelectedObject != null));
            }
        }
        public RelayCommand Paste
        {
            get
            {
                return paste ?? (paste = new RelayCommand("Вставить",
                   obj =>
                   {
                       if (clipboard.Type == Models.Type.File)
                           FileOperations.Copy(clipboard.Path, $"{PrivateCurrentDirectory}\\{clipboard.Name}");
                       else
                           FolderOperations.Copy(clipboard.Path, $"{PrivateCurrentDirectory}");
                       clipboard = null;
                       Objects = FolderOperations.Open(PrivateCurrentDirectory);
                   },
                    obj => clipboard != null && PrivateCurrentDirectory != "Home"));
            }
        }
        public RelayCommand Rename 
        {
            get
            {
                return rename ?? (rename = new RelayCommand("Переименовать",
                    obj =>
                    {
                        if (RenameTextBoxShowed)
                        {
                            if (SelectedObject.Type == Models.Type.File)
                                FileOperations.Rename(SelectedObject.Path, SelectedObject.Name);
                            else
                                FolderOperations.Rename(SelectedObject.Path, SelectedObject.Name);

                            SelectedObject = null;
                            Objects = FolderOperations.Open(PrivateCurrentDirectory);
                            RenameTextBoxShowed = false;
                        }
                        else
                            RenameTextBoxShowed = true;
                    },
                    obj => SelectedObject != null && PrivateCurrentDirectory != "Home"));
            }
        }

        //INotifyPropertyChanged implemention

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
