using QuanLyNhaHang.Model;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace QuanLyNhaHang.ViewModel
{
    public class PromotionViewModel : BaseViewModel
    {
        // --- 1. KHAI BÁO DỮ LIỆU ---
        private ObservableCollection<Promotion> _ListPromotion;
        public ObservableCollection<Promotion> ListPromotion { get => _ListPromotion; set { _ListPromotion = value; OnPropertyChanged(); } }

        private string _Id;
        public string Id { get => _Id; set { _Id = value; OnPropertyChanged(); } }

        private string _Name;
        public string Name { get => _Name; set { _Name = value; OnPropertyChanged(); } }

        private decimal _DiscountValue;
        public decimal DiscountValue { get => _DiscountValue; set { _DiscountValue = value; OnPropertyChanged(); } }

        private string _DiscountType = "Percent";
        public string DiscountType { get => _DiscountType; set { _DiscountType = value; OnPropertyChanged(); } }

        private DateTime _StartDate = DateTime.Now;
        public DateTime StartDate { get => _StartDate; set { _StartDate = value; OnPropertyChanged(); } }

        private DateTime _EndDate = DateTime.Now.AddDays(30);
        public DateTime EndDate { get => _EndDate; set { _EndDate = value; OnPropertyChanged(); } }

        private Promotion _SelectedPromotion;
        public Promotion SelectedPromotion
        {
            get => _SelectedPromotion;
            set
            {
                _SelectedPromotion = value;
                OnPropertyChanged();
                if (SelectedPromotion != null)
                {
                    Id = SelectedPromotion.Id;
                    Name = SelectedPromotion.Name;
                    DiscountValue = SelectedPromotion.DiscountValue;
                    DiscountType = SelectedPromotion.DiscountType;
                    StartDate = SelectedPromotion.StartDate;
                    EndDate = SelectedPromotion.EndDate;
                }
            }
        }

        // --- 2. COMMAND ---
        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ClearCommand { get; set; }

        // --- 3. CONSTRUCTOR ---
        public PromotionViewModel()
        {
            // QUAN TRỌNG: Bọc LoadList trong try-catch để tránh crash app khi khởi động
            try
            {
                LoadList();
            }
            catch (Exception ex)
            {
                // Nếu lỗi DB, chỉ hiện thông báo, không tắt app
                // MessageBox.Show("Lỗi tải khuyến mãi: " + ex.Message); 
                // Có thể comment dòng trên lại nếu không muốn hiện popup khi mới mở app
                ListPromotion = new ObservableCollection<Promotion>(); // Khởi tạo list rỗng để không bị null
            }

            // Lệnh THÊM MỚI
            AddCommand = new RelayCommand<object>((p) =>
            {
                return !string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(Name);
            }, (p) =>
            {
                try
                {
                    var check = DataProvider.Ins.ExecuteQuery($"SELECT * FROM Promotions WHERE Id = '{Id}'");
                    if (check.Rows.Count > 0)
                    {
                        MessageBox.Show("Mã khuyến mãi này đã tồn tại!");
                        return;
                    }

                    string query = $"INSERT INTO Promotions (Id, Name, DiscountValue, DiscountType, StartDate, EndDate, Status) " +
                                   $"VALUES ('{Id}', N'{Name}', {DiscountValue}, '{DiscountType}', '{StartDate:yyyy-MM-dd}', '{EndDate:yyyy-MM-dd}', 'Active')";

                    DataProvider.Ins.ExecuteNonQuery(query);
                    MessageBox.Show("Thêm thành công!");
                    LoadList();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi thêm: " + ex.Message); }
            });

            // Lệnh XÓA (Khóa)
            DeleteCommand = new RelayCommand<object>((p) => SelectedPromotion != null, (p) =>
            {
                if (MessageBox.Show("Bạn có chắc muốn dừng chương trình này?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        DataProvider.Ins.ExecuteNonQuery($"UPDATE Promotions SET Status = 'Inactive' WHERE Id = '{SelectedPromotion.Id}'");
                        LoadList();
                    }
                    catch (Exception ex) { MessageBox.Show("Lỗi xóa: " + ex.Message); }
                }
            });

            // Lệnh LÀM MỚI
            ClearCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                Id = ""; Name = ""; DiscountValue = 0;
                StartDate = DateTime.Now; EndDate = DateTime.Now.AddDays(30);
                SelectedPromotion = null;
            });
        }

        void LoadList()
        {
            ListPromotion = new ObservableCollection<Promotion>();

            // Kiểm tra kết nối trước khi load
            string query = "SELECT * FROM Promotions WHERE Status = 'Active'";
            var data = DataProvider.Ins.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                ListPromotion.Add(new Promotion(item));
            }
        }
    }
}