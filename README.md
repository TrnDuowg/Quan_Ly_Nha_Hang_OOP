#  Hệ Thống Quản Lý Nhà Hàng (Restaurant Management System)

> Bài tập lớn môn Lập trình Hướng đối tượng (OOP)
> 
> **Công nghệ:** C# WPF - MVVM - SQL Server


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


# 🛠️ Hướng dẫn Cài đặt và Vận hành (Installation Guide)

Tài liệu này hướng dẫn chi tiết cách cài đặt môi trường, cơ sở dữ liệu và cấu hình để chạy phần mềm **Quản lý Nhà hàng**.

---

## 1. Yêu cầu hệ thống (Prerequisites)
Trước khi cài đặt, máy tính cần đáp ứng:
* **Hệ điều hành:** Windows 10 hoặc Windows 11 (64-bit).
* **Cơ sở dữ liệu:** Microsoft SQL Server (2012 trở lên).
* **Môi trường chạy:** [.NET Desktop Runtime 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (Nếu chạy file exe) hoặc **Visual Studio 2022** (Nếu chạy source code).

---

## 2. Quy trình Cài đặt (Step-by-Step)

### Bước 1: Khởi tạo Cơ sở dữ liệu (Database)
 *Phần mềm sẽ báo lỗi và tự tắt nếu không kết nối được Database.*

1.  Tìm file script SQL: **`Database_Setup.sql`** (nằm trong thư mục gốc của bộ cài hoặc source code).
2.  Mở **SQL Server Management Studio (SSMS)** và kết nối vào Server của bạn.
3.  Kéo file `Database_Setup.sql` vào giao diện SSMS (hoặc chọn *File > Open*).
4.  Nhấn nút **Execute (F5)** để chạy script.
    * **Kết quả:** Database `QuanLyNhaHangOOP` được tạo cùng với dữ liệu mẫu.

### Bước 2: Cài đặt Ứng dụng
* **Cách 1 (Dùng bộ cài):** Truy cập vào thư mục **[Cài chương trình](./Cài%20chương%20trình)**, chạy file `setup.exe` và nhấn Next liên tục để cài đặt.
* **Cách 2 (Dùng Source Code):** Mở file `QuanLyNhaHang.sln` bằng Visual Studio -> Nhấn **F5** để Build và Run.

### Bước 3: Cấu hình Kết nối (Quan trọng)
Do tên Server SQL của mỗi máy khác nhau, bạn cần cập nhật file cấu hình.

1.  Vào thư mục đã cài đặt phần mềm (Thường là: `C:\Program Files (x86)\Default Company Name\QuanLyNhaHang_Setup`).
2.  Tìm file có đuôi `.config` (Ví dụ: `QuanLyNhaHang.dll.config` hoặc `App.config`).
3.  Mở file bằng **Notepad**.
4.  Tìm đoạn code `connectionStrings` và sửa như sau:

    ```xml
    <connectionStrings>
        <add name="QuanLyNhaHang" 
             connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=QuanLyNhaHangOOP;Integrated Security=True" 
             providerName="System.Data.SqlClient"/>
    </connectionStrings>
    ```

5.  Sửa phần `Data Source=...` thành tên Server SQL của bạn.
    * Ví dụ: `Data Source=(local)` hoặc `Data Source=TEN_MAY_TINH`.
6.  Lưu file lại (Ctrl + S).

---

## 3. Hướng dẫn Vận hành (User Manual)

###  Tài khoản Đăng nhập (Mặc định)
Mật khẩu chung cho tất cả tài khoản là: **1**

| Vai trò | Username | Chức năng |
| :--- | :--- | :--- |
| **Quản trị viên** | `admin` | Toàn quyền hệ thống (Quản lý Nhân viên, Thực đơn, Khuyến mãi, Thống kê, Bán hàng) |
| **Nhân viên** | `staff` | Chỉ truy cập chức năng Bán hàng (Đặt bàn, Gọi món, Thanh toán) |
| **Khách hàng** | `guest` | Chế độ Kiosk tại bàn (Chỉ xem Menu và tự Gọi món) |

###  Quy trình Bán hàng chuẩn (Workflow)

* **Đặt bàn:** Chuột phải vào bàn Trống (Xanh) -> Chọn **Đặt bàn**.
* **Nhận khách:** Chuột phải vào bàn Đặt trước (Cam) -> Chọn **Khách nhận bàn**.
* **Gọi món:**
    1.  Click chuột trái vào bàn Có người (Đỏ).
    2.  Chọn món và số lượng -> Bấm **Thêm món**.
* **Thanh toán:**
    1.  Chọn bàn cần thanh toán.
    2.  Nhập mã khuyến mãi (nếu có, VD: `KM10`, `TET50K`).
    3.  Bấm **Thanh toán**.

---

##  Xử lý sự cố thường gặp

### Lỗi Crash ngay khi mở App:
* **Nguyên nhân:** Chưa cấu hình đúng chuỗi kết nối SQL.
* **Khắc phục:** Xem lại **Bước 3** phần Cài đặt.

### Lỗi không hiện Menu quản lý:
* **Nguyên nhân:** Đang đăng nhập bằng tài khoản Nhân viên (staff).
* **Khắc phục:** Đăng xuất và đăng nhập lại bằng `admin`.
