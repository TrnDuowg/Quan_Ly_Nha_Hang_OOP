using QuanLyNhaHang.DTO;
using QuanLyNhaHang.Model;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace QuanLyNhaHang.ViewModel
{
    public class BillDetailViewModel : BaseViewModel
    {
        // Danh sách món ăn trong hóa đơn đó
        private ObservableCollection<BillMenu> _ListBillInfo;
        public ObservableCollection<BillMenu> ListBillInfo { get => _ListBillInfo; set { _ListBillInfo = value; OnPropertyChanged(); } }

        // Thông tin chung
        public string BillId { get; set; }
        public string TableName { get; set; }
        public string DateCheckOut { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal FinalPrice { get; set; }

        public ICommand CloseCommand { get; set; }

        public BillDetailViewModel(BillStatistic selectedBill)
        {
            // Nhận dữ liệu từ dòng được chọn bên màn hình Báo cáo
            BillId = selectedBill.Id.ToString();
            TableName = selectedBill.TableName;
            DateCheckOut = selectedBill.DateCheckOut.ToString("dd/MM/yyyy HH:mm");
            FinalPrice = selectedBill.TotalPrice;
            Discount = selectedBill.Discount;
            TotalPrice = FinalPrice + Discount; // Tính ngược lại tổng tiền gốc

            LoadBillInfo(selectedBill.Id);

            CloseCommand = new RelayCommand<Window>((p) => true, (p) => p.Close());
        }

        void LoadBillInfo(int idBill)
        {
            ListBillInfo = new ObservableCollection<BillMenu>();

            // Query lấy chi tiết món ăn của hóa đơn lịch sử (Đã thanh toán)
            string query =
                "SELECT f.Name, bi.Count, bi.CurrentPrice AS Price, bi.CurrentPrice * bi.Count AS TotalPrice " +
                "FROM BillInfos AS bi, Food AS f " +
                "WHERE bi.FoodId = f.Id AND bi.BillId = " + idBill;

            DataTable data = DataProvider.Ins.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                ListBillInfo.Add(new BillMenu(item));
            }
        }
    }
}