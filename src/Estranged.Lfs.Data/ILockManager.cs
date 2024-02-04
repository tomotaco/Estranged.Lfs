using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Estranged.Lfs.Data.Entities;

namespace Estranged.Lfs.Data
{
    public interface ILockManager
    {
        Task<Lock> CreateLock(string path, string owner, string refSpec);
        Task<Lock> DeleteLock(string id, string user, string refSpec, bool force);
        Task<Tuple<IEnumerable<Lock>, string>> FilterdLocks(string path, string refSpec, string cursor, int limit);
    }
}