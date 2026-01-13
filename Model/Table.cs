using System.Data;

namespace QuanLyNhaHang.Model
{
    public class Table
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }

        public Table(int id, string name, string status)
        {
            this.Id = id;
            this.Name = name;
            this.Status = status;
        }

        // Constructor này giúp chuyển đổi 1 dòng dữ liệu SQL thành đối tượng Table
        public Table(DataRow row)
        {
            this.Id = (int)row["Id"];
            this.Name = row["Name"].ToString();
            this.Status = row["Status"].ToString();
        }
    }
}