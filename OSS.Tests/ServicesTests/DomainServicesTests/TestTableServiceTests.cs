using AutoMapper;
using Moq;
using NUnit.Framework;
using OSS.Data.Entities;
using OSS.Services;
using OSS.Services.DomainServices;
using OSS.Services.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSS.Tests.ServicesTests.DomainServicesTests
{
    public class TestTableServiceTests
    {
        private ITestTableService _service;
        private List<TestTable> entities;
        private TestTable entity;
        [SetUp]
        public void Setup()
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

            var table = new TestAsyncEnumerable<TestTable>(entities).AsQueryable();
            //var table = entities.AsQueryable();
            var repositoryMock = new Mock<IRepository<TestTable>>();
            repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()).Result).Returns(entity);
            repositoryMock.Setup(r => r.Table).Returns(table);
            repositoryMock.Setup(r => r.TableNoTracking).Returns(table);
            _service = new TestTableService(repositoryMock.Object);


            ////Setup DbContext and DbSet mock  
            //var dbContextMock = new Mock<ApplicationDbContext>();
            //var dbSetMock = new Mock<DbSet<TestTable>>();
            //dbSetMock.Setup(s => s.FindAsync(It.IsAny<int>()).Result).Returns(entity);
            //dbContextMock.Setup(s => s.Set<TestTable>()).Returns(dbSetMock.Object);
            //var productRepository = new EfRepository<TestTable>(dbContextMock.Object);
            //_service = new TestTableService(productRepository);

            //create AutoMapper configuration
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(AllMapperConfiguration));
            });

            //register
            AutoMapperConfiguration.Init(config);
        }

        [Test]
        public async Task Service_GetById_Return_Object()
        {
            var model = await _service.PrepareMode(entity.Id.ToString());

            Assert.NotNull(model);
            Assert.IsInstanceOf<TestTableModel>(model);
            Assert.AreEqual(model.Text1, entity.Text1);
        }

        [Test]
        public async Task Service_PrepareMode_Return_List()
        {
            var list = await _service.PrepareModeListAsync(null);
            Assert.NotNull(list);

            Assert.AreEqual(entities.Count, list.Count());
        }

        [Test]
        public async Task Service_GetTotal_Return_Count()
        {
            var count = await _service.GetTotal(null);
            Assert.AreEqual(entities.Count, count);
        }

    }
}
