using System;

namespace CookBook.DAL.Data.Entities
{
    public class RecipeRevision
    {
        public int Id { get; set; }
        public Recipe Recipe { get; set; }
        public int RecipeId { get; set; }
        public DateTime DateModified { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Ingredients { get; set; }
        public string Directions { get; set; }
    }
}