using System.Windows;
using System.Windows.Input;
using QuanLyNhaHang.Model;
using QuanLyNhaHang.View;

namespace QuanLyNhaHang.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public bool IsLoaded { get; set; } = false;

        private BaseViewModel _currentView;
        public BaseViewModel CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

        // --- KHAI BÁO CÁC VIEWMODEL ---
        public HomeViewModel HomeVM { get; set; }
        public TableViewModel TableVM { get; set; }
        public FoodManagementViewModel FoodVM { get; set; }
        public StaffViewModel StaffVM { get; set; }
        public ReportViewModel ReportVM { get; set; }
        public PromotionViewModel PromotionVM { get; set; }
        public AccountViewModel AccountVM { get; set; }
        public CustomerOrderViewModel CustomerOrderVM { get; set; } // ViewModel cho Khách

        // --- CÁC BIẾN ẨN/HIỆN MENU (PHÂN QUYỀN) ---

        // 1. Quyền Admin: Chỉ Admin thấy (Nhân viên, Thống kê, Khuyến mãi...)
        private Visibility _AdminVisibility;
        public Visibility AdminVisibility
        {
            get => _AdminVisibility;
            set { _AdminVisibility = value; OnPropertyChanged(); }
        }

        // 2. Quyền Nội bộ: Admin + Staff thấy (Trang chủ, Bàn ăn)
        // Khách hàng KHÔNG được thấy cái này
        private Visibility _InternalVisibility;
        public Visibility InternalVisibility
        {
            get => _InternalVisibility;
            set { _InternalVisibility = value; OnPropertyChanged(); }
        }

        // 3. Quyền Khách hàng: Chỉ Khách thấy (Menu gọi món)
        private Visibility _CustomerVisibility;
        public Visibility CustomerVisibility
        {
            get => _CustomerVisibility;
            set { _CustomerVisibility = value; OnPropertyChanged(); }
        }

        // --- COMMANDS ---
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand HomeViewCommand { get; set; }
        public ICommand TableViewCommand { get; set; }
        public ICommand FoodViewCommand { get; set; }
        public ICommand StaffViewCommand { get; set; }
        public ICommand ReportViewCommand { get; set; }
        public ICommand PromotionViewCommand { get; set; }
        public ICommand AccountViewCommand { get; set; }
        public ICommand CustomerOrderViewCommand { get; set; } // Lệnh chuyển trang khách
        public ICommand LogOutCommand { get; set; }

        public MainViewModel()
        {
            // Khởi tạo
            HomeVM = new HomeViewModel();
            TableVM = new TableViewModel();
            FoodVM = new FoodManagementViewModel();
            StaffVM = new StaffViewModel();
            ReportVM = new ReportViewModel();
            PromotionVM = new PromotionViewModel();
            AccountVM = new AccountViewModel();
            CustomerOrderVM = new CustomerOrderViewModel(); // Khởi tạo VM khách

            CurrentView = HomeVM;

            // Mặc định ẩn hết cho an toàn
            AdminVisibility = Visibility.Collapsed;
            InternalVisibility = Visibility.Collapsed;
            CustomerVisibility = Visibility.Collapsed;

            // LOADED: Kiểm tra quyền ngay khi mở
            LoadedWindowCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                IsLoaded = true;
                ApplyRoleVisibility();
            });

            // Navigation Commands
            HomeViewCommand = new RelayCommand<object>((p) => true, (p) => CurrentView = HomeVM);
            TableViewCommand = new RelayCommand<object>((p) => true, (p) => CurrentView = TableVM);
            FoodViewCommand = new RelayCommand<object>((p) => true, (p) => CurrentView = FoodVM);
            StaffViewCommand = new RelayCommand<object>((p) => true, (p) => CurrentView = StaffVM);
            ReportViewCommand = new RelayCommand<object>((p) => true, (p) => CurrentView = ReportVM);
            PromotionViewCommand = new RelayCommand<object>((p) => true, (p) => CurrentView = PromotionVM);
            AccountViewCommand = new RelayCommand<object>((p) => true, (p) => CurrentView = AccountVM);

            // Command chuyển trang khách
            CustomerOrderViewCommand = new RelayCommand<object>((p) => true, (p) => CurrentView = CustomerOrderVM);

            // LOGOUT
            LogOutCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                var w = p as Window;
                if (w == null) return;

                if (MessageBox.Show("Đăng xuất khỏi hệ thống?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    w.Hide();
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.ShowDialog();

                    var loginVM = loginWindow.DataContext as LoginViewModel;
                    if (loginVM != null && loginVM.IsLogin)
                    {
                        ApplyRoleVisibility(); 
                        w.Show();
                    }
                    else
                    {
                        w.Close();
                        Application.Current.Shutdown();
                    }
                }
            });
            ApplyRoleVisibility();
        }

        // ===== LOGIC PHÂN QUYỀN  =====
        private void ApplyRoleVisibility()
        {
            var user = UserSession.CurrentUser;

            // 1. Nếu chưa đăng nhập -> Ẩn hết
            if (user == null)
            {
                AdminVisibility = Visibility.Collapsed;
                InternalVisibility = Visibility.Collapsed;
                CustomerVisibility = Visibility.Collapsed;
                return;
            }

            // 2. Chuẩn hóa chuỗi quyền (chữ thường, xóa khoảng trắng)
            string role = user.UserType.Trim().ToLower();

            // 3. Xử lý phân quyền
            if (role == "admin" || role == "quản lý")
            {
                // Admin: Thấy menu Quản lý + Menu Nội bộ
                AdminVisibility = Visibility.Visible;
                InternalVisibility = Visibility.Visible; // <--- QUAN TRỌNG: Phải hiện cái này thì mới thấy Trang chủ, Bàn
                CustomerVisibility = Visibility.Collapsed;

                CurrentView = HomeVM; // Mặc định vào Trang chủ
            }
            else if (role == "staff" || role == "nhân viên")
            {
                // Staff: Không thấy menu Quản lý + Thấy Menu Nội bộ
                AdminVisibility = Visibility.Collapsed;
                InternalVisibility = Visibility.Visible; // <--- QUAN TRỌNG: Phải hiện cái này
                CustomerVisibility = Visibility.Collapsed;

                CurrentView = HomeVM; // Mặc định vào Trang chủ
            }
            else if (role == "customer" || role == "khách hàng")
            {
                // Customer: Chỉ thấy Menu gọi món
                AdminVisibility = Visibility.Collapsed;
                InternalVisibility = Visibility.Collapsed;
                CustomerVisibility = Visibility.Visible;

                CurrentView = CustomerOrderVM; // Mặc định vào màn hình Gọi món
            }
            else
            {
                // Trường hợp lạ: Ẩn hết
                AdminVisibility = Visibility.Collapsed;
                InternalVisibility = Visibility.Collapsed;
                CustomerVisibility = Visibility.Collapsed;
            }
        }
    }
}