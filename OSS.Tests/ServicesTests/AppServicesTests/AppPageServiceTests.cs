using Moq;
using NUnit.Framework;
using OSS.Data.Entities;
using OSS.Services.AppServices;
using OSS.Services.DomainServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSS.Services.Tests
{
    [TestFixture]
    public class AppPageServiceTests
    {
        private IAppPageService _service;
        //private IRepository<AppPage> _repository;
        //private AppPage page1;
        [SetUp]
        public void Setup()
        {
            var page1 = new AppPage
            {
                ActionName = "actionname",
                Id = 1,
                Title = "title"
            };
            var pages = new List<AppPage>
            {
                new AppPage
                {
                    ActionName = "action2",
                    Id =2,
                    Title = "title2"
                }
            };
            pages.Add(page1);

            var repositoryMock = new Mock<IRepository<AppPage>>();
            repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()).Result).Returns(page1);
            repositoryMock.Setup(r => r.Table).Returns(pages.AsQueryable);
            repositoryMock.Setup(r => r.TableNoTracking).Returns(pages.AsQueryable);

            var cachemanger = new Mock<ICacheManager>();
            cachemanger.Setup(c => c.Get(It.IsAny<string>(), It.IsAny<System.Func<List<AppPage>>>(), It.IsAny<int>())).Returns(pages);
            _service = new AppPageService(repositoryMock.Object, cachemanger.Object);
        }

        [Test]
        public async Task Service_GetById_Return_Object()
        {
            var p = await _service.GetAppPagesAsync();

            Assert.NotNull(p);
            Assert.AreEqual(p.Count, 2);
        }
    }
}