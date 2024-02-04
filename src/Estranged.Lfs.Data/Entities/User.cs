using System;
using System.Runtime.Serialization;
using Amazon.DynamoDBv2.DataModel;

namespace Estranged.Lfs.Data.Entities
{
    [DataContract]
    public  class User
    {
        [DataMember(Name = "name"), DynamoDBHashKey("name")]
        public string Name { get; set; }
    }
}
