using CookBook.DAL.Data.Entities;

namespace CookBook.DAL.Models
{
    public class RecipeFormModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Ingredients { get; set; }
        public string Directions { get; set; }
        public int? ParentRecipeId { get; set; }
        
        public RecipeFormModel Map(Recipe recipe)
        {
            Id = recipe.Id;
            Title = recipe.Title;
            Description = recipe.Description;
            Ingredients = recipe.Ingredients;
            Directions = recipe.Directions;
            ParentRecipeId = recipe.ParentRecipeId;

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
                ParentRecipeId = this.ParentRecipeId
            };
        }
    }
}