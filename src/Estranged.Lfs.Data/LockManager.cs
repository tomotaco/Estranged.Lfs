using Estranged.Lfs.Data.Entities;
using System;
using System.Collections.Generic;
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

        public async Task<Lock> CreateLock(string path, string owner, string refSpec)
        {
            var l = await this.lockAdapter.CreateLock(path, owner, refSpec);
            return l;
        }

        public async Task<Lock> DeleteLock(string id, string user, string refSpec, bool force)
        {
            var l = await this.lockAdapter.DeleteLock(id, user, refSpec, force);
            return l;
        }

        public async Task<Tuple<IEnumerable<Lock>, string>> FilterdLocks(string path, string refSpec, string cursor, int limit)
        {
            var locks = await this.lockAdapter.FilterdLocks(path, refSpec, cursor, limit);
            return locks;
        }
    }
}
