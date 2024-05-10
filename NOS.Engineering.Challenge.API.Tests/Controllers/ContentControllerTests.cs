using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NOS.Engineering.Challenge.API.Controllers;
using NOS.Engineering.Challenge.API.Models;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;
using NOS.Engineering.Challenge.Repository;
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOS.Engineering.Challenge.API.Tests.Controllers
{
    /// <summary>
    /// Holds unit tests for the main API Controller in the project.
    /// </summary>
    public class ContentControllerTests
    {
        //GetContentsByFilter
        #region GetContentsByFilter

        [Fact]
        public void GetContentsByFilter_NoFilterString()
        {
            //Setup mock dependencies to inject into controller to be tested using Moq framework.
            var mockContentsManager = new Mock<IContentsManager>();
            var mockRepo = new Mock<IContentRepository>();
            var mockLogger = new Mock<ILogger<ContentController>>();

            //Setup the expected return value from the controller's database call below.
            mockRepo.Setup(repo => repo.GetContentsByFilter(""))
                .Returns(GetTestContentsAll());

            //Build the controller object, passing by DI the mock dependencies.
            var controller = new ContentController(mockContentsManager.Object, mockRepo.Object, mockLogger.Object);

            //Perform the action to be tested, will return data setup from mock repository above.
            var result = controller.GetContentsByFilter();

            //Assertions (test verifications)
            
            //Ensure success response.
            var viewResult = Assert.IsType<OkObjectResult>(result);
            
            //Ensure the reponse object is an IEnumerable<Content> (expected response type).
            var model = Assert.IsAssignableFrom<IEnumerable<Content>>(
                viewResult.Value);
            
            //Ensure that all the records are being returned, regardless of Title or Genres (empty filter string).
            Assert.Equal(10, model.Count());
        }

        [Fact]
        public void GetContentsByFilter_TitleFilterString()
        {
            //Setting up the filter string variable to avoid repeating literals.
            string filterString = "t2";

            //Setup mock dependencies to inject into controller to be tested using Moq framework.
            var mockContentsManager = new Mock<IContentsManager>();
            var mockRepo = new Mock<IContentRepository>();
            var mockLogger = new Mock<ILogger<ContentController>>();

            //Setup the expected return value from the controller's database call below.
            mockRepo.Setup(repo => repo.GetContentsByFilter(filterString))
                .Returns(GetTestContentsByTitle(filterString));

            //Build the controller object, passing by DI the mock dependencies.
            var controller = new ContentController(mockContentsManager.Object, mockRepo.Object, mockLogger.Object);

            //Perform the action to be tested, will return data setup from mock repository above.
            var result = controller.GetContentsByFilter(filterString);

            //Assertions (test verifications)

            //Ensure success response.
            var viewResult = Assert.IsType<OkObjectResult>(result);

            //Ensure the reponse object is an IEnumerable<Content> (expected response type).
            var model = Assert.IsAssignableFrom<IEnumerable<Content>>(
                viewResult.Value);

            //Ensure that only the records with the correct labels are being returned (based on filter string).
            for(int i = 0; i < model.Count(); i++)
                Assert.Contains(filterString, model.ElementAt(i).Title);
        }

        [Fact]
        public void GetContentsByFilter_GenreFilterString()
        {
            //Setting up the filter string variable to avoid repeating literals.
            string filterString = "g6";

            //Setup mock dependencies to inject into controller to be tested using Moq framework.
            var mockContentsManager = new Mock<IContentsManager>();
            var mockRepo = new Mock<IContentRepository>();
            var mockLogger = new Mock<ILogger<ContentController>>();

            //Setup the expected return value from the controller's database call below.
            mockRepo.Setup(repo => repo.GetContentsByFilter(filterString))
                .Returns(GetTestContentsByGenre(filterString));

            //Build the controller object, passing by DI the mock dependencies.
            var controller = new ContentController(mockContentsManager.Object, mockRepo.Object, mockLogger.Object);

            //Perform the action to be tested, will return data setup from mock repository above.
            var result = controller.GetContentsByFilter(filterString);

            //Assertions (test verifications)

            //Ensure success response.
            var viewResult = Assert.IsType<OkObjectResult>(result);

            //Ensure the reponse object is an IEnumerable<Content> (expected response type).
            var model = Assert.IsAssignableFrom<IEnumerable<Content>>(
                viewResult.Value);

            //Ensure that only the records with the correct labels are being returned (based on filter string).
            for (int i = 0; i < model.Count(); i++)
                Assert.Contains(model.ElementAt(i).GenreList, x => x.Genre.Contains(filterString));
        }

        #endregion

        //GetContent
        #region GetContent

        [Fact]
        public void GetContent_ExistingID()
        {
            //Setting up the param GUID variable to simplify.
            Guid contentID = new Guid("81144FDA-DC9A-4A71-9820-499F2BB57553");

            //Setup mock dependencies to inject into controller to be tested using Moq framework.
            var mockContentsManager = new Mock<IContentsManager>();
            var mockRepo = new Mock<IContentRepository>();
            var mockLogger = new Mock<ILogger<ContentController>>();

            //Setup the expected return value from the controller's database call below.
            mockRepo.Setup(repo => repo.GetContent(contentID))
                .Returns(GetTestContentByID(contentID));

            //Build the controller object, passing by DI the mock dependencies.
            var controller = new ContentController(mockContentsManager.Object, mockRepo.Object, mockLogger.Object);

            //Perform the action to be tested, will return data setup from mock repository above.
            var result = controller.GetContent(contentID);

            //Assertions (test verifications)

            //Ensure success response.
            var viewResult = Assert.IsType<OkObjectResult>(result);

            //Ensure the reponse object is a Content (expected response type).
            var model = Assert.IsAssignableFrom<Content>(
                viewResult.Value);

            //Ensure that the record with the correct ID is being returned.
            Assert.Equal(contentID, model.Id);
        }

        [Fact]
        public void GetContent_NonExistingID()
        {
            //Setting up the param GUID variable to simplify.
            Guid contentID = Guid.Empty;

            //Setup mock dependencies to inject into controller to be tested using Moq framework.
            var mockContentsManager = new Mock<IContentsManager>();
            var mockRepo = new Mock<IContentRepository>();
            var mockLogger = new Mock<ILogger<ContentController>>();

            //Setup the expected return value from the controller's database call below.
            mockRepo.Setup(repo => repo.GetContent(contentID))
                .Returns(GetTestContentByID(contentID));

            //Build the controller object, passing by DI the mock dependencies.
            var controller = new ContentController(mockContentsManager.Object, mockRepo.Object, mockLogger.Object);

            //Perform the action to be tested, will return data setup from mock repository above.
            var result = controller.GetContent(contentID);

            //Assertions (test verifications)

            //Ensure NotFound response (expected return from controller).
            var viewResult = Assert.IsType<OkObjectResult>(result);

        }

        #endregion
        
        //CreateContent

        //UpdateContent

        //DeleteContent

        //AddGenres

        //RemoveGenres

        //Auxiliary

        private List<Content> GetTestContentsAll()
        {
            return new List<Content>()
            {
                new Content()
                {
                    Id = new Guid(),
                    Title = "t1",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g1" },
                        new ContentGenre() { ID = new Guid(), Genre = "g2" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t2",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g3" },
                        new ContentGenre() { ID = new Guid(), Genre = "g4" }
                    }
                },
                new Content()
                {
                    Id = new Guid("81144FDA-DC9A-4A71-9820-499F2BB57553"),
                    Title = "t3",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g5" },
                        new ContentGenre() { ID = new Guid(), Genre = "g6" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t4",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g6" },
                        new ContentGenre() { ID = new Guid(), Genre = "g5" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t5",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g5" },
                        new ContentGenre() { ID = new Guid(), Genre = "g4" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t6",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g3" },
                        new ContentGenre() { ID = new Guid(), Genre = "g2" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t7",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g1" },
                        new ContentGenre() { ID = new Guid(), Genre = "g6" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t8",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g5" },
                        new ContentGenre() { ID = new Guid(), Genre = "g3" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t9",GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g2" },
                        new ContentGenre() { ID = new Guid(), Genre = "g4" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t10",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g5" },
                        new ContentGenre() { ID = new Guid(), Genre = "g6" }
                    }
                },
            };
        }

        private List<Content> GetTestContentsByTitle(string filter)
        {
            List<Content> toReturn = new ()
            {
                new Content()
                {
                    Id = new Guid(),
                    Title = "t1",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g1" },
                        new ContentGenre() { ID = new Guid(), Genre = "g2" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t2",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g3" },
                        new ContentGenre() { ID = new Guid(), Genre = "g4" }
                    }
                },
                new Content()
                {
                    Id = new Guid("81144FDA-DC9A-4A71-9820-499F2BB57553"),
                    Title = "t3",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g5" },
                        new ContentGenre() { ID = new Guid(), Genre = "g6" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t4",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g6" },
                        new ContentGenre() { ID = new Guid(), Genre = "g5" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t5",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g5" },
                        new ContentGenre() { ID = new Guid(), Genre = "g4" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t6",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g3" },
                        new ContentGenre() { ID = new Guid(), Genre = "g2" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t7",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g1" },
                        new ContentGenre() { ID = new Guid(), Genre = "g6" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t8",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g5" },
                        new ContentGenre() { ID = new Guid(), Genre = "g3" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t9",GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g2" },
                        new ContentGenre() { ID = new Guid(), Genre = "g4" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t10",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g5" },
                        new ContentGenre() { ID = new Guid(), Genre = "g6" }
                    }
                },
            };

            return toReturn.Where(x => x.Title.Contains(filter)).ToList();            
        }

        private List<Content> GetTestContentsByGenre(string filter)
        {
            List<Content> toReturn = new()
            {
                new Content()
                {
                    Id = new Guid(),
                    Title = "t1",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g1" },
                        new ContentGenre() { ID = new Guid(), Genre = "g2" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t2",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g3" },
                        new ContentGenre() { ID = new Guid(), Genre = "g4" }
                    }
                },
                new Content()
                {
                    Id = new Guid("81144FDA-DC9A-4A71-9820-499F2BB57553"),
                    Title = "t3",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g5" },
                        new ContentGenre() { ID = new Guid(), Genre = "g6" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t4",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g6" },
                        new ContentGenre() { ID = new Guid(), Genre = "g5" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t5",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g5" },
                        new ContentGenre() { ID = new Guid(), Genre = "g4" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t6",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g3" },
                        new ContentGenre() { ID = new Guid(), Genre = "g2" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t7",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g1" },
                        new ContentGenre() { ID = new Guid(), Genre = "g6" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t8",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g5" },
                        new ContentGenre() { ID = new Guid(), Genre = "g3" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t9",GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g2" },
                        new ContentGenre() { ID = new Guid(), Genre = "g4" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t10",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g5" },
                        new ContentGenre() { ID = new Guid(), Genre = "g6" }
                    }
                },
            };

            return toReturn.Where(x => x.GenreList.Any(y => y.Genre.Contains(filter))).ToList();
        }

        private Content GetTestContentByID(Guid id)
        {
            List<Content> toReturn = new()
            {
                new Content()
                {
                    Id = new Guid(),
                    Title = "t1",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g1" },
                        new ContentGenre() { ID = new Guid(), Genre = "g2" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t2",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g3" },
                        new ContentGenre() { ID = new Guid(), Genre = "g4" }
                    }
                },
                new Content()
                {
                    Id = new Guid("81144FDA-DC9A-4A71-9820-499F2BB57553"),
                    Title = "t3",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g5" },
                        new ContentGenre() { ID = new Guid(), Genre = "g6" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t4",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g6" },
                        new ContentGenre() { ID = new Guid(), Genre = "g5" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t5",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g5" },
                        new ContentGenre() { ID = new Guid(), Genre = "g4" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t6",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g3" },
                        new ContentGenre() { ID = new Guid(), Genre = "g2" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t7",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g1" },
                        new ContentGenre() { ID = new Guid(), Genre = "g6" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t8",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g5" },
                        new ContentGenre() { ID = new Guid(), Genre = "g3" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t9",GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g2" },
                        new ContentGenre() { ID = new Guid(), Genre = "g4" }
                    }
                },
                new Content()
                {
                    Id = new Guid(),
                    Title = "t10",
                    GenreList = new List<ContentGenre>()
                    {
                        new ContentGenre() { ID = new Guid(), Genre = "g5" },
                        new ContentGenre() { ID = new Guid(), Genre = "g6" }
                    }
                },
            };

            return toReturn.FirstOrDefault(x => x.Id == id);
        }

        private Content GetTestContentCreateReturnObject(ContentInput inputData)
        {
            return new Content()
            {
                Id = new Guid(),
                Title = inputData.Title,
                SubTitle = inputData.SubTitle,
                Description = inputData.Description,
                Duration = (int)inputData.Duration,
                StartTime = (DateTime)inputData.StartTime,
                EndTime = (DateTime)inputData.EndTime,
                ImageUrl = inputData.ImageUrl
            };
        }
    }
}
