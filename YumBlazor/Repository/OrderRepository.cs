using Microsoft.EntityFrameworkCore;
using YumBlazor.Data;
using YumBlazor.Repository.IRepository;

namespace YumBlazor.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        public OrderRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<OrderHeader> CreateAsync(OrderHeader orderHeader)
        {
            await using var _db = _contextFactory.CreateDbContext();
            orderHeader.OrderDate = DateTime.Now;
            await _db.OrderHeader.AddAsync(orderHeader);
            await _db.SaveChangesAsync();
            return orderHeader;
        }

        public async Task<IEnumerable<OrderHeader>> GetAllAsync(string? userId = null)
        {
            await using var _db = _contextFactory.CreateDbContext();
            if (!string.IsNullOrEmpty(userId))
            {
                return await _db.OrderHeader.Where(u => u.UserId == userId).ToListAsync();
            }
            return await _db.OrderHeader.ToListAsync();
        }

        public async Task<OrderHeader?> GetAsync(int id)
        {
            await using var _db = _contextFactory.CreateDbContext();
            return await _db.OrderHeader.Include(u => u.OrderDetails).FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<OrderHeader?> GetOrderBySessionIdAsync(string sessionId)
        {
            await using var _db = _contextFactory.CreateDbContext();
            return await _db.OrderHeader.FirstOrDefaultAsync(u => u.SessionId == sessionId);
        }

        public async Task<OrderHeader?> UpdateStatusAsync(int orderId, string status, string paymentIntentId)
        {
            await using var _db = _contextFactory.CreateDbContext();
            var orderHeader = _db.OrderHeader.FirstOrDefault(u => u.Id == orderId);
            if (orderHeader != null)
            {
                orderHeader.Status = status;
                if (!string.IsNullOrEmpty(paymentIntentId))
                {
                    orderHeader.PaymentIntentId = paymentIntentId;
                }
                await _db.SaveChangesAsync();
            }
            return orderHeader;
        }
    }
}
