using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CookBook.DAL.Interfaces;
using CookBook.DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace CookBook.API.Controllers
{
    public class RecipeController : Controller
    {
        private readonly IRecipeService _recipeService;

        public RecipeController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpGet]
        [Route("recipes")]
        public async Task<IEnumerable<RecipeModel>> Index()
        {
            var recipes = await _recipeService.GetRecipes();
            return recipes.Select(r => new RecipeModel().Map(r)).ToList();
        }
        
        [HttpGet]
        [Route("recipes/{recipeId:int}")]
        public async Task<RecipeModel> GetRecipe(int recipeId)
        {
            return await _recipeService.GetRecipeTree(recipeId);
        }
        
        [HttpPost]
        [Route("add")]
        public async Task<RecipeModel> Add(RecipeFormModel recipeModel)
        {
            var recipe = await _recipeService.AddRecipe(recipeModel);
            return new RecipeModel().Map(recipe);
        }

        [HttpPut]
        [Route("edit")]
        public async Task<RecipeModel> Edit(RecipeFormModel recipeModel)
        {
            var recipe = await _recipeService.EditRecipe(recipeModel);
            return new RecipeModel().Map(recipe);
        }
        
        [HttpDelete]
        [Route("delete")]
        public async Task Delete(int recipeId)
        {
            await _recipeService.DeleteRecipe(recipeId);
        }
    }
}