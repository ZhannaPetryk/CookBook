using System.Threading.Tasks;
using CookBook.DAL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CookBook.WebUI.Controllers
{
    public class RevisionController : Controller
    {
        private readonly IRevisionService _revisionService;
        
        public RevisionController(IRevisionService revisionService)
        {
            _revisionService = revisionService;
        }
        
        [HttpGet]
        [Route("{recipeId}/revision")]
        public async Task<IActionResult> Index(int recipeId)
        {
            return View(await _revisionService.GetRevisionsByRecipeId(recipeId));
        }
        
        [HttpGet]
        [Route("{recipeId}/revision/{revisionId}")]
        public async Task<IActionResult> GetRevision(int recipeId, int revisionId)
        {
            return View(await _revisionService.GetRevision(revisionId));
        }
        
        [HttpGet]
        [Route("{recipeId}/revision/{revisionId}/delete")]
        public async Task<IActionResult> Delete(int recipeId, int revisionId)
        {
            await _revisionService.DeleteRevision(revisionId);
            return RedirectToRoute(new RouteValueDictionary(new { action = "Index", controller = "Revision", recipeId = recipeId}));
        }
    }
}