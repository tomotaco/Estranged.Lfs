namespace Estranged.Lfs.Adapter.DynamoDB
{
    public interface IDynamoDBLockAdapterConfig
    {
        string TableNamePrefix { get; }
    }
}
