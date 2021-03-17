using System;
using CookBook.DAL.Data.Entities;

namespace CookBook.DAL.Models
{
    public class RecipeModel
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateLastUpdated { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Ingredients { get; set; }
        public string Directions { get; set; }

        public RecipeModel ParentRecipe { get; set; }
        public int? ParentRecipeId { get; set; }
        
        public RecipeModel Map(Recipe recipe)
        {
            Id = recipe.Id;
            DateCreated = recipe.DateCreated;
            DateLastUpdated = recipe.DateLastUpdated;
            Title = recipe.Title;
            Description = recipe.Description;
            Ingredients = recipe.Ingredients;
            Directions = recipe.Directions;
            ParentRecipeId = recipe.ParentRecipeId;
            ParentRecipe = (recipe.ParentRecipe == null || recipe.ParentRecipe.Id == Id) ? null : new RecipeModel().Map(recipe.ParentRecipe);

            return this;
        }
        public Recipe Map()
        {
            return new Recipe()
            {
                Id = this.Id,
                Title = this.Title,
                Description = this.Description,
                Ingredients = this.Ingredients,
                Directions = this.Directions,
                ParentRecipeId = this.ParentRecipeId,
                DateCreated = this.DateCreated,
                DateLastUpdated = this.DateLastUpdated
            };
        }
    }
}