using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CookBook.DAL.Data;
using CookBook.DAL.Data.Entities;
using CookBook.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CookBook.DAL.Services
{
    public class RevisionService : IRevisionService
    {
        private readonly ICookBookDbContext _context;
        public RevisionService(ICookBookDbContext context)
        {
            _context = context;
        }
        public async Task<List<RecipeRevision>> GetRevisionsByRecipeId(int recipeId)
        {
            return await _context.Revisions.Where(r => r.RecipeId == recipeId).ToListAsync();
        }

        public async Task<RecipeRevision> GetRevision(int revisionId)
        {
            return await _context.Revisions.FirstOrDefaultAsync(r => r.Id == revisionId);
        }

        public async Task DeleteRevision(int revisionId)
        {
            _context.Revisions.Remove(await GetRevision(revisionId));
            await _context.SaveChangesAsync(); 
        }
    }
}