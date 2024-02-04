using Estranged.Lfs.Data.Entities;
using System.Runtime.Serialization;

namespace Estranged.Lfs.Api.Entities
{
    [DataContract]
    public class LockRequest
    {
        [DataMember(Name = "path")]
        public string Path { get; set; }
        [DataMember(Name = "ref")]
        public RefSpec Ref { get; set; }
    }
}
