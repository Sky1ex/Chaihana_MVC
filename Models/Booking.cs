namespace WebApplication1.Models
{
    public class Booking
    {
        public Guid BookingId { get; set; }
        public User User { get; set; }
        public int Table { get; set; }
        public DateTime Time { get; set; }
    }
}
