using Microsoft.EntityFrameworkCore;
using YumBlazor.Data;
using YumBlazor.Repository.IRepository;

namespace YumBlazor.Repository
{
	public class ProductRepository : IProductRepository
	{
		private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        public ProductRepository(IDbContextFactory<ApplicationDbContext> contextFactory, IWebHostEnvironment webHostEnvironment)
        {
            _contextFactory = contextFactory;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<Product> CreateAsync(Product obj)
		{
            await using var _db = _contextFactory.CreateDbContext();
            await _db.Product.AddAsync(obj);
			await _db.SaveChangesAsync();
			return obj;
		}

		public async Task<bool> DeleteAsync(int id)
		{
            await using var _db = _contextFactory.CreateDbContext();
            var obj = await _db.Product.FirstOrDefaultAsync(u => u.Id == id);
			var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('/'));
			if (File.Exists(imagePath))
			{
				File.Delete(imagePath);
			}
			if (obj != null)
			{
				_db.Product.Remove(obj);
				return (await _db.SaveChangesAsync()) > 0;
			}
			return false;
		}

		public async Task<Product> GetAsync(int id)
		{
            await using var _db = _contextFactory.CreateDbContext();
            var obj = await _db.Product.FirstOrDefaultAsync(u => u.Id == id);
			if(obj == null)
			{
				return new Product();
			}
			return obj;
		}

		public async Task<IEnumerable<Product>> GetAllAsync()
		{
            await using var _db = _contextFactory.CreateDbContext();
            return await _db.Product.Include(u=>u.Category).ToListAsync();
		}

		public async Task<Product> UpdateAsync(Product obj)
		{
            await using var _db = _contextFactory.CreateDbContext();
            var objFromDb = await _db.Product.FirstOrDefaultAsync(u => u.Id == obj.Id);
            if (objFromDb is not null)
            {
				objFromDb.Name = obj.Name;
                objFromDb.Description = obj.Description;
                objFromDb.ImageUrl = obj.ImageUrl;
                objFromDb.CategoryId = obj.CategoryId;
                objFromDb.Price = obj.Price;
                _db.Product.Update(objFromDb);
				await _db.SaveChangesAsync();
				return objFromDb;
            }
			return obj;
        }
	}
}
