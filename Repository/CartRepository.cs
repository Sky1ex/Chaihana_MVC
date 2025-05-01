using Microsoft.EntityFrameworkCore;
using WebApplication1.DataBase;
using WebApplication1.Models;
using WebApplication1.Repository.Default;

namespace WebApplication1.Repository
{
    public class CartRepository : Repository<Cart>, IDisposable
    {
        private bool disposed = false;

        public CartRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Cart?> GetByUserId(Guid userId)
        {
            return await _context.Carts
                .FirstOrDefaultAsync(x => x.User.UserId == userId);
        }

        public async Task<Cart?> GetByUserIdWithCartElements(Guid userId)
        {
            return await _context.Carts
                .Include(x => x.CartElement)
                .FirstOrDefaultAsync(x => x.User.UserId == userId);
        }

        public async Task<Cart?> GetByUserIdFull(Guid userId)
        {
            return await _context.Carts
                .Include(x => x.CartElement)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.User.UserId == userId);
        }

        public async Task<Cart?> GetByUserIdFullWithoutTracking(Guid userId)
        {
            return await _context.Carts
                .AsNoTracking()
                .Include(x => x.CartElement)
                .ThenInclude(x => x.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.User.UserId == userId);
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
