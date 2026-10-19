using System.Windows;
using Slashers.WPF.Data;

namespace Slashers.WPF;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        try
        {
            Database.Initialize();
            var mainWindow = new MainWindow();
            MainWindow = mainWindow;
            mainWindow.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Slashers tidak dapat tersambung ke SQL Server LocalDB.\n\n" +
                "Pastikan SQL Server Express LocalDB telah terpasang, lalu periksa konfigurasi pada Data/Database.cs.\n\nDetail: " + ex.Message,
                "Koneksi Database Slashers Gagal", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(-1);
            return;
        }
    }
}
