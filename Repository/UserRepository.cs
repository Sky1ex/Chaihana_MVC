using Microsoft.EntityFrameworkCore;
using WebApplication1.DataBase;
using WebApplication1.Models;
using WebApplication1.Repository.Default;

namespace WebApplication1.Repository
{
    public class UserRepository : Repository<User>, IDisposable
    {
        private bool disposed = false;

        public UserRepository(ApplicationDbContext context) : base(context) { }

        public async Task<User?> GetByPhone(string phoneNumber)
        {
            return await _context.Users
                .Include(u => u.Adresses)
                .FirstOrDefaultAsync(u => u.Phone == phoneNumber);
        }

        public async Task<User?> GetByIdWithAddresses(Guid userId)
        {
            return await _context.Users
                .Include(u => u.Adresses)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User?> GetByIdWithOrders(Guid userId)
        {
            return await _context.Users
                .Include(u => u.Orders)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User?> GetByIdFull(Guid userId)
        {
            /*return await _context.Users
                .Include(u => u.Orders)
                .Include(u => u.Adresses)
                .FirstOrDefaultAsync(u => u.UserId == userId);*/
            return await _context.Users
                .Include(c => c.Orders)
                .ThenInclude(ce => ce.OrderElement)
                .ThenInclude(ced => ced.Product)
                .Include(c => c.Orders)
                .ThenInclude(ce => ce.Adress)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
