using System;
using System.Runtime.Serialization;
using Amazon.DynamoDBv2.DataModel;

namespace Estranged.Lfs.Data.Entities
{
    [DataContract, DynamoDBTable("Locks")]
    public  class Lock
    {
        [DataMember(Name = "Id"), DynamoDBGlobalSecondaryIndexHashKey("IdIndex")]
        public string Id { get; set; }
        [DataMember(Name = "Path"), DynamoDBHashKey("Path")]
        public string Path { get; set; }
        [DataMember(Name = "Owner"), DynamoDBProperty("Owner")]
        public User Owner {  get; set; }
        [DataMember(Name = "LockedAt"), DynamoDBProperty("LockedAt")]
        public DateTime LockedAt { get; set; }
        [DataMember(Name = "RefSpec"), DynamoDBRangeKey("RefSpec"), DynamoDBGlobalSecondaryIndexHashKey("RefSpecIndex")]
        public String RefSpec { get; set; }

    }
}
