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
    public partial class ScheduleTaskService : IScheduleTaskService
    {
        #region Fields

        private readonly ApplicationDbContext _ctx;

        #endregion

        #region Ctor

        public ScheduleTaskService(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a task
        /// </summary>
        /// <param name="task">Task</param>
        public virtual async Task DeleteAsync(ScheduleTask task)
        {
             _ctx.ScheduleTask.Remove(task);
            await _ctx.SaveChangesAsync();
        }

        /// <summary>
        /// Gets a task
        /// </summary>
        /// <param name="taskId">Task identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the schedule task
        /// </returns>
        public virtual async Task<ScheduleTask> GetByIdAsync(int taskId)
        {
            return await _ctx.ScheduleTask.FindAsync(taskId);
        }

        /// <summary>
        /// Gets a task by its type
        /// </summary>
        /// <param name="type">Task type</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the schedule task
        /// </returns>
        public virtual async Task<ScheduleTask> GetByTypeAsync(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return null;

            return await _ctx.ScheduleTask.Where(st => st.Type == type)
                .OrderByDescending(t => t.Id).FirstOrDefaultAsync();

        }

        /// <summary>
        /// Gets all tasks
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of schedule task
        /// </returns>
        public virtual async Task<IList<ScheduleTask>> GetAllAsync(bool showHidden = false)
        {
            var query = _ctx.ScheduleTask.AsNoTracking();
            if (!showHidden) query = query.Where(t => t.Enabled);
            return await query.OrderByDescending(t => t.Seconds).ToListAsync();
        }

        /// <summary>
        /// Inserts a task
        /// </summary>
        /// <param name="task">Task</param>
        public virtual async System.Threading.Tasks.Task InsertTaskAsync(ScheduleTask task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            await _ctx.ScheduleTask.AddAsync(task);
            await _ctx.SaveChangesAsync();
        }

        /// <summary>
        /// Updates the task
        /// </summary>
        /// <param name="task">Task</param>
        public virtual async System.Threading.Tasks.Task UpdateAsync(ScheduleTask task)
        {
            _ctx.ScheduleTask.Update(task);
            await _ctx.SaveChangesAsync();
        }

        #endregion
    }
}
