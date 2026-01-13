using QuanLyNhaHang.Model;
using System;
using System.Data;
using System.Windows.Input;

namespace QuanLyNhaHang.ViewModel
{
    public class HomeViewModel : BaseViewModel
    {
        // --- 1. CÁC BIẾN HIỂN THỊ ---

        // Doanh thu hôm nay
        private decimal _DailyRevenue;
        public decimal DailyRevenue { get => _DailyRevenue; set { _DailyRevenue = value; OnPropertyChanged(); } }

        // Số đơn đã bán hôm nay
        private int _OrderCount;
        public int OrderCount { get => _OrderCount; set { _OrderCount = value; OnPropertyChanged(); } }

        // Số bàn đang có khách / Tổng số bàn
        private string _TableStatusInfo;
        public string TableStatusInfo { get => _TableStatusInfo; set { _TableStatusInfo = value; OnPropertyChanged(); } }

        // Lệnh làm mới dữ liệu
        public ICommand RefreshCommand { get; set; }

        // --- 2. CONSTRUCTOR ---
        public HomeViewModel()
        {
            LoadDashboardData();

            RefreshCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                LoadDashboardData();
            });
        }

        // --- 3. HÀM LOAD DỮ LIỆU TỪ SQL ---
        public void LoadDashboardData()
        {
            try
            {
                string today = DateTime.Now.ToString("yyyy-MM-dd");

                // 1. Tính tổng doanh thu hôm nay
                string queryRevenue = $"SELECT SUM(TotalPrice) FROM Bills WHERE Status = 1 AND CAST(DateCheckOut AS DATE) = '{today}'";
                object resultRevenue = DataProvider.Ins.ExecuteScalar(queryRevenue);
                DailyRevenue = (resultRevenue != DBNull.Value && resultRevenue != null) ? Convert.ToDecimal(resultRevenue) : 0;

                // 2. Đếm số hóa đơn hôm nay
                string queryCount = $"SELECT COUNT(*) FROM Bills WHERE Status = 1 AND CAST(DateCheckOut AS DATE) = '{today}'";
                object resultCount = DataProvider.Ins.ExecuteScalar(queryCount);
                OrderCount = (resultCount != null) ? Convert.ToInt32(resultCount) : 0;

                // 3. Tính trạng thái bàn (Số bàn có người / Tổng bàn)
                string queryTableBusy = "SELECT COUNT(*) FROM DiningTable WHERE Status = N'Có người'";
                int busyCount = (int)DataProvider.Ins.ExecuteScalar(queryTableBusy);

                string queryTableTotal = "SELECT COUNT(*) FROM DiningTable";
                int totalCount = (int)DataProvider.Ins.ExecuteScalar(queryTableTotal);

                TableStatusInfo = $"{busyCount} / {totalCount}";
            }
            catch (Exception)
            {
                // Có thể log lỗi hoặc bỏ qua nếu DB chưa sẵn sàng
                DailyRevenue = 0;
                OrderCount = 0;
                TableStatusInfo = "0 / 0";
            }
        }
    }
}