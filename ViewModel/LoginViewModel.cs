using QuanLyNhaHang.Model;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuanLyNhaHang.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        public bool IsLogin { get; set; } = false;
        private string _UserName;
        public string UserName { get => _UserName; set { _UserName = value; OnPropertyChanged(); } }
        private string _Password;
        public string Password { get => _Password; set { _Password = value; OnPropertyChanged(); } }

        // Lệnh đăng nhập và Thoát
        public ICommand LoginCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand PasswordChangedCommand { get; set; }

        public LoginViewModel()
        {
            UserName = "";
            Password = "";

            // Xử lý sự kiện đăng nhập
            LoginCommand = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                Login(p);
            });

            // Xử lý sự kiện thoát
            CloseCommand = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                if (p == null) return;

                // Hiển thị hộp thoại xác nhận
                var result = MessageBox.Show("Bạn có chắc chắn muốn thoát chương trình không?",
                                             "Xác nhận thoát",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Question);

                // Nếu người dùng chọn Yes thì mới đóng cửa sổ
                if (result == MessageBoxResult.Yes)
                {
                    p.Close();
                }
                // Nếu chọn No thì không làm gì cả (chương trình vẫn chạy)
            });

            // Xử lý lấy mật khẩu từ PasswordBox
            PasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { Password = p.Password; });
        }

        void Login(Window p)
        {
            if (p == null) return;

            // Kiểm tra SQL injection cơ bản bằng cách dùng Parameter (đã xử lý trong DataProvider)
            // Hoặc viết query an toàn. Ở đây demo viết thẳng để dễ hiểu logic:

            string query = "SELECT * FROM Users WHERE UserName = @user AND PasswordHash = @pass";

            // Gọi xuống SQL qua Singleton DataProvider
            DataTable result = DataProvider.Ins.ExecuteQuery(query, new object[] { UserName, Password });

            if (result.Rows.Count > 0)
            {
                IsLogin = true;
                UserSession.CurrentUser = new User(result.Rows[0]);
                p.Hide(); // Ẩn màn hình đăng nhập đi
                MainWindow main = new MainWindow(); // Tạo màn hình chính
                main.ShowDialog(); // Hiển thị màn hình chính và đợi nó đóng

                // Khi màn hình chính đóng lại (người dùng tắt app), thì mới đóng login và thoát
                p.Close();
            }
            else
            {
                IsLogin = false;
                MessageBox.Show("Sai tài khoản hoặc mật khẩu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}