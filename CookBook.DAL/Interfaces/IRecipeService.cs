using System.Collections.Generic;
using System.Threading.Tasks;
using CookBook.DAL.Data.Entities;
using CookBook.DAL.Models;

namespace CookBook.DAL.Interfaces
{
    public interface IRecipeService
    {
        Task<List<Recipe>> GetRecipes();
        Task<Recipe> GetRecipe(int recipeId);
        Task<RecipeModel> GetRecipeTree(int recipeId);
        Task<Recipe> AddRecipe(RecipeFormModel recipe);
        Task<Recipe> EditRecipe(RecipeFormModel recipe);
        Task DeleteRecipe(int recipeId);
    }
}