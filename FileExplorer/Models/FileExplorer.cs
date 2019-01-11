using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FileExplorer.Models
{
    public static class FileOperations
    {
        public static void Open(string path)
        {
            if (File.Exists(path))
                Process.Start(path);
        }

        public static void Create(string path, int i = 1)
        {
            string s = i == 1 ? "" : $" ({i})";
            if (!File.Exists(path + s))
                File.Create(path + s);
            else
                Create(path, i + 1);
        }

        public static void Rename(string path, string name)
        {
            if (File.Exists(path))
            {
                var info = new FileInfo(path);
                var newPath = $"{info.DirectoryName}\\{name}";
                File.Move(path, newPath);
            }

        }
        public static void Move(string oldPath, string newPath)
        {
            if (File.Exists(oldPath))
                File.Move(oldPath, newPath);
        }
        public static void Copy(string oldPath, string newPath)
        {
            if (File.Exists(oldPath))
                File.Copy(oldPath, newPath);
        }
        public static void Delete(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

    }
    public static class FolderOperations
    {
        public static ObservableCollection<FileExplorerObject> StartPage()
        {
            ObservableCollection<FileExplorerObject> result = new ObservableCollection<FileExplorerObject>();
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var documents = new DirectoryInfo(documentsPath);
            var desktop = new DirectoryInfo(desktopPath);
            result.Add(new FileExplorerObject("Документы", documents.FullName, Type.Folder, documents.LastWriteTime.ToString()));
            result.Add(new FileExplorerObject("Рабочий стол", desktop.FullName, Type.Folder, desktop.LastWriteTime.ToString()));
            foreach (var disc in Environment.GetLogicalDrives())
            {
                var directoryInfo = new DirectoryInfo(disc);
                try
                {
                    if (!directoryInfo.Attributes.HasFlag(FileAttributes.NotContentIndexed) && (directoryInfo.EnumerateDirectories().Any() || directoryInfo.EnumerateFiles().Any()))
                        result.Add(new FileExplorerObject(directoryInfo.Name, directoryInfo.FullName, Type.Folder, directoryInfo.LastWriteTime.ToString()));
                }
                catch (IOException) { }
            }
            return result;
        }
        public static ObservableCollection<FileExplorerObject> Open(string path)
        {
            if (path == "Home")
                return StartPage();
            if (!Directory.Exists(path))
                return null;
            ObservableCollection<FileExplorerObject> result = new ObservableCollection<FileExplorerObject>();
            var directoryInfo = new DirectoryInfo(path);
            var folders = directoryInfo.EnumerateDirectories();
            var files = directoryInfo.EnumerateFiles();
            foreach (var item in folders)
                if (!item.Attributes.HasFlag(FileAttributes.Hidden))
                    result.Add(new FileExplorerObject(item.Name, item.FullName, Type.Folder, item.LastWriteTime.ToString()));
            foreach (var item in files)
            {
                if (!item.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    string size;
                    if (item.Length < 1024)
                        size = $"{item.Length} байт";
                    else if (item.Length < 1024 * 1024)
                        size = $"{((double)item.Length / 1024).ToString("0.##")} Кбайт";
                    else if (item.Length < 1024 * 1024 * 1024)
                        size = $"{((double)item.Length / 1024 / 1024).ToString("0.##")} Мбайт";
                    else
                        size = $"{((double)item.Length / 1024 / 1024 / 1024).ToString("0.##")} Гбайт";
                    result.Add(new FileExplorerObject(item.Name, item.FullName, Type.File, item.LastWriteTime.ToString(), size));
                }
            }
            return result;
        }
        public static void Create(string path, int i = 1)
        {
            string s = i == 1 ? "" : $" ({i})";
            if (!Directory.Exists(path + s))
                Directory.CreateDirectory(path + s);
            else
                Create(path, i + 1);
        }
        public static void Rename(string path, string name)
        {
            if (Directory.Exists(path))
            {
                var info = new DirectoryInfo(path);
                var newPath = $"{info.Parent.FullName}\\{name}";

                Directory.Move(path, newPath);
            }
        }

        // Folder: A    oldpPath: c://A   newPath: d://A
        public static void Move(string oldPath, string newPath)
        {
            if (Directory.Exists(oldPath))
                Directory.Move(oldPath, newPath);
        }

        // Folder: A    oldpPath: c://A   newPath: d://
        public static void Copy(string oldPath, string newPath)
        {
            if (Directory.Exists(oldPath))
            {
                DirectoryInfo directory = new DirectoryInfo(oldPath);
                Directory.CreateDirectory($"{newPath}\\{directory.Name}");
                foreach (var file in directory.EnumerateFiles())
                    file.CopyTo($"{newPath}\\{directory.Name}\\{file.Name}");
                foreach (var folder in directory.EnumerateDirectories())
                    Copy($"{oldPath}\\{folder.Name}", $"{newPath}\\{directory.Name}\\{folder.Name}");
            }
        }
        public static void Delete(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path);
        }
    }
}
