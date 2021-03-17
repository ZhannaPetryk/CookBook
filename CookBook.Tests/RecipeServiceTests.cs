using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CookBook.DAL.Data.Entities;
using CookBook.DAL.Interfaces;
using CookBook.DAL.Models;
using CookBook.DAL.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using MockQueryable;
using MockQueryable.Moq;

namespace CookBook.Tests
{
    public partial class Tests
    {
        [SetUp]
        public void Setup()
        {
            

        }

        [Test]
        public async Task GetRecipes_OrdersByName()
        {
            //Arrange
            var mockRecipes = SetTestRecipes();
            
            var mockContext = new Mock<ICookBookDbContext>();
            mockContext.Setup(m => m.Recipes).Returns(mockRecipes.Object);
            var recipeService = new RecipeService(mockContext.Object);
            
            //Act
            var recipes = await recipeService.GetRecipes();
            
            //Assert
            Assert.AreEqual(3, recipes.Count);
            Assert.AreEqual("Cheesecake", recipes[0].Title);
            Assert.AreEqual("Cheesecake with Blueberries", recipes[1].Title);
            Assert.AreEqual("Chocolate Cake", recipes[2].Title);
        }
        
        [Test]
        public async Task AddRecipe_SavesARecipeViaContext()
        {
            //Arrange
            var mockSet = new Mock<DbSet<Recipe>>();
            var mockContext = new Mock<ICookBookDbContext>();
            mockContext.Setup(m => m.Recipes).Returns(mockSet.Object);

            var service = new RecipeService(mockContext.Object);
            
            //Act
            await service.AddRecipe( new RecipeFormModel()
            {
                Title = "Cheesecake",
                Description = "Sweet and fluffy",
                Ingredients = "1 kg cheese, 4 eggs, 1 glass of sugar," +
                              "3 tbs of flour, 200 ml of sour cream",
                Directions = "Mix everything. Bake at 180 for 10 min, then at 120 for 30 min"
            });
            
            //Assert
            mockSet.Verify(m => m.AddAsync(It.IsAny<Recipe>(),default), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once());
        }
        
        [Test]
        public async Task EditRecipe_SavesARecipeViaContext()
        {
            //Arrange
            var mockRecipes = SetTestRecipes();
            var mockRevisions = new Mock<DbSet<RecipeRevision>>();
            var mockContext = new Mock<ICookBookDbContext>();
            
            mockContext.Setup(m => m.Recipes).Returns(mockRecipes.Object);
            mockContext.Setup(m => m.Revisions).Returns(mockRevisions.Object);

            var service = new RecipeService(mockContext.Object);
            
            //Act
            await service.EditRecipe(new RecipeFormModel()
            {
                Id =1,
                Title = "New York style Cheesecake",
                Description = "Sweet and fluffy",
                Ingredients = "1 kg cheese, 4 eggs, 1 glass of sugar," +
                              "3 tbs of flour, 200 ml of sour cream",
                Directions = "Mix everything. Bake at 180 for 10 min, then at 120 for 30 min"
            });
            
            //Assert
            mockRecipes.Verify(m => m.Update(It.IsAny<Recipe>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once());
        }
        
        [Test]
        public async Task EditRecipe_AddsRevision()
        {
            //Arrange
            var mockRecipes = SetTestRecipes();
            var mockRevisions = new Mock<DbSet<RecipeRevision>>();
            var mockContext = new Mock<ICookBookDbContext>();
            
            mockContext.Setup(m => m.Recipes).Returns(mockRecipes.Object);
            mockContext.Setup(m => m.Revisions).Returns(mockRevisions.Object);

            var service = new RecipeService(mockContext.Object);
            
            //Act
            await service.EditRecipe(new RecipeFormModel()
            {
                Id =1,
                Title = "New York style Cheesecake",
                Description = "Sweet and fluffy",
                Ingredients = "1 kg cheese, 4 eggs, 1 glass of sugar," +
                              "3 tbs of flour, 200 ml of sour cream",
                Directions = "Mix everything. Bake at 180 for 10 min, then at 120 for 30 min"
            });
            
            //Assert
            mockRevisions.Verify(m => m.AddAsync(It.IsAny<RecipeRevision>(),default), Times.Once());
        }
        
        [Test]
        public async Task DeleteRecipe_SavesViaContext()
        {
            //Arrange
            var mockRecipes = SetTestRecipes();
            var mockRevisions = new Mock<DbSet<RecipeRevision>>();
            var mockContext = new Mock<ICookBookDbContext>();
            
            mockContext.Setup(m => m.Recipes).Returns(mockRecipes.Object);
            mockContext.Setup(m => m.Revisions).Returns(mockRevisions.Object);

            var service = new RecipeService(mockContext.Object);
            
            //Act
            await service.DeleteRecipe(2);
            
            //Assert
            mockRecipes.Verify(m => m.Remove(It.IsAny<Recipe>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once());
        }
        
        [Test]
        public async Task DeleteRecipe_DeleteRelatedRevisions()
        {
            //Arrange
            var mockRecipes = SetTestRecipes();
            var mockRevisions = SetTestRevisions();
            var mockContext = new Mock<ICookBookDbContext>();
            
            mockContext.Setup(m => m.Recipes).Returns(mockRecipes.Object);
            mockContext.Setup(m => m.Revisions).Returns(mockRevisions.Object);

            var service = new RecipeService(mockContext.Object);
            
            //Act
            await service.DeleteRecipe(2);
            
            //Assert
            mockRevisions.Verify(m => m.RemoveRange(It.IsAny<RecipeRevision>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once());
        }
        
        [Test]
        public async Task DeleteRecipe_ChangesParentRecipeForInherited()
        {
            //Arrange
            var mockRecipes = SetTestRecipes();
            var mockRevisions = new Mock<DbSet<RecipeRevision>>();
            var mockContext = new Mock<ICookBookDbContext>();
            
            mockContext.Setup(m => m.Recipes).Returns(mockRecipes.Object);
            mockContext.Setup(m => m.Revisions).Returns(mockRevisions.Object);

            var service = new RecipeService(mockContext.Object);

            //Act
            await service.DeleteRecipe(2);
            
            //Assert
            var recipes = await service.GetRecipes();
            Assert.AreEqual(2, recipes.Count);
            Assert.AreEqual(3, recipes[1].Id);
            Assert.AreEqual(1, recipes[1].ParentRecipeId);
        }
        
        [Test]
        public async Task GetRecipe_ProperId_ReturnsRecipe()
        {
            //Arrange
            var mockRecipes = SetTestRecipes();
            var mockContext = new Mock<ICookBookDbContext>();
            mockContext.Setup(m => m.Recipes).Returns(mockRecipes.Object);

            var service = new RecipeService(mockContext.Object);

            //Act
            var result = await service.GetRecipe(2);
            
            //Assert
            Assert.AreEqual("Chocolate Cake", result.Title);
            mockContext.VerifyGet(m=>m.Recipes);
        }

        [Test]
        public async Task GetRecipe_InvalidId_ReturnsNull()
        {
            //Arrange
            var mockRecipes = SetTestRecipes();
            var mockContext = new Mock<ICookBookDbContext>();
            mockContext.Setup(m => m.Recipes).Returns(mockRecipes.Object);

            var service = new RecipeService(mockContext.Object);

            //Act
            var result = await service.GetRecipe(5);
            
            //Assert
            Assert.IsNull(result);
            mockContext.VerifyGet(m=>m.Recipes);
        }
        
        [Test]
        public async Task GetRecipeTree_ReturnsParents()
        {
            //Arrange
            var mockRecipes = SetTestRecipes();
            var mockContext = new Mock<ICookBookDbContext>();
            mockContext.Setup(m => m.Recipes).Returns(mockRecipes.Object);

            var service = new RecipeService(mockContext.Object);

            //Act
            var result = await service.GetRecipeTree(3);
            
            //Assert
            Assert.IsNotNull(result.ParentRecipe.Id);
            Assert.IsNotNull(result.ParentRecipe.ParentRecipe.Id);
            mockContext.VerifyGet(m=>m.Recipes);
        }
        
        private Mock<DbSet<Recipe>> SetTestRecipes()
        {
            var recipe1 = new Recipe()
            {
                Id = 1,
                DateCreated = DateTime.Now,
                Title = "Cheesecake",
                Description = "Sweet and fluffy",
                Ingredients = "1 kg cheese, 4 eggs, 1 glass of sugar," +
                              "3 tbs of flour, 200 ml of sour cream",
                Directions = "Mix everything. Bake at 120 for 40 min"
            };
            var recipe2 = new Recipe()
            {
                Id = 2,
                DateCreated = DateTime.Now,
                Title = "Chocolate Cake",
                Description = "Very tasty",
                Ingredients = "flour, cacao, sugar, butter, berries",
                Directions = "",
                ParentRecipeId = 1,
                ParentRecipe = recipe1
            };
            var recipe3 = new Recipe()
            {
                Id = 3,
                DateCreated = DateTime.Now,
                Title = "Cheesecake with Blueberries",
                Description = "Sweet and fresh",
                Ingredients = "1 kg cheese, 4 eggs, 1 glass of sugar," +
                              "3 tbs of flour, 200 ml of sour cream, blueberry",
                Directions = "Mix everything. Bake at 120 for 40 min",
                ParentRecipeId = 2,
                ParentRecipe = recipe2
            };
            var data = new List<Recipe>
            {
                recipe1,recipe2,recipe3
            }.AsQueryable();

            return GetMockDbSet(data);
        }
        
        private Mock<DbSet<RecipeRevision>> SetTestRevisions()
        {
            var data = new List<RecipeRevision>
            {
                new RecipeRevision()
                {
                    Id=1,
                    RecipeId = 1,
                    DateModified = DateTime.Now,
                    Title = "Cheesecake Rev1",
                    Description = "Sweet and fluffy",
                    Ingredients = "1 kg cheese, 4 eggs, 1 glass of sugar," +
                                  "3 tbs of flour, 200 ml of sour cream",
                    Directions = "Mix everything. Bake at 120 for 40 min"
                },
                new RecipeRevision()
                {
                    Id = 2,
                    RecipeId = 2,
                    DateModified = DateTime.Now,
                    Title = "Chocolate Cake Rev2",
                    Description = "Very tasty",
                    Ingredients = "flour, cacao, sugar, butter, berries",
                    Directions = "Bake cake into an oven until it is ready"
                },
                new RecipeRevision()
                {
                    Id = 3,
                    RecipeId = 2,
                    DateModified = DateTime.Now,
                    Title = "Coco cake 2 Rev3",
                    Description = "",
                    Ingredients = "flour, cacao, sugar, butter, berries",
                    Directions = "Bake cake into an oven until it is ready"
                }
            }.AsQueryable();

            return GetMockDbSet(data);
        }

        private Mock<DbSet<T>> GetMockDbSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            
            var enumerable = new TestAsyncEnumerableEfCore<T>(data);
            mockSet.As<IAsyncEnumerable<T>>()
                .Setup(d => d.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(() => enumerable.GetAsyncEnumerator());

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            
            mockSet.Setup(m => m.AsQueryable()).Returns(mockSet.Object);
            mockSet = data.AsQueryable().BuildMockDbSet();
            
            return mockSet;
        }
    }
}