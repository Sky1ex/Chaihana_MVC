using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DataBase;
using WebApplication1.DTO;
using WebApplication1.Models;
using WebApplication1.OtherClasses;
using WebApplication1.Repository.Default;

namespace WebApplication1.Services
{
    public class BookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CartService> _logger;
        private readonly IMapper _mapper;

        public BookingService(ApplicationDbContext context, ILogger<CartService> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<BookingDto>> GetAllBookingsByTableId(int tableId)
        {
            var booking = await _unitOfWork.Bookings.GetBookingsByTableId(tableId);

            var bookingDto = booking.Select(x => _mapper.Map<BookingDto>(x)).ToList();

            return bookingDto;
        }

		public async Task<List<BookingDto>> GetAllBookingsByTableIdAndDate(int tableId, DateTime time)
		{
			var booking = await _unitOfWork.Bookings.GetBookingsByTableId(tableId);
            
            booking = booking.Where(x => x.Time.Day == time.Day).OrderBy(x => x.Time.Hour).ToList();

			var bookingDto = booking.Select(x => _mapper.Map<BookingDto>(x)).ToList();

			return bookingDto;
		}

		public async Task<List<BookingDto>> GetAllBookingsByUserId(Guid userId)
		{
			var booking = await _unitOfWork.Bookings.GetBookingsByUserId(userId);

			var bookingDto = booking.Select(x => _mapper.Map<BookingDto>(x)).ToList();

			bookingDto.ForEach(x => x.Time = x.Time.ToLocalTime());

			return bookingDto;
		}

		public async Task<bool> AddBooking(int tableId, DateTime time, int interval, Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            var bookings = await _unitOfWork.Bookings.GetBookingsByTableId(tableId);

            var checkBooking = bookings.Where(b => (time.Hour >= b.Time.Hour && (time.Hour < b.Time.Hour + b.Interval)) || ((time.Hour + interval) > b.Time.Hour && (time.Hour + interval) <= (b.Time.Hour + b.Interval)));

            if (checkBooking.Count() != 0 || interval > 3 || interval <= 0)
            {
                return false;
            }

            var booking = new Booking
            {
                BookingId = Guid.NewGuid(),
                User = user,
                Time = time,
                Table = tableId,
                Interval = interval
            };

            await _unitOfWork.Bookings.AddAsync(booking);

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task DeleteBooking(Guid bookingId)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
            _unitOfWork.Bookings.Delete(booking);

			await _unitOfWork.SaveChangesAsync();
		}
    }
}
