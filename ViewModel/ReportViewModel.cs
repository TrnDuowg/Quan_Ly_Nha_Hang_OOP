using QuanLyNhaHang.DTO;
using QuanLyNhaHang.Model;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;

namespace QuanLyNhaHang.ViewModel
{
    public class ReportViewModel : BaseViewModel
    {
        // Danh sách hóa đơn thống kê
        private ObservableCollection<BillStatistic> _ListBill;
        public ObservableCollection<BillStatistic> ListBill { get => _ListBill; set { _ListBill = value; OnPropertyChanged(); } }

        // Ngày bắt đầu và kết thúc
        private DateTime _FromDate;
        public DateTime FromDate { get => _FromDate; set { _FromDate = value; OnPropertyChanged(); } }

        private DateTime _ToDate;
        public DateTime ToDate { get => _ToDate; set { _ToDate = value; OnPropertyChanged(); } }

        // Tổng doanh thu hiển thị
        private decimal _SumRevenue;
        public decimal SumRevenue { get => _SumRevenue; set { _SumRevenue = value; OnPropertyChanged(); } }

        // Chi tiết hóa đơn
        private BillStatistic _SelectedBill;
        public BillStatistic SelectedBill { get => _SelectedBill; set { _SelectedBill = value; OnPropertyChanged(); } }

        // Lệnh xem
        public ICommand ViewCommand { get; set; }
        public ICommand DoubleClickCommand { get; set; }
        public ReportViewModel()
        {
            // Mặc định load tháng hiện tại
            FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            ToDate = DateTime.Now;

            LoadRevenue(); // Load lần đầu

            ViewCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                LoadRevenue();
            });

            DoubleClickCommand = new RelayCommand<object>((p) => SelectedBill != null, (p) =>
            {
                // Mở cửa sổ chi tiết
                View.BillDetailWindow window = new View.BillDetailWindow();

                // Truyền hóa đơn đang chọn sang ViewModel của cửa sổ con
                window.DataContext = new BillDetailViewModel(SelectedBill);

                window.ShowDialog();
            });
        }

        void LoadRevenue()
        {
            ListBill = new ObservableCollection<BillStatistic>();
            decimal total = 0;

            // Query lấy hóa đơn đã thanh toán (Status=1) trong khoảng thời gian
            // Join bảng Bills và DiningTable để lấy tên bàn
            string query = "SELECT b.Id, t.Name AS TableName, b.DateCheckOut, b.TotalPrice, b.DiscountAmount " +
                           "FROM Bills AS b, DiningTable AS t " +
                           "WHERE b.TableId = t.Id " +
                           "AND b.Status = 1 " +
                           $"AND b.DateCheckOut >= '{FromDate:yyyy-MM-dd} 00:00:00' " +
                           $"AND b.DateCheckOut <= '{ToDate:yyyy-MM-dd} 23:59:59'";

            DataTable data = DataProvider.Ins.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                BillStatistic bill = new BillStatistic(item);
                ListBill.Add(bill);
                total += bill.TotalPrice;
            }

            SumRevenue = total;
        }
    }
}