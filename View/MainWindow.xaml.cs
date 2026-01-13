using System.Windows;

namespace QuanLyNhaHang
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Thêm đoạn này để chắc chắn App tắt hoàn toàn khi đóng Main
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
    }
}