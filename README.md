#  Hệ Thống Quản Lý Nhà Hàng (Restaurant Management System)

> Bài tập lớn môn Lập trình Hướng đối tượng (OOP)
> 
> **Công nghệ:** C# WPF - MVVM - SQL Server

# Thông tin nhóm sinh viên

| STT | Họ và tên | Mã sinh viên |
|:---:|---|:---:|
| 1 | **Trần Đoàn Hoàng Anh** | 20237297 |
| 2 | **Trần Công Dương** | 20237321 |

##  Giới thiệu
Phần mềm quản lý vận hành nhà hàng toàn diện, hỗ trợ quy trình từ đặt bàn, gọi món, thanh toán đến quản lý nhân sự và báo cáo doanh thu. Dự án được xây dựng dựa trên kiến trúc **MVVM (Model-View-ViewModel)** giúp tách biệt giao diện và xử lý logic, đồng thời áp dụng triệt để các nguyên lý **OOP** và các **Mẫu thiết kế (Design Patterns)**.

---

##  Tính năng Chính

### 1. Hệ thống & Bảo mật
*   **Đăng nhập/Đăng xuất:** Bảo mật với mật khẩu.
*   **Phân quyền (RBAC):**
    *   **Admin:** Toàn quyền hệ thống.
    *   **Staff:** Chỉ truy cập các chức năng bán hàng.
    *   **Customer:** Chế độ Kiosk tự gọi món.
*   **Quản lý Tài khoản:** Cập nhật thông tin cá nhân, đổi mật khẩu an toàn.

### 2. Nghiệp vụ Bán hàng (POS)
*   **Quản lý Sơ đồ bàn:** Hiển thị trạng thái bàn trực quan bằng màu sắc:
    *   🟩 **Xanh:** Bàn trống.
    *   🟥 **Đỏ:** Có người đang ăn.
    *   🟧 **Cam:** Đặt trước.
*   **Đặt bàn (Reservation):** Đặt chỗ trước cho khách, tự động đổi trạng thái bàn.
*   **Gọi món (Ordering):** Thêm món, tìm kiếm món ăn, bớt món/hủy món (qua menu chuột phải).
*   **Chuyển bàn:** Hỗ trợ khách đổi chỗ ngồi, chuyển toàn bộ đơn hàng sang bàn mới.
*   **Thanh toán:** Tính tổng tiền, áp dụng mã giảm giá.

### 3. Quản lý (Admin)
*   **Quản lý Thực đơn:** Thêm/Sửa/Xóa món ăn, danh mục.
*   **Quản lý Nhân sự:** Thêm tài khoản nhân viên, phân quyền.
*   **Quản lý Khuyến mãi:** Tạo mã Voucher giảm giá theo % hoặc số tiền.
*   **Báo cáo Thống kê:** Xem doanh thu theo ngày/tháng, xem lại lịch sử chi tiết từng hóa đơn.

---

##  Công nghệ & Kỹ thuật

### Tech Stack
*   **Language:** C# (.NET 6.0 / .NET 8.0)
*   **UI Framework:** WPF (Windows Presentation Foundation)
*   **UI Library:** MaterialDesignThemes (Giao diện hiện đại)
*   **Database:** Microsoft SQL Server
*   **IDE:** Visual Studio 2022

### Áp dụng OOP & Design Patterns
Dự án áp dụng các kiến thức cốt lõi của môn học:
1.  **Mô hình MVVM:** Tách biệt Model, View, ViewModel. Không viết code xử lý trong code-behind (.xaml.cs).
2.  **Singleton Pattern:** Áp dụng cho lớp `DataProvider` để quản lý kết nối cơ sở dữ liệu duy nhất.
3.  **Command Pattern:** Sử dụng `RelayCommand` để xử lý sự kiện thay vì Event Handler truyền thống.
4.  **Observer Pattern:** Sử dụng `INotifyPropertyChanged` để cập nhật giao diện thời gian thực.
5.  **Tính Đóng gói, Kế thừa, Đa hình:** Thể hiện qua cấu trúc các lớp `User` (Cha) -> `Employee`, `Customer` (Con).
