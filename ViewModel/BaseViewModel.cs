using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QuanLyNhaHang.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Hàm này giúp giao diện tự cập nhật khi dữ liệu thay đổi
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}