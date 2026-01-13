using QuanLyNhaHang.Model;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace QuanLyNhaHang.ViewModel
{
    public class FoodManagementViewModel : BaseViewModel
    {
        // Danh sách hiển thị
        private ObservableCollection<Food> _ListFood;
        public ObservableCollection<Food> ListFood { get => _ListFood; set { _ListFood = value; OnPropertyChanged(); } }

        private ObservableCollection<Category> _ListCategory;
        public ObservableCollection<Category> ListCategory { get => _ListCategory; set { _ListCategory = value; OnPropertyChanged(); } }

        // Binding dữ liệu nhập
        private string _DisplayName;
        public string DisplayName { get => _DisplayName; set { _DisplayName = value; OnPropertyChanged(); } }

        private decimal _Price;
        public decimal Price { get => _Price; set { _Price = value; OnPropertyChanged(); } }

        private Category _SelectedCategory;
        public Category SelectedCategory { get => _SelectedCategory; set { _SelectedCategory = value; OnPropertyChanged(); } }

        // Binding món đang chọn trong Grid
        private Food _SelectedFood;
        public Food SelectedFood
        {
            get => _SelectedFood;
            set
            {
                _SelectedFood = value;
                OnPropertyChanged();
                if (SelectedFood != null)
                {
                    DisplayName = SelectedFood.Name;
                    Price = SelectedFood.Price;
                    // Logic tìm category tương ứng để gán vào combobox (bạn tự implement thêm vòng lặp tìm ID nếu cần)
                }
            }
        }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public FoodManagementViewModel()
        {
            LoadListFood();
            LoadCategory();

            // THÊM
            AddCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(DisplayName) || SelectedCategory == null) return false;
                return true;
            }, (p) =>
            {
                var check = DataProvider.Ins.ExecuteQuery($"SELECT * FROM Food WHERE Name = N'{DisplayName}'");
                if (check.Rows.Count > 0) { MessageBox.Show("Món này đã tồn tại!"); return; }

                string query = $"INSERT INTO Food (Name, CategoryId, Price) VALUES (N'{DisplayName}', {SelectedCategory.Id}, {Price})";
                DataProvider.Ins.ExecuteNonQuery(query);
                MessageBox.Show("Thêm thành công!");
                LoadListFood();
            });

            // SỬA
            EditCommand = new RelayCommand<object>((p) =>
            {
                return SelectedFood != null && SelectedCategory != null;
            }, (p) =>
            {
                string query = $"UPDATE Food SET Name = N'{DisplayName}', CategoryId = {SelectedCategory.Id}, Price = {Price} WHERE Id = {SelectedFood.Id}";
                DataProvider.Ins.ExecuteNonQuery(query);
                MessageBox.Show("Sửa thành công!");
                LoadListFood();
            });

            // XÓA
            DeleteCommand = new RelayCommand<object>((p) => { return SelectedFood != null; }, (p) =>
            {
                if (MessageBox.Show("Bạn có chắc muốn xóa?", "Cảnh báo", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // Cần xử lý ràng buộc khóa ngoại trước khi xóa (xóa bill info liên quan hoặc update status thành Ngừng bán)
                    // Ở đây demo xóa cứng:
                    try
                    {
                        DataProvider.Ins.ExecuteNonQuery($"DELETE FROM Food WHERE Id = {SelectedFood.Id}");
                        LoadListFood();
                    }
                    catch { MessageBox.Show("Không thể xóa món này vì đã có trong hóa đơn!"); }
                }
            });
        }

        void LoadListFood()
        {
            ListFood = new ObservableCollection<Food>();
            var data = DataProvider.Ins.ExecuteQuery("SELECT * FROM Food");
            foreach (DataRow item in data.Rows) ListFood.Add(new Food(item));
        }

        void LoadCategory()
        {
            ListCategory = new ObservableCollection<Category>();
            var data = DataProvider.Ins.ExecuteQuery("SELECT * FROM FoodCategory");
            foreach (DataRow item in data.Rows) ListCategory.Add(new Category(item));
        }
    }
}