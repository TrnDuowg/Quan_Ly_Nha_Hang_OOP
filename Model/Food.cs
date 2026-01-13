using System.Data;

namespace QuanLyNhaHang.Model
{
    public class Food
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }

        public Food(DataRow row)
        {
            this.Id = (int)row["Id"];
            this.Name = row["Name"].ToString();
            this.CategoryId = (int)row["CategoryId"];
            this.Price = (decimal)row["Price"];
        }
    }
}