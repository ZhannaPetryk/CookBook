using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using CookBook.DAL.Data.Entities;
using CookBook.DAL.Interfaces;
using CookBook.DAL.Models;
using CookBook.DAL.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace CookBook.Tests
{
    public partial class Tests
    {
        
        [Test]
        public async Task DeleteRevision_SavesViaContext()
        {
            //Arrange
            var mockRevisions = SetTestRevisions();
            var mockContext = new Mock<ICookBookDbContext>();
            mockContext.Setup(m => m.Revisions).Returns(mockRevisions.Object);

            var service = new RevisionService(mockContext.Object);
            
            //Act
            await service.DeleteRevision(2);
            
            //Assert
            mockRevisions.Verify(m => m.Remove(It.IsAny<RecipeRevision>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once());
        }

        [Test]
        public async Task GetRevision_ProperId_ReturnsRevision()
        {
            //Arrange
            var mockRevisions = SetTestRevisions();
            var mockContext = new Mock<ICookBookDbContext>();
            mockContext.Setup(m => m.Revisions).Returns(mockRevisions.Object);

            var service = new RevisionService(mockContext.Object);

            //Act
            var result = await service.GetRevision(2);
            
            //Assert
            Assert.AreEqual("Chocolate Cake Rev2", result.Title);
            mockContext.VerifyGet(m=>m.Revisions);
        }

        [Test]
        public async Task GetRevision_InvalidId_ReturnsNull()
        {
            //Arrange
            var mockRevisions = SetTestRevisions();
            var mockContext = new Mock<ICookBookDbContext>();
            mockContext.Setup(m => m.Revisions).Returns(mockRevisions.Object);

            var service = new RevisionService(mockContext.Object);

            //Act
            var result = await service.GetRevision(5);
            
            //Assert
            Assert.IsNull(result);
            mockContext.VerifyGet(m=>m.Revisions);
        }
        
        [Test]
        public async Task GetRevisionByProductId_ReturnsListOfRevision()
        {
            //Arrange
            var mockRevisions = SetTestRevisions();
            var mockContext = new Mock<ICookBookDbContext>();
            mockContext.Setup(m => m.Revisions).Returns(mockRevisions.Object);

            var service = new RevisionService(mockContext.Object);

            //Act
            var result = await service.GetRevisionsByRecipeId(2);
            
            //Assert
            Assert.AreEqual(2, result.Count);
            mockContext.VerifyGet(m=>m.Revisions);
        }
    }
}