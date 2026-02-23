using NSubstitute;
using Xunit;
using FluentAssertions;
using OSS.Data;
using OSS.Data.Entities;
using OSS.Services.AppServices;
using OSS.Services.DomainServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSS.Services.Tests
{
    public class AppPageServiceTests
    {
        private IAppPageService _service;

        public AppPageServiceTests()
        {
            var page1 = new AppPage
            {
                ActionName = "actionname",
                Id = 1,
                Title = "title",
                SystemName = "s",
                ControllerName = "c",
                AreaName = "a",
                IconClass = "i",
                PermissionNames = "p"
            };
            var pages = new List<AppPage>
            {
                new AppPage
                {
                    ActionName = "action2",
                    Id =2,
                    Title = "title2",
                    SystemName = "s2",
                    ControllerName = "c2",
                    AreaName = "a2",
                    IconClass = "i2",
                    PermissionNames = "p2"
                }
            };
            pages.Add(page1);

            var repositorySub = Substitute.For<IRepository<AppPage>>();
            repositorySub.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult(page1));

            // AsQueryable returns IQueryable
            var queryablePages = pages.AsQueryable();
            repositorySub.Table.Returns(queryablePages);
            repositorySub.TableNoTracking.Returns(queryablePages);

            var cachemanger = Substitute.For<ICacheManager>();
            cachemanger.Get(Arg.Any<string>(), Arg.Any<System.Func<Task<List<AppPage>>>>(), Arg.Any<int?>()).Returns(Task.FromResult(pages));

            _service = new AppPageService(repositorySub, cachemanger);
        }

        [Fact]
        public async Task Service_GetById_Return_Object()
        {
            var p = await _service.GetAppPagesAsync();

            p.Count.Should().Be(2);
        }
    }
}
