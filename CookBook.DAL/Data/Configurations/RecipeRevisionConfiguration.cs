using CookBook.DAL.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CookBook.DAL.Data.Configurations
{
    public class RecipeRevisionConfiguration: IEntityTypeConfiguration<RecipeRevision>
    {
        public void Configure(EntityTypeBuilder<RecipeRevision> builder)
        {
            builder.ToTable("RecipeRevisions");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.DateModified).IsRequired();
            builder.Property(r => r.Title).IsRequired();
            builder.Property(r => r.Ingredients).IsRequired();
            builder.Property(r => r.Directions).IsRequired();

            builder.HasOne(r => r.Recipe)
                .WithMany(r => r.Revisions)
                .HasForeignKey(r => r.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}