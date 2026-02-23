using Microsoft.EntityFrameworkCore;
using OSS.Data.Entities;
using OSS.Services.Models;
using System.Linq.Expressions;

namespace OSS.Services.DomainServices
{
    public class TestTableService : ITestTableService
    {
        private readonly IRepository<TestTable> _repository;

        public TestTableService(IRepository<TestTable> repository)
            => _repository = repository;

        public async Task<int> GetTotal(Expression<Func<TestTable, bool>>? where = null)
            => await _repository.GetAll(where).CountAsync();

        public async Task<IPagedList<TestTableModel>> PrepareModePagedList(
            Dictionary<string, string> param, bool all = false)
        {
            int start = 0;
            int pageSize = 10;
            string orderBy = string.Empty;
            string orderDir = "asc";
            var currentPage = 1;
            Expression<Func<TestTable, bool>>? where = null;

            if (param?.Any() == true)
            {
                if (param.TryGetValue("draw", out var draw))
                    currentPage = int.Parse(draw);

                if (param.TryGetValue("start", out var startVal))
                    _ = int.TryParse(startVal, out start);

                if (param.TryGetValue("length", out var lenVal))
                    _ = int.TryParse(lenVal, out pageSize);

                if (param.TryGetValue("order[0][column]", out var col) && col != "0")
                {
                    orderBy = col;
                    if (param.TryGetValue("order[0][dir]", out var dir))
                        orderDir = dir;
                }

                if (param.TryGetValue("search[value]", out var search) && !string.IsNullOrEmpty(search))
                    where = x => x.Text1!.Contains(search) || x.Text2!.Contains(search);
            }

            var query = _repository.GetAll(where);

            if (!string.IsNullOrEmpty(orderBy))
                query = orderDir == "asc"
                    ? query.OrderBy(x => x.Text1)
                    : query.OrderByDescending(x => x.Text1);

            // Fix-6: manual mapping — no AutoMapper
            if (all)
            {
                var data = await query.Select(x => x.ToModel()).ToListAsync();
                return new PagedList<TestTableModel>(data, currentPage, pageSize, data.Count);
            }

            return new PagedList<TestTableModel>(
                query.Select(x => x.ToModel()), currentPage, pageSize);
        }

        public async Task<TestTableModel?> PrepareMode(int id)
        {
            if (id <= 0) return null;
            var entity = await _repository.GetByIdAsync(id);
            return entity?.ToModel();
        }

        public async Task<TestTableModel?> Save(TestTableModel model)
        {
            if (model is null) return null;

            TestTable entity;
            if (model.Id <= 0)
            {
                entity = new TestTable();
            }
            else
            {
                entity = await _repository.GetByIdAsync(model.Id)
                    ?? throw new InvalidOperationException($"Record '{model.Id}' not found.");
            }

            entity.Text1 = model.Text1;
            entity.Text2 = model.Text2;

            if (model.Id <= 0)
            {
                await _repository.InsertAsync(entity);
                model.Id = entity.Id;
            }
            else
            {
                await _repository.UpdateAsync(entity);
            }

            return model;
        }

        public async Task Delete(int id)
        {
            var entity = await _repository.GetByIdAsync(id)
                ?? throw new InvalidOperationException($"Record '{id}' not found.");
            await _repository.DeleteAsync(entity);
        }
    }
}
