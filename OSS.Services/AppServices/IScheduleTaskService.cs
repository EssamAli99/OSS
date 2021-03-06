using OSS.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OSS.Services.AppServices
{
    /// <summary>
    /// Task service interface
    /// </summary>
    public partial interface IScheduleTaskService
    {
        /// <summary>
        /// Deletes a task
        /// </summary>
        /// <param name="task">Task</param>
        Task DeleteAsync(ScheduleTask task);

        /// <summary>
        /// Gets a task
        /// </summary>
        /// <param name="taskId">Task identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the schedule task
        /// </returns>
        Task<ScheduleTask> GetByIdAsync(int taskId);

        /// <summary>
        /// Gets a task by its type
        /// </summary>
        /// <param name="type">Task type</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the schedule task
        /// </returns>
        Task<ScheduleTask> GetByTypeAsync(string type);

        /// <summary>
        /// Gets all tasks
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of schedule task
        /// </returns>
        Task<IList<ScheduleTask>> GetAllAsync(bool showHidden = false);

        /// <summary>
        /// Inserts a task
        /// </summary>
        /// <param name="task">Task</param>
        Task InsertTaskAsync(ScheduleTask task);

        /// <summary>
        /// Updates the task
        /// </summary>
        /// <param name="task">Task</param>
        Task UpdateAsync(ScheduleTask task);
    }
}
