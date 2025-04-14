using Microsoft.EntityFrameworkCore;
using WebApplication1.DataBase;
using WebApplication1.DTO;
using WebApplication1.Models;
using WebApplication1.OtherClasses;

namespace WebApplication1.Services
{
    public class BookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CartService> _logger;

        public BookingService(ApplicationDbContext context, ILogger<CartService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<BookingDto>> GetAllBookingsByTableId(int tableId)
        {
            var booking = await _context.Bookings.Where(x => x.Table == tableId).ToListAsync();

            var bookingDto = booking.Select(x => new BookingDto
            {
                Time = x.Time.ToLocalTime(),
                Interval = x.Interval,
            }).ToList();

            return bookingDto;
        }

        public async Task AddBooking(int tableId, DateTime time, Guid userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(c => c.UserId == userId);

            var booking = new Booking
            {
                BookingId = Guid.NewGuid(),
                User = user,
                Time = time,
                Table = tableId,
            };

            _context.Bookings.Add(booking);

            await _context.SaveChangesAsync();
        }
    }
}
