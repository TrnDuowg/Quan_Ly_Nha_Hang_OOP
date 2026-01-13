using System.Data;

namespace QuanLyNhaHang.DTO
{
    public class BillMenu
    {
        public string FoodName { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }

        public BillMenu(string foodName, int count, decimal price, decimal totalPrice = 0)
        {
            this.FoodName = foodName;
            this.Count = count;
            this.Price = price;
            this.TotalPrice = totalPrice;
        }

        public BillMenu(DataRow row)
        {
            this.FoodName = row["Name"].ToString();
            this.Count = (int)row["Count"];
            this.Price = (decimal)row["Price"];
            this.TotalPrice = (decimal)row["TotalPrice"];
        }
    }
}