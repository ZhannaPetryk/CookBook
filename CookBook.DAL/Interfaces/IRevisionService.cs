using System.Collections.Generic;
using System.Threading.Tasks;
using CookBook.DAL.Data.Entities;

namespace CookBook.DAL.Interfaces
{
    public interface IRevisionService
    {
        Task<List<RecipeRevision>> GetRevisionsByRecipeId(int recipeId);
        Task<RecipeRevision> GetRevision(int revisionId);
        Task DeleteRevision(int revisionId);
    }
}