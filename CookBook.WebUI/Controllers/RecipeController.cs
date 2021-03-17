using System.Linq;
using System.Threading.Tasks;
using CookBook.DAL.Interfaces;
using CookBook.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CookBook.WebUI.Controllers
{
    [Route("recipe")]
    [Authorize]
    public class RecipeController : Controller
    {
        private readonly IRecipeService _recipeService;

        public RecipeController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }
        
        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        [Route("/")]
        public async Task<IActionResult> Index()
        {
            var recipes = await _recipeService.GetRecipes();
            
            return View(recipes.Select(r => new RecipeModel().Map(r)).ToList());
        }
        
        [HttpGet]
        [AllowAnonymous]
        [Route("{recipeId:int}")]
        public async Task<IActionResult> GetRecipe(int recipeId)
        {
            var recipe = await _recipeService.GetRecipeTree(recipeId);
            return View(recipe);
        }
        
        [HttpGet]
        [Route("add")]
        public IActionResult Add(int? parentId)
        {
            return View(new RecipeFormModel(){ParentRecipeId = parentId});
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add(RecipeFormModel recipeModel)
        {
            var recipe = await _recipeService.AddRecipe(recipeModel);
            return RedirectToRoute(new RouteValueDictionary(new { action = "GetRecipe", controller = "Recipe", recipeId = recipe.Id}));
        }

        [HttpGet]
        [Route("edit")]
        public async Task<IActionResult> Edit(int recipeId)
        {
            var recipe = await _recipeService.GetRecipe(recipeId);
            return View(new RecipeFormModel().Map(recipe));
        }
        
        [HttpPost]
        [Route("edit")]
        public async Task<IActionResult> Edit(RecipeFormModel recipeModel)
        {
            var recipe = await _recipeService.EditRecipe(recipeModel);
            return RedirectToRoute(new RouteValueDictionary(new { action = "GetRecipe", controller = "Recipe", recipeId = recipe.Id}));
        }
        
        [HttpGet]
        [Route("delete")]
        public async Task<IActionResult> Delete(int recipeId)
        {
            await _recipeService.DeleteRecipe(recipeId);
            return RedirectToAction("Index");
        }
    }
}