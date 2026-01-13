using System;
using System.Data;

namespace QuanLyNhaHang.DTO
{
    public class BillStatistic
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public DateTime DateCheckOut { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }

        public BillStatistic(DataRow row)
        {
            this.Id = (int)row["Id"];
            this.TableName = row["TableName"].ToString();
            this.DateCheckOut = (DateTime)row["DateCheckOut"];

            // Xử lý chuyển đổi kiểu an toàn
            this.TotalPrice = row["TotalPrice"] != DBNull.Value ? Convert.ToDecimal(row["TotalPrice"]) : 0;
            this.Discount = row["DiscountAmount"] != DBNull.Value ? Convert.ToDecimal(row["DiscountAmount"]) : 0;
        }
    }
}