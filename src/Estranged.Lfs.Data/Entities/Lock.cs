using System;
using System.Runtime.Serialization;
using Amazon.DynamoDBv2.DataModel;

namespace Estranged.Lfs.Data.Entities
{
    [DataContract, DynamoDBTable("Locks")]
    public  class Lock
    {
        [DataMember(Name = "lockid"), DynamoDBHashKey("lockid")]
        public string LockId { get; set; }
        [DataMember(Name = "path"), DynamoDBProperty("path")]
        public string Path { get; set; }
        [DataMember(Name = "owner"), DynamoDBProperty("owner")]
        public User Owner {  get; set; }
        [DataMember(Name = "locked_at"), DynamoDBProperty("locked_at")]
        public DateTime LockedAt { get; set; }
        [DataMember(Name = "ref_spec"), DynamoDBProperty("ref_spec")]
        public String RefSpec { get; set; }

    }
}
