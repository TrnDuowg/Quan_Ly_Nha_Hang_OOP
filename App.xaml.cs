using System.Windows;
using QuanLyNhaHang.View;
using QuanLyNhaHang.ViewModel;

namespace QuanLyNhaHang
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1. Hiện màn hình đăng nhập
            LoginWindow loginWindow = new LoginWindow();
            var loginVM = loginWindow.DataContext as LoginViewModel;

            loginWindow.ShowDialog(); // Chờ người dùng đăng nhập

            // 2. Kiểm tra kết quả
            if (loginVM != null && loginVM.IsLogin)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
            else
            {
                Shutdown(); // Tắt app
            }
        }
    }
}