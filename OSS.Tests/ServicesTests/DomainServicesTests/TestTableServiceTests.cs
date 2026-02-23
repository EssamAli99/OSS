using FluentAssertions;
using NSubstitute;
using OSS.Data.Entities;
using OSS.Services.DomainServices;
using System.Linq.Expressions;
using Xunit;

namespace OSS.Tests.ServicesTests.DomainServicesTests
{
    public class TestTableServiceTests
    {
        private ITestTableService _service;
        private List<TestTable> entities;
        private TestTable entity;

        public TestTableServiceTests()
        {
            entity = new TestTable
            {
                Id = 1,
                Text1 = "t1",
                Text2 = "t11"
            };

            entities = new List<TestTable>
            {
                new TestTable
                {
                    Id = 2,
                    Text1 = "t2",
                    Text2 = "t22"
                }
            };
            entities.Add(entity);

            var table = new TestAsyncEnumerable<TestTable>(entities);

            var repositorySub = Substitute.For<IRepository<TestTable>>();
            repositorySub.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult(entity));
            repositorySub.Table.Returns(table);
            repositorySub.TableNoTracking.Returns(table);
            repositorySub.GetAll(Arg.Any<Expression<Func<TestTable, bool>>>(), Arg.Any<List<string>>()).Returns(table);

            _service = new TestTableService(repositorySub);
        }

        [Fact]
        public async Task Service_GetById_Return_Object()
        {
            var model = await _service.PrepareMode(entity.Id);

            model.Text1.Should().Be(entity.Text1);
        }

        [Fact]
        public async Task Service_PrepareMode_Return_List()
        {
            var list = await _service.PrepareModePagedList(null, true);

            list.Count.Should().Be(entities.Count);
        }

        [Fact]
        public async Task Service_GetTotal_Return_Count()
        {
            var count = await _service.GetTotal(null);

            count.Should().Be(entities.Count);
        }
    }
}
