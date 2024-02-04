
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Estranged.Lfs.Data;
using Estranged.Lfs.Data.Entities;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace Estranged.Lfs.Adapter.DynamoDB
{
    public class DynamoDBLockAdapter : ILockAdapter
    {
        private readonly IAmazonDynamoDB client;
        private readonly DynamoDBContextConfig config;

        public DynamoDBLockAdapter(IAmazonDynamoDB client, IDynamoDBLockAdapterConfig config)
        {
            this.client = client;
            this.config = new DynamoDBContextConfig { TableNamePrefix = config.TableNamePrefix };
        }

        public async Task<Lock> CreateLock(string path, string owner, string refSpec)
        {
            var l = new Lock()
            {
                LockId = Guid.NewGuid().ToString(),
                Path = path,
                Owner = new User() { Name = owner },
                LockedAt = DateTime.Now,
                RefSpec = refSpec
            };
            var context = new DynamoDBContext(this.client, this.config);
            await context.SaveAsync(l);

            return l;
        }

        public async Task<Lock> DeleteLock(string id, string user, string refSpec, bool force)
        {
            var context = new DynamoDBContext(this.client, this.config);
            var l = await context.LoadAsync<Lock>(id);
            if (l == null) return null;

            if ((l.Owner.Name != user || (l.RefSpec != null && l.RefSpec != refSpec)) && !force) return null;
            await context.DeleteAsync(l);
            return l;
        }

        public async Task<Tuple<IEnumerable<Lock>, string>> FilterdLocks(string path, string refSpec, string cursor, int limit)
        {
            var context = new DynamoDBContext(this.client, this.config);
            var locks = await context.ScanAsync<Lock>(null).GetRemainingAsync();
            var filteredLocks = new List<Lock>();

            bool Found = (cursor == null || cursor.Length == 0);
            foreach (var l in locks)
            {
                if ((path == null || path.Length == 0 || l.Path.StartsWith(path)) &&
                    (refSpec == null || refSpec.Length == 0 || refSpec == l.RefSpec))
                {
                    if (!Found)
                    {
                        if (l.LockId == cursor) Found = true;
                        continue;
                    } 

                    filteredLocks.Add(l);
                    if (limit > 0 && filteredLocks.Count >= limit)
                    {
                        return Tuple.Create<IEnumerable<Lock>, string>(filteredLocks, l.LockId);
                    }
                }
            }
            
            
            return Tuple.Create<IEnumerable<Lock>, string>(filteredLocks, "");
        }
    }
}
