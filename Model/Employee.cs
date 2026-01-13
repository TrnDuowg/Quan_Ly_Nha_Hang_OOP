using System;
using System.Data;

namespace QuanLyNhaHang.Model
{
    // Employee kế thừa từ User 
    public class Employee : User
    {
        public string Position { get; set; } // Chức vụ
        public decimal Salary { get; set; }  // Lương
        public DateTime HireDate { get; set; } // Ngày vào làm

        public Employee() { }

        public Employee(DataRow row) : base(row) // Gọi constructor của lớp cha User
        {
            // Các thuộc tính riêng của Employee
            this.Position = row["Position"] != DBNull.Value ? row["Position"].ToString() : "";
            this.Salary = row["Salary"] != DBNull.Value ? (decimal)row["Salary"] : 0;
            this.HireDate = row["HireDate"] != DBNull.Value ? (DateTime)row["HireDate"] : DateTime.Now;
        }
    }
}