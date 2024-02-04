namespace Estranged.Lfs.Adapter.DynamoDB
{
    public sealed class DynamoDBLockAdapterConfig : IDynamoDBLockAdapterConfig
    {
        public string TableNamePrefix { get; set; }
    }
}
