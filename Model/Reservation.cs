using System;
using System.Data;

namespace QuanLyNhaHang.Model
{
    public class Reservation
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BookingTime { get; set; }
        public int GuestCount { get; set; }
        public int TableId { get; set; }
        public string Status { get; set; }

        public Reservation() { }
    }
}