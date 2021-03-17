using System.Threading;
using System.Threading.Tasks;
using CookBook.DAL.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CookBook.DAL.Interfaces
{
    public interface ICookBookDbContext
    {
        DbSet<Recipe> Recipes { get; set; }
        DbSet<RecipeRevision> Revisions { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}