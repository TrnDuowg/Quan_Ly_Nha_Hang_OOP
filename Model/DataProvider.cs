using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace QuanLyNhaHang.Model
{
    public class DataProvider
    {
        // --- 1. Áp dụng SINGLETON PATTERN (Mẫu thiết kế duy nhất) ---
        private static DataProvider _ins;
        public static DataProvider Ins
        {
            get
            {
                if (_ins == null) _ins = new DataProvider();
                return _ins;
            }
            set { _ins = value; }
        }

        // --- 2. CHUỖI KẾT NỐI ---
        // Lưu ý: Nếu máy bạn tên Server khác .\SQLEXPRESS thì sửa lại đoạn này.
        private string connectionSTR = @"Data Source=.\SQLEXPRESS;Initial Catalog=QuanLyNhaHangOOP;Integrated Security=True";

        // --- 3. CÁC HÀM XỬ LÝ CHÍNH ---

        // Hàm lấy dữ liệu (SELECT) -> Trả về bảng DataTable
        public DataTable ExecuteQuery(string query, object[] parameter = null)
        {
            DataTable data = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionSTR))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                AddParameter(command, query, parameter);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(data);
                connection.Close();
            }
            return data;
        }

        // Hàm thực hiện thay đổi dữ liệu (INSERT, UPDATE, DELETE) -> Trả về số dòng ảnh hưởng
        public int ExecuteNonQuery(string query, object[] parameter = null)
        {
            int data = 0;
            using (SqlConnection connection = new SqlConnection(connectionSTR))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                AddParameter(command, query, parameter);
                data = command.ExecuteNonQuery();
                connection.Close();
            }
            return data;
        }

        // Hàm thực hiện đếm hoặc lấy giá trị đầu tiên (COUNT, SELECT TOP 1) -> Trả về object
        public object ExecuteScalar(string query, object[] parameter = null)
        {
            object data = 0;
            using (SqlConnection connection = new SqlConnection(connectionSTR))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                AddParameter(command, query, parameter);
                data = command.ExecuteScalar();
                connection.Close();
            }
            return data;
        }

        // Hàm phụ trợ để map tham số vào query (Tránh lỗi SQL Injection)
        private void AddParameter(SqlCommand command, string query, object[] parameter)
        {
            if (parameter != null)
            {
                string[] listPara = query.Split(' ');
                int i = 0;
                foreach (string item in listPara)
                {
                    if (item.Contains('@'))
                    {
                        command.Parameters.AddWithValue(item, parameter[i]);
                        i++;
                    }
                }
            }
        }
    }
}