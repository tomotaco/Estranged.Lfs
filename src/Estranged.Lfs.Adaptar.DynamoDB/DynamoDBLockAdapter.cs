
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Estranged.Lfs.Data;
using Estranged.Lfs.Data.Entities;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace Estranged.Lfs.Adapter.DynamoDB
{
    public class DynamoDBLockAdapter : ILockAdapter
    {
        private readonly IAmazonDynamoDB client;
        private readonly DynamoDBContextConfig config;
        private DynamoDBContext context;

        private const string globalRefSpec = "refs/global";

        public DynamoDBLockAdapter(IAmazonDynamoDB client, IDynamoDBLockAdapterConfig config)
        {
            this.client = client;
            this.config = new DynamoDBContextConfig { TableNamePrefix = config.TableNamePrefix };
            this.context = new DynamoDBContext(this.client, this.config);
        }

        public async Task<(bool, Lock)> CreateLock(string path, string owner, string refSpec, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(refSpec))
            {
                refSpec = globalRefSpec;
            }

            var foundLock = await this.context.LoadAsync<Lock>(path, refSpec, token);
            if (foundLock != null)
            {
                return (true, foundLock);
            }

            var l = new Lock()
            {
                Id = Guid.NewGuid().ToString(),
                Path = path,
                Owner = new User() { Name = owner },
                LockedAt = DateTime.Now,
                RefSpec = refSpec
            };
            await this.context.SaveAsync(l, token);

            return (false, l);
        }


        public async Task<Lock> DeleteLock(string id, string user, string refSpec, bool force, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(refSpec))
            {
                refSpec = globalRefSpec;
            }

            var queryConfig = new QueryOperationConfig
            {
                Filter = new QueryFilter("Id", QueryOperator.Equal, id),
                IndexName = "IdIndex"
            };

            var locks = await this.context.FromQueryAsync<Lock>(queryConfig).GetRemainingAsync(token);
            if (locks.Count == 0)
            {
                return null;
            }
            var l = locks[0];

            if ((l.Owner.Name != user || (l.RefSpec != null && l.RefSpec != refSpec)) && !force) return null;
            await context.DeleteAsync(l, token);
            return l;
        }

        public async Task<(IEnumerable<Lock>, string)> Locks(string path, string refSpec, string id, string cursor, int limits, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (!string.IsNullOrEmpty(id))
            {
                return await this.QueryLocks("Id", "IdIndex", id, cursor, limits, token);
            }
            else if (!string.IsNullOrEmpty(path))
            {
                if (!string.IsNullOrEmpty(refSpec))
                {
                    var l = await this.context.LoadAsync<Lock>(path, refSpec, token);
                    return (new List<Lock>() { l }, null);
                } else
                {
                    return await this.QueryLocks("Path", null, path, cursor, limits, token);
                }
            } else if (!string.IsNullOrEmpty(refSpec))
            {
                return await this.QueryLocks("RefSpec", "RefSpecIndex", refSpec, cursor, limits, token);
            } else
            {
                var scanConfig = new ScanOperationConfig();
                if (!string.IsNullOrEmpty(cursor)) {
                    scanConfig.PaginationToken = cursor;
                }
                if (limits > 0)
                {
                    scanConfig.Limit = limits;
                }
                var search = this.context.GetTargetTable<Lock>().Scan(scanConfig);
                var items = await search.GetNextSetAsync(token);
                var locks = this.context.FromDocuments<Lock>(items);
                return (locks, search.PaginationToken);
            }
        }

        public async Task<(IEnumerable<Lock>, string)> VerifiedLocks(string refSpec, string cursor, int limit, CancellationToken token)
        {
            return await this.QueryLocks("RefSpec", "RefSpecIndex", refSpec, cursor, limit, token);
        }
        private async Task<(IEnumerable<Lock>, string)> QueryLocks(string propertyName, string indexName, string value, string cursor, int limits, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var queryConfig = new QueryOperationConfig();
            queryConfig.Filter = new QueryFilter(propertyName, QueryOperator.Equal, value);
            if (!string.IsNullOrEmpty(indexName))
            {
                queryConfig.IndexName = indexName;
            }
            if (!string.IsNullOrEmpty(cursor))
            {
                queryConfig.PaginationToken = cursor;
            }
            if (limits > 0)
            {
                queryConfig.Limit = limits;
            }

            var search = this.context.GetTargetTable<Lock>().Query(queryConfig);
            var items = await search.GetNextSetAsync(token);
            var locks = this.context.FromDocuments<Lock>(items);
            var paginationToken = search.PaginationToken;
            return (locks, search.PaginationToken);
        }
    }
}
