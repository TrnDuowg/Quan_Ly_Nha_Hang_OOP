-- 1. Tạo Database mới
CREATE DATABASE QuanLyNhaHangOOP;
GO

USE QuanLyNhaHangOOP;
GO

-- =============================================
-- KHỐI 1: QUẢN LÝ NGƯỜI DÙNG (USER & INHERITANCE)
-- Ánh xạ Class User, Admin, NhanVien, KhachHang
-- =============================================

-- Bảng cha: Users (Chứa thông tin chung)
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DisplayName NVARCHAR(100) NOT NULL, -- Tên hiển thị
    UserName VARCHAR(100) NOT NULL UNIQUE, -- Tên đăng nhập
    PasswordHash VARCHAR(1000) NOT NULL, -- Mật khẩu
    PhoneNumber VARCHAR(20),
    UserType NVARCHAR(50) NOT NULL, -- Discriminator: 'Admin', 'Staff', 'Customer'
    CreateDate DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1 -- 1: Hoạt động, 0: Bị khóa
);

-- Bảng con: Employee (Thông tin riêng của Nhân viên)
-- Quan hệ 1-1 với Users (Kế thừa trong DB)
CREATE TABLE Employees (
    UserId INT PRIMARY KEY,
    Position NVARCHAR(100), -- Chức vụ
    Salary DECIMAL(18,0) DEFAULT 0,
    HireDate DATE,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Bảng con: Customer (Thông tin riêng của Khách hàng)
-- Quan hệ 1-1 với Users
CREATE TABLE Customers (
    UserId INT PRIMARY KEY,
    MembershipLevel NVARCHAR(50) DEFAULT 'Standard', -- Hạng thành viên
    Points INT DEFAULT 0, -- Điểm tích lũy
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- =============================================
-- KHỐI 2: QUẢN LÝ THỰC ĐƠN (MENU)
-- =============================================

CREATE TABLE FoodCategory (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL
);

CREATE TABLE Food (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    CategoryId INT NOT NULL,
    Price DECIMAL(18, 0) NOT NULL DEFAULT 0,
    ImagePath NVARCHAR(MAX), -- Đường dẫn ảnh món ăn
    Status NVARCHAR(50) DEFAULT N'Đang bán', -- Đang bán / Ngừng bán
    FOREIGN KEY (CategoryId) REFERENCES FoodCategory(Id)
);

-- =============================================
-- KHỐI 3: QUẢN LÝ BÀN & ĐẶT BÀN
-- =============================================

CREATE TABLE DiningTable (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL, -- Bàn 1, Bàn 2
    Status NVARCHAR(50) NOT NULL DEFAULT N'Trống', -- Trống, Có người, Đặt trước
    Capacity INT DEFAULT 4 -- Số ghế
);

CREATE TABLE Reservations (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CustomerName NVARCHAR(100), -- Tên khách đặt (có thể không cần acc)
    CustomerPhone VARCHAR(20),
    BookingTime DATETIME NOT NULL,
    GuestCount INT,
    TableId INT NULL, -- Có thể chưa xếp bàn ngay
    Status NVARCHAR(50) DEFAULT N'Chờ xác nhận',
    FOREIGN KEY (TableId) REFERENCES DiningTable(Id)
);

-- =============================================
-- KHỐI 4: KHUYẾN MÃI (PROMOTION)
-- =============================================

CREATE TABLE Promotions (
    Id VARCHAR(50) PRIMARY KEY, -- Mã code (VD: TET2025)
    Name NVARCHAR(200),
    DiscountValue DECIMAL(18,0), -- Giá trị giảm
    DiscountType NVARCHAR(20), -- 'Percent' hoặc 'Amount'
    StartDate DATETIME,
    EndDate DATETIME,
    Status NVARCHAR(50) DEFAULT 'Active'
);

-- =============================================
-- KHỐI 5: HÓA ĐƠN (BILL & ORDER)
-- =============================================

CREATE TABLE Bills (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DateCheckIn DATETIME NOT NULL DEFAULT GETDATE(),
    DateCheckOut DATETIME,
    TableId INT NOT NULL,
    StaffId INT NULL, -- Nhân viên thanh toán
    PromoId VARCHAR(50) NULL, -- Mã KM áp dụng
    
    Status INT NOT NULL DEFAULT 0, -- 0: Chưa thanh toán, 1: Đã thanh toán
    TotalPrice DECIMAL(18,0) DEFAULT 0, -- Tổng tiền cuối cùng
    
    FOREIGN KEY (TableId) REFERENCES DiningTable(Id),
    FOREIGN KEY (StaffId) REFERENCES Users(Id),
    FOREIGN KEY (PromoId) REFERENCES Promotions(Id)
);

CREATE TABLE BillInfos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    BillId INT NOT NULL,
    FoodId INT NOT NULL,
    Count INT NOT NULL DEFAULT 0,
    CurrentPrice DECIMAL(18, 0) NOT NULL, -- Lưu giá tại thời điểm bán
    
    FOREIGN KEY (BillId) REFERENCES Bills(Id),
    FOREIGN KEY (FoodId) REFERENCES Food(Id)
);
GO

-- =============================================
-- KHỐI 6: DỮ LIỆU MẪU (SEED DATA)
-- =============================================

-- 1. Tạo Admin (Mật khẩu '1')
INSERT INTO Users (DisplayName, UserName, PasswordHash, UserType) 
VALUES (N'Administrator', 'admin', '1', 'Admin');

-- 2. Tạo Nhân viên (Mật khẩu '1')
INSERT INTO Users (DisplayName, UserName, PasswordHash, UserType) 
VALUES (N'Nhân viên A', 'staff', '1', 'Staff');

-- Lấy ID vừa tạo để thêm vào bảng Employees
DECLARE @StaffId INT = (SELECT Id FROM Users WHERE UserName = 'staff');
INSERT INTO Employees (UserId, Position, Salary, HireDate)
VALUES (@StaffId, N'Phục vụ', 5000000, GETDATE());

-- 3. Tạo Danh mục & Món ăn
INSERT INTO FoodCategory (Name) VALUES (N'Hải sản'), (N'Đồ uống');

INSERT INTO Food (Name, CategoryId, Price) VALUES (N'Mực nướng', 1, 120000);
INSERT INTO Food (Name, CategoryId, Price) VALUES (N'Tôm hấp bia', 1, 200000);
INSERT INTO Food (Name, CategoryId, Price) VALUES (N'Bia Tiger', 2, 20000);

-- 4. Tạo Bàn ăn
INSERT INTO DiningTable (Name, Status) VALUES (N'Bàn 01', N'Trống');
INSERT INTO DiningTable (Name, Status) VALUES (N'Bàn 02', N'Trống');
INSERT INTO DiningTable (Name, Status) VALUES (N'Bàn 03', N'Trống');
INSERT INTO DiningTable (Name, Status) VALUES (N'Bàn 04', N'Trống');
INSERT INTO DiningTable (Name, Status) VALUES (N'Bàn 05', N'Trống');
INSERT INTO DiningTable (Name, Status) VALUES (N'Bàn 06', N'Trống');
INSERT INTO DiningTable (Name, Status) VALUES (N'Bàn 07', N'Trống');
INSERT INTO DiningTable (Name, Status) VALUES (N'Bàn 08', N'Trống');
INSERT INTO DiningTable (Name, Status) VALUES (N'Bàn 09', N'Trống');
INSERT INTO DiningTable (Name, Status) VALUES (N'Bàn 10', N'Trống');

-- 5. Tạo Khuyến mãi
INSERT INTO Promotions (Id, Name, DiscountValue, DiscountType) 
VALUES ('KM10', N'Giảm giá 10%', 10, 'Percent');
INSERT INTO Promotions (Id, Name, DiscountValue, DiscountType, StartDate, EndDate) 
VALUES ('TET50K', N'Lì xì 50k', 50000, 'Amount', '2020-01-01', '2030-01-01');
INSERT INTO Promotions (Id, Name, DiscountValue, DiscountType, StartDate, EndDate)
VALUES ('TEST10', N'Giảm giá test 10%', 10, 'Percent', GETDATE(), GETDATE() + 30);
GO


select * from diningtable

select * from FoodCategory
select * from Promotions

USE QuanLyNhaHangOOP;
GO
-- Tạo user guest với quyền Customer
INSERT INTO Users (DisplayName, UserName, PasswordHash, UserType) 
VALUES (N'Khách Hàng', 'guest', '1', 'Customer'); 
-- Mật khẩu là '1' (đã mã hóa MD5)

INSERT INTO Users (DisplayName, UserName, PasswordHash, UserType, PhoneNumber) 
VALUES (N'Khách hàng', 'guest', '1', 'Customer', '0000000000');

