using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Estranged.Lfs.Data.Entities;

namespace Estranged.Lfs.Data
{
    public interface ILockManager
    {
        Task<(bool, Lock)> CreateLock(string path, string owner, string refSpec, CancellationToken token);
        Task<Lock> DeleteLock(string id, string user, string refSpec, bool force, CancellationToken token);
        Task<(IEnumerable<Lock>, string)> Locks(string path, string refSpec, string id, string cursor, int limit, CancellationToken token);
        Task<(IEnumerable<Lock>, string)> VerifiedLocks(string refSpec, string cursor, int limit, CancellationToken token);
    }
}