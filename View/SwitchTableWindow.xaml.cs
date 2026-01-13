using QuanLyNhaHang.Model;
using System.Collections.Generic;
using System.Data;
using System.Windows;

namespace QuanLyNhaHang.View
{
    public partial class SwitchTableWindow : Window
    {
        // Biến lưu bàn đích được chọn để truyền ra ngoài
        public Table SelectedTargetTable { get; private set; } = null;

        public SwitchTableWindow()
        {
            InitializeComponent();
            LoadEmptyTables();
        }

        void LoadEmptyTables()
        {
            // Chỉ lấy những bàn đang TRỐNG
            string query = "SELECT * FROM DiningTable WHERE Status = N'Trống'";
            DataTable data = DataProvider.Ins.ExecuteQuery(query);

            List<Table> list = new List<Table>();
            foreach (DataRow item in data.Rows)
            {
                list.Add(new Table(item));
            }

            cbTableList.ItemsSource = list;
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (cbTableList.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn bàn muốn chuyển đến!");
                return;
            }

            SelectedTargetTable = cbTableList.SelectedItem as Table;
            this.DialogResult = true; // Báo thành công
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}