using Amazon.DynamoDBv2;
using Estranged.Lfs.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Estranged.Lfs.Adapter.DynamoDB
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLfsDynamoDBAdapter(this IServiceCollection services, IDynamoDBLockAdapterConfig config, IAmazonDynamoDB amazonDynamoDB)
        {
            return services.AddSingleton<ILockAdapter, DynamoDBLockAdapter>().AddSingleton(amazonDynamoDB).AddSingleton(config);
        }
    }
}