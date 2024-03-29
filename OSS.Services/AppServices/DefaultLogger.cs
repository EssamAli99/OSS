﻿using Microsoft.EntityFrameworkCore;
using OSS.Data.Entities;
using OSS.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSS.Services.AppServices
{
    public partial class DefaultLogger : ILogger
    {

        private readonly IRepository<Log> _ctx;
        public DefaultLogger(IRepository<Log> ctx)
        {
            _ctx = ctx;
        }
        private async Task<Log> GetEntity(string encryptedId)
        {
            //TODO: add data protection encrypt and decrypt
            int Id = 0;
            if (!string.IsNullOrEmpty(encryptedId)) Int32.TryParse(encryptedId, out Id);
            if (Id > 0) return await _ctx.GetByIdAsync(Id);
            return null;
        }
        public virtual async Task Delete(string id)
        {
            var log = await GetEntity(id);
            if (log == null) throw new ArgumentNullException(nameof(log));

            await _ctx.DeleteAsync(log);
        }

        public virtual IPagedList<Log> GetAllLogs(DateTime? fromUtc = null, DateTime? toUtc = null,
            string message = "", int? logLevel = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _ctx.TableNoTracking;
            if (fromUtc.HasValue)
                query = query.Where(l => fromUtc.Value <= l.CreatedOnUtc);
            if (toUtc.HasValue)
                query = query.Where(l => toUtc.Value >= l.CreatedOnUtc);
            if (logLevel.HasValue)
            {
                var logLevelId = logLevel.Value;
                query = query.Where(l => logLevelId == l.LogLevelId);
            }

            if (!string.IsNullOrEmpty(message))
                query = query.Where(l => l.ShortMessage.Contains(message) || l.FullMessage.Contains(message));
            query = query.OrderByDescending(l => l.CreatedOnUtc);

            return new PagedList<Log>(query, pageIndex, pageSize);
        }

        public virtual async Task<LogModel> PrepareModel(string logId)
        {
            var log = await GetEntity(logId);
            if (log != null)
            {
                return new LogModel
                {
                    CreatedOnUtc = log.CreatedOnUtc,
                    EncrypedId = log.Id.ToString(),
                    FullMessage = log.FullMessage,
                    IpAddress = log.IpAddress,
                    LogLevelId = log.LogLevelId,
                    ModelMode = ModelActions.List,
                    PageUrl = log.PageUrl,
                    ReferrerUrl = log.ReferrerUrl,
                    ShortMessage = log.ShortMessage,
                    UserId = log.UserId
                };
            }
            return null;

        }

        public virtual async Task<IList<Log>> GetLogByIds(int[] logIds)
        {
            if (logIds == null || logIds.Length == 0)
                return new List<Log>();

            var query = from l in _ctx.TableNoTracking
                        where logIds.Contains(l.Id)
                        select l;
            return await query.ToListAsync();

            ////sort by passed identifiers
            //var sortedLogItems = new List<Log>();
            //foreach (var id in logIds)
            //{
            //    var log = logItems.Find(x => x.Id == id);
            //    if (log != null)
            //        sortedLogItems.Add(log);
            //}

            //return sortedLogItems;
        }

        public virtual async Task<int> Insert(LogModel model)
        {
            if (model == null || string.IsNullOrEmpty(model?.ShortMessage)) return 0;

            var log = new Log
            {
                LogLevelId = model.LogLevelId,
                ShortMessage = model.ShortMessage,
                FullMessage = model.FullMessage,
                IpAddress = model.IpAddress, //_webHelper.GetCurrentIpAddress(),
                UserId = string.IsNullOrEmpty(model.UserId) ? null : model.UserId,
                PageUrl = model.PageUrl, //_webHelper.GetThisPageUrl(true),
                ReferrerUrl = model.ReferrerUrl, //_webHelper.GetUrlReferrer(),
                CreatedOnUtc = DateTime.UtcNow
            };

            await _ctx.InsertAsync(log);

            return log.Id;
        }

        public virtual async Task<IList<LogModel>> GetAll(DateTime? fromUtc = null, DateTime? toUtc = null, string userid = "",
                int? logLevel = null, string message = "")
        {
            var query = _ctx.TableNoTracking.Include(x => x.User).AsQueryable();
            if (fromUtc.HasValue)
                query = query.Where(l => fromUtc.Value <= l.CreatedOnUtc);
            if (toUtc.HasValue)
                query = query.Where(l => toUtc.Value >= l.CreatedOnUtc);
            if (logLevel.HasValue)
            {
                var logLevelId = logLevel.Value;
                query = query.Where(l => logLevelId == l.LogLevelId);
            }
            if (!string.IsNullOrEmpty(userid)) query = query.Where(l => l.UserId == userid);
            if (!string.IsNullOrEmpty(message))
                query = query.Where(l => l.ShortMessage.Contains(message) || l.FullMessage.Contains(message));

            //query = query.OrderByDescending(l => l.CreatedOnUtc);
            query = query.OrderByDescending(l => l.Id);

            var logs = await query.Select(x => new LogModel
            {
                CreatedOnUtc = x.CreatedOnUtc,
                FullMessage = x.FullMessage,
                IpAddress = x.IpAddress,
                LogLevelId = x.LogLevelId,
                PageUrl = x.PageUrl,
                ReferrerUrl = x.ReferrerUrl,
                ShortMessage = x.ShortMessage,
                UserId = x.UserId,
                EncrypedId = x.Id.ToString(),
                CreatedOn = x.CreatedOnUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm"),
                Email = x.User.Email,
                ModelMode = ModelActions.List
            }).ToListAsync();

            return logs;
        }

        public async Task ClearLogAsync()
        {
            var lst = _ctx.Table.OrderBy(x => x.Id).Take(100);
            await _ctx.DeleteAsync(lst);
        }

    }
}
