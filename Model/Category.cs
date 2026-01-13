using System.Data;

namespace QuanLyNhaHang.Model
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Category(DataRow row)
        {
            this.Id = (int)row["Id"];
            this.Name = row["Name"].ToString();
        }
    }
}