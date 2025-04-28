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

        // Доделать бронь!!! Сейчас отсутствует интервал

        public async Task AddBooking(int tableId, DateTime time, Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            var booking = new Booking
            {
                BookingId = Guid.NewGuid(),
                User = user,
                Time = time,
                Table = tableId
            };

            await _unitOfWork.Bookings.AddAsync(booking);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
