using QuanLyNhaHang.Model;
using System;
using System.Windows;
using System.Windows.Input;

namespace QuanLyNhaHang.ViewModel
{
    public class ReservationViewModel : BaseViewModel
    {
        // --- CÁC BIẾN BINDING DỮ LIỆU ---
        private string _CustomerName;
        public string CustomerName { get => _CustomerName; set { _CustomerName = value; OnPropertyChanged(); } }

        private string _PhoneNumber;
        public string PhoneNumber { get => _PhoneNumber; set { _PhoneNumber = value; OnPropertyChanged(); } }

        private DateTime _BookingDate = DateTime.Now;
        public DateTime BookingDate { get => _BookingDate; set { _BookingDate = value; OnPropertyChanged(); } }

        private string _BookingTime = DateTime.Now.ToString("HH:mm");
        public string BookingTime { get => _BookingTime; set { _BookingTime = value; OnPropertyChanged(); } }

        private int _GuestCount = 2;
        public int GuestCount { get => _GuestCount; set { _GuestCount = value; OnPropertyChanged(); } }

        // Biến lưu ID bàn đang được chọn để đặt (được truyền từ TableViewModel sang)
        public int CurrentTableId { get; set; }

        // --- COMMANDS ---
        public ICommand ConfirmCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public ReservationViewModel()
        {
            // Lệnh XÁC NHẬN
            ConfirmCommand = new RelayCommand<Window>((p) =>
            {
                // Điều kiện: Phải nhập tên và sđt
                return !string.IsNullOrEmpty(CustomerName) && !string.IsNullOrEmpty(PhoneNumber);
            }, (p) =>
            {
                try
                {
                    // 1. Xử lý gộp Ngày + Giờ
                    DateTime fullDateTime;
                    if (!DateTime.TryParse($"{BookingDate:yyyy-MM-dd} {BookingTime}", out fullDateTime))
                    {
                        MessageBox.Show("Định dạng giờ không hợp lệ! Vui lòng nhập kiểu HH:mm (VD: 18:30)");
                        return;
                    }

                    // 2. Lưu vào Database (Bảng Reservations)
                    string queryInsert = $"INSERT INTO Reservations (CustomerName, PhoneNumber, BookingTime, GuestCount, TableId, Status) " +
                                         $"VALUES (N'{CustomerName}', '{PhoneNumber}', '{fullDateTime:yyyy-MM-dd HH:mm}', {GuestCount}, {CurrentTableId}, N'Chờ xác nhận')";

                    DataProvider.Ins.ExecuteNonQuery(queryInsert);

                    // 3. Cập nhật trạng thái Bàn thành 'Đặt trước'
                    string queryUpdateTable = $"UPDATE DiningTable SET Status = N'Đặt trước' WHERE Id = {CurrentTableId}";
                    DataProvider.Ins.ExecuteNonQuery(queryUpdateTable);

                    MessageBox.Show("Đặt bàn thành công!");

                    // Đóng cửa sổ
                    p?.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            });

            // Lệnh HỦY (Đóng cửa sổ)
            CancelCommand = new RelayCommand<Window>((p) => true, (p) =>
            {
                p?.Close();
            });
        }
    }
}