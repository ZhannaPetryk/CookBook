using System;
using System.Collections;
using System.Collections.Generic;

namespace CookBook.DAL.Data.Entities
{
    public class Recipe
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateLastUpdated { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Ingredients { get; set; }
        public string Directions { get; set; }

        public Recipe ParentRecipe { get; set; }
        public int? ParentRecipeId { get; set; }

        public IEnumerable<Recipe> InheritedRecipes { get; set; } = new List<Recipe>();

        public IEnumerable<RecipeRevision> Revisions { get; set; } = new List<RecipeRevision>();
    }
}