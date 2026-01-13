using System;
using System.Data;

namespace QuanLyNhaHang.Model
{
    public class Promotion
    {
        public string Id { get; set; }        // Mã khuyến mãi (Khóa chính)
        public string Name { get; set; }      // Tên chương trình
        public decimal DiscountValue { get; set; } // Giá trị giảm
        public string DiscountType { get; set; }   // Loại: 'Percent' hoặc 'Amount'
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }    // 'Active' hoặc 'Inactive'

        // Constructor mặc định
        public Promotion() { }

        // Constructor chuyển từ DataRow của SQL sang Object C#
        public Promotion(DataRow row)
        {
            this.Id = row["Id"].ToString();
            this.Name = row["Name"].ToString();

            // Kiểm tra null cho số
            this.DiscountValue = row["DiscountValue"] != DBNull.Value ? Convert.ToDecimal(row["DiscountValue"]) : 0;

            this.DiscountType = row["DiscountType"].ToString();

            //  KIỂM TRA NULL CHO NGÀY THÁNG ---
            this.StartDate = row["StartDate"] != DBNull.Value ? (DateTime)row["StartDate"] : DateTime.Now;
            this.EndDate = row["EndDate"] != DBNull.Value ? (DateTime)row["EndDate"] : DateTime.Now;
            // ---------------------------------------------------

            this.Status = row["Status"].ToString();
        }
    }
}