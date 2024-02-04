using Estranged.Lfs.Data.Entities;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Estranged.Lfs.Api.Entities
{
    [DataContract]
    public class LockList
    {
        [DataMember(Name ="locks")]
        public IEnumerable<Lock> Locks { get; set; }
        [DataMember(Name = "next_cursor")]
        public string NextCursor { get; set; }
        [DataMember(Name = "message")]
        public string Message { get; set; }
    }
}
