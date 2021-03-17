using System.Collections.Generic;
using System.Threading.Tasks;
using CookBook.DAL.Data.Entities;
using CookBook.DAL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CookBook.API.Controllers
{
    public class RevisionController : Controller
    {
        private readonly IRevisionService _revisionService;

        public RevisionController(IRevisionService revisionService)
        {
            _revisionService = revisionService;
        }
        
        [HttpGet]
        [Route("{recipeId:int}/revisions")]
        public async Task<List<RecipeRevision>> GetAllByRecipeId(int recipeId)
        {
            return await _revisionService.GetRevisionsByRecipeId(recipeId);
        }
        
        [HttpGet]
        [Route("revisions/{revisionId:int}")]
        public async Task<RecipeRevision> GetRevision(int revisionId)
        {
            return await _revisionService.GetRevision(revisionId);
        }
        
        [HttpDelete]
        [Route("revisions/{revisionId:int}/delete")]
        public async Task DeleteRevision(int revisionId)
        { 
            await _revisionService.DeleteRevision(revisionId);
        }
    }
}