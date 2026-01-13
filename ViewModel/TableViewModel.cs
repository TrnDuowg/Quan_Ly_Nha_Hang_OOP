using QuanLyNhaHang.DTO;
using QuanLyNhaHang.Model;
using QuanLyNhaHang.View;
using QuanLyNhaHang.Utilities; // Thêm namespace này để dùng BillPrinter
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace QuanLyNhaHang.ViewModel
{
    public class TableViewModel : BaseViewModel
    {
        // --- 1. KHAI BÁO DỮ LIỆU ---
        private ObservableCollection<Table> _ListTable;
        public ObservableCollection<Table> ListTable
        {
            get => _ListTable;
            set { _ListTable = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Category> _ListCategory;
        public ObservableCollection<Category> ListCategory
        {
            get => _ListCategory;
            set { _ListCategory = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Food> _ListFood;
        public ObservableCollection<Food> ListFood
        {
            get => _ListFood;
            set { _ListFood = value; OnPropertyChanged(); }
        }

        private ObservableCollection<BillMenu> _ListBillInfo;
        public ObservableCollection<BillMenu> ListBillInfo
        {
            get => _ListBillInfo;
            set { _ListBillInfo = value; OnPropertyChanged(); }
        }

        private string _DiscountCode;
        public string DiscountCode { get => _DiscountCode; set { _DiscountCode = value; OnPropertyChanged(); } }

        private decimal _DiscountAmountMoney;
        public decimal DiscountAmountMoney { get => _DiscountAmountMoney; set { _DiscountAmountMoney = value; OnPropertyChanged(); } }

        private decimal _FinalTotalPrice;
        public decimal FinalTotalPrice { get => _FinalTotalPrice; set { _FinalTotalPrice = value; OnPropertyChanged(); } }

        private BillMenu _SelectedBillDetail;
        public BillMenu SelectedBillDetail { get => _SelectedBillDetail; set { _SelectedBillDetail = value; OnPropertyChanged(); } }

        private Promotion _SelectedPromotion;

        // --- 2. TRẠNG THÁI ĐƯỢC CHỌN ---
        private Table _SelectedTable;
        public Table SelectedTable
        {
            get => _SelectedTable;
            set
            {
                _SelectedTable = value;
                OnPropertyChanged();
                if (_SelectedTable != null) LoadBill(_SelectedTable.Id);
            }
        }

        private Category _SelectedCategory;
        public Category SelectedCategory
        {
            get => _SelectedCategory;
            set
            {
                _SelectedCategory = value;
                OnPropertyChanged();
                if (_SelectedCategory != null) LoadFoodListByCategoryId(_SelectedCategory.Id);
            }
        }

        private Food _SelectedFood;
        public Food SelectedFood
        {
            get => _SelectedFood;
            set { _SelectedFood = value; OnPropertyChanged(); }
        }

        private int _Amount = 1;
        public int Amount
        {
            get => _Amount;
            set { _Amount = value; OnPropertyChanged(); }
        }

        private decimal _TotalPrice;
        public decimal TotalPrice
        {
            get => _TotalPrice;
            set { _TotalPrice = value; OnPropertyChanged(); }
        }

        // --- 3. COMMAND ---
        public ICommand AddFoodCommand { get; set; }
        public ICommand PayCommand { get; set; }
        public ICommand ApplyDiscountCommand { get; set; }
        public ICommand RemoveFoodCommand { get; set; }
        public ICommand BookTableCommand { get; set; }
        public ICommand CheckInTableCommand { get; set; } // <--- MỚI THÊM
        public ICommand SwitchTableCommand { get; set; }

        public TableViewModel()
        {
            LoadTable();
            LoadCategory();

            // --- XỬ LÝ NÚT THÊM MÓN ---
            AddFoodCommand = new RelayCommand<object>((p) =>
            {
                return SelectedTable != null && SelectedFood != null;
            }, (p) =>
            {
                if (SelectedTable == null || SelectedFood == null) return;

                try
                {
                    int idBill = GetUncheckBillId(SelectedTable.Id);
                    int idFood = SelectedFood.Id;
                    int count = Amount;

                    if (idBill == -1) // Tạo hóa đơn mới
                    {
                        DataProvider.Ins.ExecuteNonQuery("INSERT INTO Bills (DateCheckIn, DateCheckOut, TableId, Status) VALUES (GETDATE(), NULL, " + SelectedTable.Id + ", 0)");

                        string getMaxIdQuery = "SELECT MAX(Id) FROM Bills";
                        object result = DataProvider.Ins.ExecuteScalar(getMaxIdQuery);

                        int idBillNew = 1;
                        if (result != null && result != DBNull.Value) idBillNew = Convert.ToInt32(result);

                        DataProvider.Ins.ExecuteNonQuery($"INSERT INTO BillInfos (BillId, FoodId, Count, CurrentPrice) VALUES ({idBillNew}, {idFood}, {count}, {SelectedFood.Price})");
                        DataProvider.Ins.ExecuteNonQuery($"UPDATE DiningTable SET Status = N'Có người' WHERE Id = {SelectedTable.Id}");
                    }
                    else // Thêm món vào hóa đơn cũ
                    {
                        string checkFoodQuery = $"SELECT * FROM BillInfos WHERE BillId = {idBill} AND FoodId = {idFood}";
                        var checkResult = DataProvider.Ins.ExecuteQuery(checkFoodQuery);

                        if (checkResult.Rows.Count > 0)
                        {
                            DataProvider.Ins.ExecuteNonQuery($"UPDATE BillInfos SET Count = Count + {count} WHERE BillId = {idBill} AND FoodId = {idFood}");
                        }
                        else
                        {
                            DataProvider.Ins.ExecuteNonQuery($"INSERT INTO BillInfos (BillId, FoodId, Count, CurrentPrice) VALUES ({idBill}, {idFood}, {count}, {SelectedFood.Price})");
                        }
                    }

                    int tableId = SelectedTable.Id;
                    LoadTable();
                    foreach (var t in ListTable) { if (t.Id == tableId) { SelectedTable = t; break; } }
                    LoadBill(tableId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message + "\n" + ex.StackTrace);
                }
            });

            // --- ÁP DỤNG MÃ GIẢM GIÁ ---
            ApplyDiscountCommand = new RelayCommand<object>((p) =>
            {
                return !string.IsNullOrEmpty(DiscountCode) && SelectedTable != null;
            }, (p) =>
            {
                string query = $"SELECT * FROM Promotions WHERE Id = '{DiscountCode}' AND Status = 'Active' AND GETDATE() BETWEEN StartDate AND EndDate";
                DataTable data = DataProvider.Ins.ExecuteQuery(query);

                if (data.Rows.Count > 0)
                {
                    _SelectedPromotion = new Promotion(data.Rows[0]);

                    if (_SelectedPromotion.DiscountType == "Percent")
                        DiscountAmountMoney = TotalPrice * (_SelectedPromotion.DiscountValue / 100);
                    else
                        DiscountAmountMoney = _SelectedPromotion.DiscountValue;

                    FinalTotalPrice = TotalPrice - DiscountAmountMoney;
                    if (FinalTotalPrice < 0) FinalTotalPrice = 0;

                    MessageBox.Show($"Áp dụng mã {_SelectedPromotion.Name} thành công!\nGiảm: {DiscountAmountMoney:N0} VNĐ");
                }
                else
                {
                    MessageBox.Show("Mã giảm giá không tồn tại hoặc đã hết hạn!");
                    DiscountAmountMoney = 0;
                    FinalTotalPrice = TotalPrice;
                    _SelectedPromotion = null;
                }
            });

            // --- THANH TOÁN ---
            PayCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedTable == null) return false;
                int idBill = GetUncheckBillId(SelectedTable.Id);
                return idBill != -1;
            }, (p) =>
            {
                int idBill = GetUncheckBillId(SelectedTable.Id);
                if (_SelectedPromotion == null) FinalTotalPrice = TotalPrice;

                if (MessageBox.Show($"Thanh toán bàn {SelectedTable.Name}?\nTổng: {TotalPrice:N0}\nGiảm: {DiscountAmountMoney:N0}\nCần trả: {FinalTotalPrice:N0}",
                    "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        string promoId = _SelectedPromotion != null ? $"'{_SelectedPromotion.Id}'" : "NULL";

                        string queryUpdateBill = $"UPDATE Bills SET DateCheckOut = GETDATE(), Status = 1, " +
                                                 $"PromoId = {promoId}, " +
                                                 $"DiscountAmount = {DiscountAmountMoney}, " +
                                                 $"TotalPrice = {FinalTotalPrice} " +
                                                 $"WHERE Id = {idBill}";

                        DataProvider.Ins.ExecuteNonQuery(queryUpdateBill);
                        DataProvider.Ins.ExecuteNonQuery($"UPDATE DiningTable SET Status = N'Trống' WHERE Id = {SelectedTable.Id}");

                        // In hóa đơn
                       
                        MessageBox.Show("Thanh toán thành công!");

                        int tableId = SelectedTable.Id;
                        LoadTable();
                        LoadBill(tableId);

                        TotalPrice = 0;
                        FinalTotalPrice = 0;
                        DiscountAmountMoney = 0;
                        DiscountCode = "";
                        _SelectedPromotion = null;
                    }
                    catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
                }
            });

            // --- XÓA / GIẢM MÓN ---
            RemoveFoodCommand = new RelayCommand<object>((p) =>
            {
                return SelectedTable != null && SelectedBillDetail != null;
            }, (p) =>
            {
                try
                {
                    int idBill = GetUncheckBillId(SelectedTable.Id);
                    string foodName = SelectedBillDetail.FoodName;
                    int foodId = (int)DataProvider.Ins.ExecuteScalar($"SELECT Id FROM Food WHERE Name = N'{foodName}'");

                    DataProvider.Ins.ExecuteNonQuery($"UPDATE BillInfos SET Count = Count - 1 WHERE BillId = {idBill} AND FoodId = {foodId}");
                    DataProvider.Ins.ExecuteNonQuery($"DELETE FROM BillInfos WHERE Count <= 0 AND BillId = {idBill}");

                    LoadBill(SelectedTable.Id);
                }
                catch { }
            });

            // --- ĐẶT BÀN ---
            BookTableCommand = new RelayCommand<object>((p) =>
            {
                var table = p as Table;
                return table != null && table.Status == "Trống";
            }, (p) =>
            {
                var table = p as Table;
                View.ReservationWindow window = new View.ReservationWindow();
                var vm = window.DataContext as ReservationViewModel;
                if (vm != null) vm.CurrentTableId = table.Id;

                window.ShowDialog();
                LoadTable();
            });

            // --- NHẬN BÀN (CHECK-IN) --- (Đã bổ sung)
            CheckInTableCommand = new RelayCommand<object>((p) =>
            {
                var table = p as Table;
                if (table == null) return false;
                // So sánh chữ thường để tránh lỗi chính tả
                return table.Status.Trim().ToLower() == "đặt trước";
            }, (p) =>
            {
                var table = p as Table;
                if (MessageBox.Show($"Khách đã đến nhận bàn {table.Name}?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        // 1. Đổi trạng thái bàn
                        DataProvider.Ins.ExecuteNonQuery($"UPDATE DiningTable SET Status = N'Có người' WHERE Id = {table.Id}");
                        // 2. Hoàn thành đơn đặt
                        DataProvider.Ins.ExecuteNonQuery($"UPDATE Reservations SET Status = 'Completed' WHERE TableId = {table.Id} AND Status = N'Chờ xác nhận'");
                        // 3. Tạo hóa đơn rỗng
                        int existingBill = (int)DataProvider.Ins.ExecuteScalar($"SELECT COUNT(*) FROM Bills WHERE TableId = {table.Id} AND Status = 0");
                        if (existingBill == 0)
                        {
                            DataProvider.Ins.ExecuteNonQuery($"INSERT INTO Bills (DateCheckIn, DateCheckOut, TableId, Status) VALUES (GETDATE(), NULL, {table.Id}, 0)");
                        }

                        LoadTable();

                        // Tự động chọn bàn đó
                        foreach (var t in ListTable) { if (t.Id == table.Id) { SelectedTable = t; break; } }
                    }
                    catch (Exception ex) { MessageBox.Show("Lỗi check-in: " + ex.Message); }
                }
            });

            // --- CHUYỂN BÀN ---
            SwitchTableCommand = new RelayCommand<object>((p) =>
            {
                var table = p as Table;
                return table != null && table.Status == "Có người";
            }, (p) =>
            {
                var currentTable = p as Table;
                View.SwitchTableWindow switchWindow = new View.SwitchTableWindow();

                if (switchWindow.ShowDialog() == true)
                {
                    Table targetTable = switchWindow.SelectedTargetTable;
                    try
                    {
                        int idBill = GetUncheckBillId(currentTable.Id);
                        DataProvider.Ins.ExecuteNonQuery($"UPDATE Bills SET TableId = {targetTable.Id} WHERE Id = {idBill}");
                        DataProvider.Ins.ExecuteNonQuery($"UPDATE DiningTable SET Status = N'Trống' WHERE Id = {currentTable.Id}");
                        DataProvider.Ins.ExecuteNonQuery($"UPDATE DiningTable SET Status = N'Có người' WHERE Id = {targetTable.Id}");

                        LoadTable();

                        if (SelectedTable != null && SelectedTable.Id == currentTable.Id)
                        {
                            ListBillInfo.Clear();
                            TotalPrice = 0;
                        }

                        MessageBox.Show($"Đã chuyển từ {currentTable.Name} sang {targetTable.Name} thành công!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi chuyển bàn: " + ex.Message);
                    }
                }
            });
        }


        // --- 4. CÁC HÀM HỖ TRỢ ---
        void LoadTable()
        {
            ListTable = new ObservableCollection<Table>();
            DataTable data = DataProvider.Ins.ExecuteQuery("SELECT * FROM DiningTable");
            foreach (DataRow item in data.Rows) ListTable.Add(new Table(item));
        }

        void LoadCategory()
        {
            ListCategory = new ObservableCollection<Category>();
            DataTable data = DataProvider.Ins.ExecuteQuery("SELECT * FROM FoodCategory");
            foreach (DataRow item in data.Rows) ListCategory.Add(new Category(item));
        }

        void LoadFoodListByCategoryId(int id)
        {
            ListFood = new ObservableCollection<Food>();
            DataTable data = DataProvider.Ins.ExecuteQuery("SELECT * FROM Food WHERE CategoryId = " + id);
            foreach (DataRow item in data.Rows) ListFood.Add(new Food(item));
        }

        int GetUncheckBillId(int idTable)
        {
            string query = "SELECT * FROM Bills WHERE TableId = " + idTable + " AND Status = 0";
            DataTable data = DataProvider.Ins.ExecuteQuery(query);
            if (data.Rows.Count > 0) return (int)data.Rows[0]["Id"];
            return -1;
        }

        void LoadBill(int idBan)
        {
            ListBillInfo = new ObservableCollection<BillMenu>();
            decimal total = 0;

            string query = "SELECT f.Name, bi.Count, bi.CurrentPrice AS Price, bi.CurrentPrice * bi.Count AS TotalPrice " +
                           "FROM BillInfos AS bi, Bills AS b, Food AS f " +
                           "WHERE bi.BillId = b.Id AND bi.FoodId = f.Id AND b.Status = 0 AND b.TableId = " + idBan;

            DataTable data = DataProvider.Ins.ExecuteQuery(query);
            foreach (DataRow item in data.Rows)
            {
                BillMenu info = new BillMenu(item);
                ListBillInfo.Add(info);
                total += info.TotalPrice;
            }

            TotalPrice = total;
            FinalTotalPrice = total;
            DiscountAmountMoney = 0;
            DiscountCode = "";
            _SelectedPromotion = null;
        }
    }
}