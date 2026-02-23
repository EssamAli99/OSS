namespace OSS.Web.Framework
{
    /// <summary>
    /// Interface that should be implemented by each task
    /// </summary>
    public interface IScheduleTask
    {
        /// <summary>
        /// Executes a task
        /// </summary>
        System.Threading.Tasks.Task ExecuteAsync();
    }
}
