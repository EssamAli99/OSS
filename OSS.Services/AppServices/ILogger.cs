using OSS.Data.Entities;
using OSS.Services.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OSS.Services.AppServices
{
    public partial interface ILogger
    {
        void Delete(string id);
        IPagedList<Log> GetAllLogs(DateTime? fromUtc = null, DateTime? toUtc = null,
            string message = "", int? logLevel = null,
            int pageIndex = 0, int pageSize = int.MaxValue);

        LogModel PrepareModel(string logId);
        IList<Log> GetLogByIds(int[] logIds);
        int Insert(LogModel model);
        IList<LogModel> GetAll(DateTime? fromUtc = null, DateTime? toUtc = null, string userid = "",
                int? logLevel = null, string message = "");

        Task ClearLogAsync();

    }
}
