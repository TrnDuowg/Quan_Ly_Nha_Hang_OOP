using QuanLyNhaHang.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuanLyNhaHang.ViewModel
{
    public class AccountViewModel : BaseViewModel
    {
        // --- 1. Dữ liệu hiển thị ---
        private string _DisplayName;
        public string DisplayName { get => _DisplayName; set { _DisplayName = value; OnPropertyChanged(); } }

        private string _UserName;
        public string UserName { get => _UserName; set { _UserName = value; OnPropertyChanged(); } }

        private string _PhoneNumber;
        public string PhoneNumber { get => _PhoneNumber; set { _PhoneNumber = value; OnPropertyChanged(); } }

        // --- 2. Lệnh ---
        public ICommand UpdateInfoCommand { get; set; }
        public ICommand ChangePasswordCommand { get; set; }

        public AccountViewModel()
        {
            LoadCurrentUserInfo();

            // LỆNH 1: CẬP NHẬT THÔNG TIN
            UpdateInfoCommand = new RelayCommand<object>((p) => !string.IsNullOrEmpty(DisplayName), (p) =>
            {
                if (UserSession.CurrentUser == null) return;

                try
                {
                    string query = $"UPDATE Users SET DisplayName = N'{DisplayName}', PhoneNumber = '{PhoneNumber}' WHERE Id = {UserSession.CurrentUser.Id}";
                    DataProvider.Ins.ExecuteNonQuery(query);

                    // Cập nhật lại Session để đồng bộ
                    UserSession.CurrentUser.DisplayName = DisplayName;
                    UserSession.CurrentUser.PhoneNumber = PhoneNumber;

                    MessageBox.Show("Cập nhật thông tin thành công!");
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            });

            // LỆNH 2: ĐỔI MẬT KHẨU
            ChangePasswordCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                // Lấy 3 ô PasswordBox từ View truyền sang
                var values = (object[])p;
                var pBoxOld = (PasswordBox)values[0];
                var pBoxNew = (PasswordBox)values[1];
                var pBoxConfirm = (PasswordBox)values[2];

                string oldPass = pBoxOld.Password;
                string newPass = pBoxNew.Password;
                string confirmPass = pBoxConfirm.Password;

                // Kiểm tra
                if (string.IsNullOrEmpty(oldPass) || string.IsNullOrEmpty(newPass) || string.IsNullOrEmpty(confirmPass))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                    return;
                }

                // So sánh mật khẩu cũ (Trong Model User.cs bạn đặt là PasswordHash)
                if (oldPass != UserSession.CurrentUser.PasswordHash)
                {
                    MessageBox.Show("Mật khẩu cũ không đúng!");
                    return;
                }

                if (newPass != confirmPass)
                {
                    MessageBox.Show("Mật khẩu mới không khớp!");
                    return;
                }

                try
                {
                    // Cập nhật vào DB
                    string query = $"UPDATE Users SET PasswordHash = '{newPass}' WHERE Id = {UserSession.CurrentUser.Id}";
                    DataProvider.Ins.ExecuteNonQuery(query);

                    // Cập nhật Session
                    UserSession.CurrentUser.PasswordHash = newPass;

                    MessageBox.Show("Đổi mật khẩu thành công!");

                    // Xóa trắng các ô nhập
                    pBoxOld.Password = ""; pBoxNew.Password = ""; pBoxConfirm.Password = "";
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            });
        }

        void LoadCurrentUserInfo()
        {
            if (UserSession.CurrentUser != null)
            {
                DisplayName = UserSession.CurrentUser.DisplayName;
                UserName = UserSession.CurrentUser.UserName;
                PhoneNumber = UserSession.CurrentUser.PhoneNumber;
            }
        }
    }
}