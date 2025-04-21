namespace WebApplication1.Models
{
    public class Booking
    {
        public Guid BookingId { get; set; } = Guid.NewGuid();
        public required User User { get; set; }
        public required int Table { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
		public int Interval { get; set; }
    }
}
