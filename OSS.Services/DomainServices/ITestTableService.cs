using OSS.Data.Entities;
using OSS.Services.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OSS.Services.DomainServices
{
    public interface ITestTableService
    {
        Task<TestTableModel> PrepareMode(string encryptedId);
        Task<IList<TestTableModel>> PrepareModePagedList(Dictionary<string, string> param);
        Task<IEnumerable<TestTableModel>> PrepareModeListAsync(Func<TestTable, bool>? where);
        Task<TestTableModel> Save(TestTableModel model);
        Task Delete(string encryptedId);
        Task<int> GetTotal(Func<TestTableModel, bool>? where);
    }
}
