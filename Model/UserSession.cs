using QuanLyNhaHang.Model;

namespace QuanLyNhaHang
{
    // Class tĩnh (static) để truy cập từ bất cứ đâu
    public static class UserSession
    {
        // Biến lưu thông tin người đang đăng nhập hiện tại
        public static User CurrentUser { get; set; }
    }
}