using Estranged.Lfs.Data.Entities;
using System.Runtime.Serialization;

namespace Estranged.Lfs.Api.Entities
{
    [DataContract]
    public class LockResponse
    {
        [DataMember(Name ="lock")]
        public Lock Lock { get; set; }
        [DataMember(Name = "Message")]
        public string Message { get; set; }
    }
}
