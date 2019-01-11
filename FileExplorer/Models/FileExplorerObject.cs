using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Models
{
    public class FileExplorerObject //  ¯\_(ツ)_/¯ 
    {
        public FileExplorerObject(string name, string path, Type type,string dateTime, string size ="")
        {
            Date = dateTime;
            Image = size == "" ? "folder.png" : "file.png";
            Name = name;
            Path = path;
            Size = size;
            Type = type;
        }

        public string Date { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Size { get; set; }
        public Type Type { get; set; }
    }

    public enum Type
    {
        File,
        Folder
    }
}
