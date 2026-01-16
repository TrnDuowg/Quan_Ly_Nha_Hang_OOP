using QuanLyNhaHang.Model;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;
using System;

namespace QuanLyNhaHang.ViewModel
{
    public class CustomerOrderViewModel : BaseViewModel
    {
        // 1. Dữ liệu hiển thị
        private ObservableCollection<Food> _ListFood;
        public ObservableCollection<Food> ListFood { get => _ListFood; set { _ListFood = value; OnPropertyChanged(); } }

        private ObservableCollection<Table> _ListTable;
        public ObservableCollection<Table> ListTable { get => _ListTable; set { _ListTable = value; OnPropertyChanged(); } }

        private string _CustomerPhone;
        public string CustomerPhone { get => _CustomerPhone; set { _CustomerPhone = value; OnPropertyChanged(); } }

        private Table _CurrentTable;
        public Table CurrentTable
        {
            get => _CurrentTable;
            set
            {
                _CurrentTable = value;
                OnPropertyChanged();
                IsTableSelected = (_CurrentTable != null);
            }
        }

        private bool _IsTableSelected;
        public bool IsTableSelected { get => _IsTableSelected; set { _IsTableSelected = value; OnPropertyChanged(); } }

        // 2. Lệnh
        public ICommand OrderCommand { get; set; }
        public ICommand SelectTableCommand { get; set; }

        public CustomerOrderViewModel()
        {
            // --- QUAN TRỌNG: Bọc try-catch để không bị sập app nếu lỗi DB ---
            try
            {
                LoadMenu();
                LoadTables();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu Khách hàng: " + ex.Message);
            }
            // ---------------------------------------------------------------

            IsTableSelected = false;

            // Lệnh CHỌN BÀN
            SelectTableCommand = new RelayCommand<Table>((t) =>
            {
                return t != null && t.Status != "Đặt trước";
            }, (t) =>
            {
                if (t.Status == "Đặt trước")
                {
                    MessageBox.Show("Bàn này đã được đặt trước, vui lòng chọn bàn khác!");
                    return;
                }
                CurrentTable = t;
                IsTableSelected = true;
            });

            // Lệnh GỌI MÓN
            OrderCommand = new RelayCommand<Food>((food) =>
            {
                return CurrentTable != null;
            }, (food) =>
            {
                try
                {
                    // 1. Kiểm tra hóa đơn
                    string queryBill = $"SELECT * FROM Bills WHERE TableId = {CurrentTable.Id} AND Status = 0";
                    DataTable dataBill = DataProvider.Ins.ExecuteQuery(queryBill);

                    int idBill = -1;
                    if (dataBill.Rows.Count > 0)
                    {
                        idBill = (int)dataBill.Rows[0]["Id"];
                    }
                    else
                    {
                        // Tạo hóa đơn mới
                        DataProvider.Ins.ExecuteNonQuery($"INSERT INTO Bills (DateCheckIn, DateCheckOut, TableId, Status) VALUES (GETDATE(), NULL, {CurrentTable.Id}, 0)");

                        // Lấy ID an toàn
                        object result = DataProvider.Ins.ExecuteScalar("SELECT MAX(Id) FROM Bills");
                        idBill = (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 1;

                        // Cập nhật bàn
                        DataProvider.Ins.ExecuteNonQuery($"UPDATE DiningTable SET Status = N'Có người' WHERE Id = {CurrentTable.Id}");
                    }

                    // 2. Tích điểm (Nếu có nhập SĐT)
                    if (!string.IsNullOrEmpty(CustomerPhone))
                    {
                        string queryFindUser = $"SELECT Id FROM Users WHERE PhoneNumber = '{CustomerPhone}' AND UserType = 'Customer'";
                        object resultId = DataProvider.Ins.ExecuteScalar(queryFindUser);

                        if (resultId != null)
                        {
                            int userId = Convert.ToInt32(resultId);
                            // Cộng điểm vào bảng Customers (Lưu ý bảng Customers liên kết qua UserId)
                            DataProvider.Ins.ExecuteNonQuery($"UPDATE Customers SET Points = Points + 10 WHERE UserId = {userId}");
                        }
                    }

                    // 3. Thêm món vào BillInfo (Đã bỏ cột Status)
                    string checkFood = $"SELECT * FROM BillInfos WHERE BillId = {idBill} AND FoodId = {food.Id}";
                    if (DataProvider.Ins.ExecuteQuery(checkFood).Rows.Count > 0)
                    {
                        DataProvider.Ins.ExecuteNonQuery($"UPDATE BillInfos SET Count = Count + 1 WHERE BillId = {idBill} AND FoodId = {food.Id}");
                    }
                    else
                    {
                        DataProvider.Ins.ExecuteNonQuery($"INSERT INTO BillInfos (BillId, FoodId, Count, CurrentPrice) VALUES ({idBill}, {food.Id}, 1, {food.Price})");
                    }

                    MessageBox.Show($"Đã thêm món {food.Name} vào {CurrentTable.Name}!", "Thành công");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi gọi món: " + ex.Message);
                }
            });
        }

        void LoadMenu()
        {
            ListFood = new ObservableCollection<Food>();
            DataTable data = DataProvider.Ins.ExecuteQuery("SELECT * FROM Food");
            foreach (DataRow item in data.Rows) ListFood.Add(new Food(item));
        }

        void LoadTables()
        {
            ListTable = new ObservableCollection<Table>();
            DataTable data = DataProvider.Ins.ExecuteQuery("SELECT * FROM DiningTable");
            foreach (DataRow item in data.Rows) ListTable.Add(new Table(item));
        }
    }
}