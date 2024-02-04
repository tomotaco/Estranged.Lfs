using Estranged.Lfs.Data.Entities;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Estranged.Lfs.Api.Entities
{
    [DataContract]
    public class VerifiableLockList
    {
        [DataMember(Name ="ours")]
        public IEnumerable<Lock> Ours { get; set; }
        [DataMember(Name = "thiers")]
        public IEnumerable<Lock> Theirs { get; set; }
        [DataMember(Name = "next_cursor")]
        public string NextCursor { get; set; }
        [DataMember(Name = "message")]
        public string Message { get; set; }
    }
}
