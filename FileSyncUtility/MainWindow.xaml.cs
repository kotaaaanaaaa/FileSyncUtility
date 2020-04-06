using FileSyncUtility.Common;
using System.IO;
using System.Reflection;
using System.Windows;

namespace FileSyncUtility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ApplicationDbContext.ConnectionString = @"Data Source=default.db";
            ApplicationDbContext.SqlFileBasePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Sqls");
            var vm = new MainWindowViewModel();
            vm.ReloadSynchronizeItems();
            DataContext = vm;
        }
    }
}
