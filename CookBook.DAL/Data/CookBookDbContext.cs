using CookBook.DAL.Data.Entities;
using CookBook.DAL.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CookBook.DAL.Data
{
    public class CookBookDbContext : IdentityDbContext<AdminUser>, ICookBookDbContext
    {
        public virtual DbSet<Recipe> Recipes { get; set; }
        public virtual DbSet<RecipeRevision> Revisions { get; set; }
        
        public CookBookDbContext(DbContextOptions<CookBookDbContext> options) : base(options)
        {
            _ = Database.EnsureCreated();
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var assemblyWithConfigurations = GetType().Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(assemblyWithConfigurations);

            base.OnModelCreating(modelBuilder);
        }
    }
}