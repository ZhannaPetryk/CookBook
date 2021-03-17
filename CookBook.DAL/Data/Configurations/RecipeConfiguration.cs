using CookBook.DAL.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CookBook.DAL.Data.Configurations
{
    public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
    {
        public void Configure(EntityTypeBuilder<Recipe> builder)
        {
            builder.ToTable("Recipes");
            builder.HasKey(recipe => recipe.Id);
            builder.Property(r => r.DateCreated).IsRequired();
            builder.Property(r => r.Title).IsRequired();
            builder.Property(r => r.Ingredients).IsRequired();
            builder.Property(r => r.Directions).IsRequired();

            builder.HasOne(r => r.ParentRecipe)
                .WithMany(r => r.InheritedRecipes)
                .HasForeignKey(k => k.ParentRecipeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}