using System;
using System.Data;

namespace QuanLyNhaHang.Model
{
    public class User
    {
        // Các thuộc tính trùng tên với cột trong SQL Users
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; } 
        public string UserType { get; set; } // Admin, Staff, Customer

        
        public string PhoneNumber { get; set; }

        public User() { }

        
        public User(DataRow row)
        {
            this.Id = (int)row["Id"];
            this.DisplayName = row["DisplayName"].ToString();
            this.UserName = row["UserName"].ToString();
            this.PasswordHash = row["PasswordHash"].ToString();
            this.UserType = row["UserType"].ToString();

         
            // Kiểm tra xem trong kết quả truy vấn có cột PhoneNumber không để tránh lỗi
            if (row.Table.Columns.Contains("PhoneNumber") && row["PhoneNumber"] != DBNull.Value)
            {
                this.PhoneNumber = row["PhoneNumber"].ToString();
            }
            else
            {
                this.PhoneNumber = ""; // Mặc định nếu không có
            }
        }
    }
}