using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CookBook.DAL.Data.Entities;
using CookBook.DAL.Interfaces;
using CookBook.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace CookBook.DAL.Services
{
    public class RecipeService:IRecipeService
    {
        private readonly ICookBookDbContext _context;
        public RecipeService(ICookBookDbContext context)
        {
            _context = context;
        }

        public async Task<List<Recipe>> GetRecipes()
        {
            return await _context.Recipes.OrderBy(r=>r.Title.Trim()).ToListAsync();
        }

        public async Task<Recipe> GetRecipe(int recipeId)
        {
            return await _context.Recipes.FirstOrDefaultAsync(r => r.Id == recipeId);
        }
        
        public async Task<RecipeModel> GetRecipeTree(int recipeId)
        {
            var recipes = await GetRecipes();
            return recipes.Select(r => new RecipeModel().Map(r)).FirstOrDefault(r => r.Id == recipeId);
        }
        
        private async Task<Recipe> GetRecipeNoTracking(int recipeId)
        {
            return await _context.Recipes.AsNoTracking().FirstOrDefaultAsync(r => r.Id == recipeId);
        }

        public async Task<Recipe> AddRecipe(RecipeFormModel recipeModel)
        {
            var recipe = recipeModel.Map();
            recipe.DateCreated=DateTime.Now;
            await _context.Recipes.AddAsync(recipe);
            await _context.SaveChangesAsync();
            return recipe;
        }

        public async Task<Recipe> EditRecipe(RecipeFormModel recipeModel)
        {
            var currentRecipe = await GetRecipeNoTracking(recipeModel.Id);
            
            if (recipeModel.Title.Equals(currentRecipe.Title) &&
                recipeModel.Description == currentRecipe.Description &&
                recipeModel.Ingredients.Equals(currentRecipe.Ingredients) &&
                recipeModel.Directions.Equals(currentRecipe.Directions))
            {
                return currentRecipe;
            }

            await _context.Revisions.AddAsync(new RecipeRevision()
            {
                RecipeId = currentRecipe.Id,
                DateModified = currentRecipe.DateLastUpdated ?? currentRecipe.DateCreated,
                Title = currentRecipe.Title,
                Description = currentRecipe.Description,
                Directions = currentRecipe.Directions,
                Ingredients = currentRecipe.Ingredients
            });
            
            var recipe = recipeModel.Map();
            recipe.DateCreated = currentRecipe.DateCreated;
            recipe.ParentRecipeId = currentRecipe.ParentRecipeId;
            recipe.DateLastUpdated=DateTime.Now;
            _context.Recipes.Update(recipe);
            
            await _context.SaveChangesAsync();
            
            return await GetRecipe(recipe.Id);
        }

        public async Task DeleteRecipe(int recipeId)
        {
            var recipeToDelete = await GetRecipe(recipeId);
            var inheritedRecipes = await GetInheritedRecipes(recipeId);
            inheritedRecipes.ForEach(r => r.ParentRecipeId = recipeToDelete.ParentRecipeId);
            _context.Recipes.UpdateRange(inheritedRecipes);
            _context.Recipes.Remove(recipeToDelete);
            await _context.SaveChangesAsync();
        }

        private async Task<List<Recipe>> GetInheritedRecipes(int recipeId)
        {
            return await _context.Recipes.Where(r => r.ParentRecipeId == recipeId).ToListAsync();
        }
    }
}