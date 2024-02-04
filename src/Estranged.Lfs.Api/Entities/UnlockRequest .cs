using Estranged.Lfs.Data.Entities;
using System.Runtime.Serialization;

namespace Estranged.Lfs.Api.Entities
{
    [DataContract]
    public class UnlockRequest
    {
        [DataMember(Name = "force")]
        public bool Force { get; set; }
        [DataMember(Name = "ref")]
        public RefSpec Ref { get; set; }
    }
}
