using Microsoft.EntityFrameworkCore;
using YumBlazor.Data;
using YumBlazor.Repository.IRepository;

namespace YumBlazor.Repository
{
	public class CategoryRepository : ICategoryRepository
	{
		private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        public CategoryRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
		{
            _contextFactory = contextFactory;
		}
		public async Task<Category> CreateAsync(Category obj)
		{
            await using var _db = _contextFactory.CreateDbContext();
            await _db.Category.AddAsync(obj);
			await _db.SaveChangesAsync();
			return obj;
		}

		public async Task<bool> DeleteAsync(int id)
		{
            await using var _db = _contextFactory.CreateDbContext();
            var obj = await _db.Category.FirstOrDefaultAsync(u => u.Id == id);
			if (obj != null)
			{
				_db.Category.Remove(obj);
				return (await _db.SaveChangesAsync()) > 0;
			}
			return false;
		}

		public async Task<Category> GetAsync(int id)
		{
            await using var _db = _contextFactory.CreateDbContext();
            var obj = await _db.Category.FirstOrDefaultAsync(u => u.Id == id);
			if(obj == null)
			{
				return new Category();
			}
			return obj;
		}

		public async Task<IEnumerable<Category>> GetAllAsync()
		{
            await using var _db = _contextFactory.CreateDbContext();
            return await _db.Category.ToListAsync();
		}

		public async Task<Category> UpdateAsync(Category obj)
		{
            await using var _db = _contextFactory.CreateDbContext();
            var objFromDb = await _db.Category.FirstOrDefaultAsync(u => u.Id == obj.Id);
            if (objFromDb is not null)
            {
				objFromDb.Name = obj.Name;
				_db.Category.Update(objFromDb);
				await _db.SaveChangesAsync();
				return objFromDb;
            }
			return obj;
        }
	}
}
