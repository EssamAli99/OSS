using OSS.Data.Entities;
using OSS.Services.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OSS.Services.AppServices
{
    public partial interface ILogger
    {
        Task Delete(string id);
        IPagedList<Log> GetAllLogs(DateTime? fromUtc = null, DateTime? toUtc = null,
            string message = "", int? logLevel = null,
            int pageIndex = 0, int pageSize = int.MaxValue);

        Task<LogModel> PrepareModel(string logId);
        Task<IList<Log>> GetLogByIds(int[] logIds);
        Task<int> Insert(LogModel model);
        Task<IList<LogModel>> GetAll(DateTime? fromUtc = null, DateTime? toUtc = null, string userid = "",
                int? logLevel = null, string message = "");

        Task ClearLogAsync();

    }
}
