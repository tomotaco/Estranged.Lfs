using Estranged.Lfs.Data.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Estranged.Lfs.Data
{
    public class LockManager : ILockManager
    {
        private readonly ILockAdapter lockAdapter;

        public LockManager(ILockAdapter lockAdapter)
        {
            this.lockAdapter = lockAdapter;
        }

        public async Task<(bool, Lock)> CreateLock(string path, string owner, string refSpec, CancellationToken token)
        {
            var (found, l) = await this.lockAdapter.CreateLock(path, owner, refSpec, token);
            return (found, l);
        }

        public async Task<Lock> DeleteLock(string id, string user, string refSpec, bool force, CancellationToken token)
        {
            var l = await this.lockAdapter.DeleteLock(id, user, refSpec, force, token);
            return l;
        }

        public async Task<(IEnumerable<Lock>, string)> Locks(string path, string refSpec, string id, string cursor, int limits, CancellationToken token)
        {
            return await this.lockAdapter.Locks(path, refSpec, id, cursor, limits, token);
        }

        public async Task<(IEnumerable<Lock>, string)> VerifiedLocks(string refSpec, string cursor, int limits, CancellationToken token)
        {
            return await this.lockAdapter.VerifiedLocks(refSpec, cursor, limits, token);
        }
    }
}
