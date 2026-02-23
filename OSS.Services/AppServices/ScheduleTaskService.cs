#nullable disable
using Microsoft.EntityFrameworkCore;
using OSS.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSS.Services.AppServices
{
    /// <summary>
    /// Task service
    /// </summary>
    public class ScheduleTaskService : IScheduleTaskService
    {


        private readonly IRepository<ScheduleTask> _repository;





        public ScheduleTaskService(IRepository<ScheduleTask> ctx)
        {
            _repository = ctx;
        }





        public virtual async Task DeleteAsync(ScheduleTask task)
        {
            await _repository.DeleteAsync(task);
        }

        public virtual async Task<ScheduleTask> GetByIdAsync(int taskId)
        {
            return await _repository.GetByIdAsync(taskId);
        }

        public virtual async Task<ScheduleTask> GetByTypeAsync(string type)
        {
            if (string.IsNullOrWhiteSpace(type)) return null!;

            return await _repository.Table.Where(st => st.Type == type)
                .OrderByDescending(t => t.Id).FirstOrDefaultAsync();
        }

        public virtual async Task<IList<ScheduleTask>> GetAllAsync(bool showHidden = false)
        {
            var query = _repository.TableNoTracking;
            if (!showHidden) query = query.Where(t => t.Enabled);
            return await query.OrderByDescending(t => t.Seconds).ToListAsync();
        }

        public virtual async Task InsertTaskAsync(ScheduleTask task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            await _repository.InsertAsync(task);
        }

        public virtual async Task UpdateAsync(ScheduleTask task)
        {
            await _repository.UpdateAsync(task);
        }


    }
}
