using OSS.Data.Entities;
using OSS.Services.Models;
using System.Linq.Expressions;

namespace OSS.Services.DomainServices
{
    public interface ITestTableService
    {
        Task<TestTableModel?> PrepareMode(int id);
        Task<IPagedList<TestTableModel>> PrepareModePagedList(Dictionary<string, string> param, bool all = false);
        Task<TestTableModel?> Save(TestTableModel model);
        Task Delete(int id);
        Task<int> GetTotal(Expression<Func<TestTable, bool>>? where = null);
    }
}
