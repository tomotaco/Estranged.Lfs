using Estranged.Lfs.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Estranged.Lfs.Data
{
    public interface ILockAdapter
    {
        Task<Lock> CreateLock(string path, string owner, string refSpec);
        Task<Lock> DeleteLock(string id, string user, string refSpec, bool force);
        Task<Tuple<IEnumerable<Lock>, string>> FilterdLocks(string path, string refSpec, string cursor, int limit);
        
    }
}
