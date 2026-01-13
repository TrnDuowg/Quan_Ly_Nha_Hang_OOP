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

        // Danh sách bàn để khách chọn lúc mới vào
        private ObservableCollection<Table> _ListTable;
        public ObservableCollection<Table> ListTable { get => _ListTable; set { _ListTable = value; OnPropertyChanged(); } }

        // Bàn khách đang ngồi
        private Table _CurrentTable;
        public Table CurrentTable
        {
            get => _CurrentTable;
            set
            {
                _CurrentTable = value;
                OnPropertyChanged();
                // Khi chọn bàn xong -> Ẩn màn hình chọn bàn, hiện Menu
                IsTableSelected = (_CurrentTable != null);
            }
        }

        // Biến điều khiển giao diện (True: Hiện Menu, False: Hiện chọn bàn)
        private bool _IsTableSelected;
        public bool IsTableSelected { get => _IsTableSelected; set { _IsTableSelected = value; OnPropertyChanged(); } }

        // 2. Lệnh gọi món
        public ICommand OrderCommand { get; set; }
        public ICommand SelectTableCommand { get; set; }

        public CustomerOrderViewModel()
        {
            LoadMenu();
            LoadTables();
            IsTableSelected = false; // Mặc định phải chọn bàn trước

            SelectTableCommand = new RelayCommand<Table>((t) =>
            {
                // Điều kiện để nút ấn được: Bàn khác null VÀ KHÔNG PHẢI là bàn đặt trước
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

            // Lệnh GỌI MÓN (Khi khách bấm nút +)
            OrderCommand = new RelayCommand<Food>((food) =>
            {
                return CurrentTable != null; // Phải có bàn mới được gọi
            }, (food) =>
            {
                try
                {
                    // 1. Kiểm tra bàn này đã có hóa đơn chưa
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
                        idBill = (int)DataProvider.Ins.ExecuteScalar("SELECT MAX(Id) FROM Bills");

                        // Cập nhật bàn thành Có người
                        DataProvider.Ins.ExecuteNonQuery($"UPDATE DiningTable SET Status = N'Có người' WHERE Id = {CurrentTable.Id}");
                    }

                    // 2. Thêm món vào BillInfo
                    // Kiểm tra xem món đó đã có chưa để cộng dồn
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
            // Khách chỉ được chọn bàn Trống hoặc bàn đang Có người (nếu muốn gọi thêm)
            DataTable data = DataProvider.Ins.ExecuteQuery("SELECT * FROM DiningTable");
            foreach (DataRow item in data.Rows) ListTable.Add(new Table(item));
        }
    }
}