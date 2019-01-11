using System.Windows;

namespace FileExplorer
{
    public partial class ExplorerView : Window
    {
        public ExplorerView()
        {
            InitializeComponent();
            DataContext = new ExplorerViewModel();
        }
        
    }
}
