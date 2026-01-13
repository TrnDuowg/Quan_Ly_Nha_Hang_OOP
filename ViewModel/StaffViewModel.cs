using QuanLyNhaHang.Model;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace QuanLyNhaHang.ViewModel
{
    public class StaffViewModel : BaseViewModel
    {
        // Danh sách nhân viên
        private ObservableCollection<Employee> _ListStaff;
        public ObservableCollection<Employee> ListStaff { get => _ListStaff; set { _ListStaff = value; OnPropertyChanged(); } }

        // Các trường binding dữ liệu nhập
        private string _DisplayName;
        public string DisplayName { get => _DisplayName; set { _DisplayName = value; OnPropertyChanged(); } }

        private string _UserName;
        public string UserName { get => _UserName; set { _UserName = value; OnPropertyChanged(); } }

        private string _Position;
        public string Position { get => _Position; set { _Position = value; OnPropertyChanged(); } }

        private decimal _Salary;
        public decimal Salary { get => _Salary; set { _Salary = value; OnPropertyChanged(); } }

        // Nhân viên đang chọn
        private Employee _SelectedStaff;
        public Employee SelectedStaff
        {
            get => _SelectedStaff;
            set
            {
                _SelectedStaff = value;
                OnPropertyChanged();
                if (SelectedStaff != null)
                {
                    DisplayName = SelectedStaff.DisplayName;
                    UserName = SelectedStaff.UserName;
                    Position = SelectedStaff.Position;
                    Salary = SelectedStaff.Salary;
                }
            }
        }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public StaffViewModel()
        {
            LoadListStaff();

            // --- THÊM NHÂN VIÊN ---
            AddCommand = new RelayCommand<object>((p) =>
            {
                return !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(DisplayName);
            }, (p) =>
            {
                // Kiểm tra trùng username
                var check = DataProvider.Ins.ExecuteQuery($"SELECT * FROM Users WHERE UserName = '{UserName}'");
                if (check.Rows.Count > 0) { MessageBox.Show("Tên đăng nhập đã tồn tại!"); return; }

                try
                {
                    // 1. Thêm vào bảng Users trước (Mật khẩu mặc định là '1')
                    string queryUser = $"INSERT INTO Users (DisplayName, UserName, PasswordHash, UserType) VALUES (N'{DisplayName}', '{UserName}', '1', 'Staff')";
                    DataProvider.Ins.ExecuteNonQuery(queryUser);

                    // 2. Lấy ID vừa tạo
                    int newId = (int)DataProvider.Ins.ExecuteScalar("SELECT MAX(Id) FROM Users");

                    // 3. Thêm vào bảng Employees
                    string queryEmp = $"INSERT INTO Employees (UserId, Position, Salary, HireDate) VALUES ({newId}, N'{Position}', {Salary}, GETDATE())";
                    DataProvider.Ins.ExecuteNonQuery(queryEmp);

                    MessageBox.Show("Thêm nhân viên thành công! Mật khẩu mặc định là '1'");
                    LoadListStaff();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            });

            // --- SỬA NHÂN VIÊN ---
            EditCommand = new RelayCommand<object>((p) =>
            {
                return SelectedStaff != null;
            }, (p) =>
            {
                try
                {
                    // Update bảng Users
                    DataProvider.Ins.ExecuteNonQuery($"UPDATE Users SET DisplayName = N'{DisplayName}' WHERE Id = {SelectedStaff.Id}");

                    // Update bảng Employees
                    DataProvider.Ins.ExecuteNonQuery($"UPDATE Employees SET Position = N'{Position}', Salary = {Salary} WHERE UserId = {SelectedStaff.Id}");

                    MessageBox.Show("Cập nhật thành công!");
                    LoadListStaff();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            });

            // --- XÓA NHÂN VIÊN ---
            DeleteCommand = new RelayCommand<object>((p) => { return SelectedStaff != null; }, (p) =>
            {
                if (MessageBox.Show("Bạn có chắc muốn xóa nhân viên này? Tài khoản sẽ bị khóa.", "Cảnh báo", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // Xóa logic (Soft Delete) bằng cách set IsActive = 0 để giữ lịch sử hóa đơn
                    DataProvider.Ins.ExecuteNonQuery($"UPDATE Users SET IsActive = 0 WHERE Id = {SelectedStaff.Id}");
                    LoadListStaff();
                }
            });
        }

        void LoadListStaff()
        {
            ListStaff = new ObservableCollection<Employee>();
            // Join 2 bảng để lấy đầy đủ thông tin: User + Employee
            string query = "SELECT u.Id, u.DisplayName, u.UserName, u.PasswordHash, u.UserType, e.Position, e.Salary, e.HireDate " +
                           "FROM Users u JOIN Employees e ON u.Id = e.UserId " +
                           "WHERE u.IsActive = 1 AND u.UserType = 'Staff'";

            DataTable data = DataProvider.Ins.ExecuteQuery(query);
            foreach (DataRow item in data.Rows)
            {
                ListStaff.Add(new Employee(item));
            }
        }
    }
}