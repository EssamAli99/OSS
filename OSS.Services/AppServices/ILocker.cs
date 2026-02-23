using System;

namespace OSS.Services.AppServices
{
    /// <summary>
    /// Provides exclusive distributed-style locking backed by the cache layer.
    /// </summary>
    public interface ILocker
    {
        /// <summary>
        /// Perform an action under an exclusive lock. Returns false if the lock is already held.
        /// </summary>
        bool PerformActionWithLock(string resource, TimeSpan expirationTime, Action action);
    }
}
