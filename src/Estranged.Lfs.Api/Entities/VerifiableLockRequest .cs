using Estranged.Lfs.Data.Entities;
using System.Runtime.Serialization;

namespace Estranged.Lfs.Api.Entities
{
    [DataContract]
    public class VerifiableLockRequest
    {
        [DataMember(Name = "cursor")]
        public string Cursor { get; set; }
        [DataMember(Name = "limit")]
        public int Limit { get; set; }
        [DataMember(Name = "ref")]
        public RefSpec Ref { get; set; }
    }
}
