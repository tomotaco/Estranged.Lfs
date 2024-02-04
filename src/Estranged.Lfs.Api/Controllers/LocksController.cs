using Estranged.Lfs.Api.Entities;
using Estranged.Lfs.Data;
using Estranged.Lfs.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Estranged.Lfs.Api.Controllers
{
    [Route("locks")]
    public class LocksController : ControllerBase
    {
        private readonly ILockManager lockManager;
        private readonly IHostEnvironment hostEnvironmant;

        public LocksController(ILockManager lockManager, IHostEnvironment hostEnvironment)
        {
            this.lockManager = lockManager;
            this.hostEnvironmant = hostEnvironment;
        }
        
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<LockResponse>> LockAsync([FromBody] LockRequest request, [FromHeader(Name = "USER")] string user)
        {
            var lockFound = await this.lockManager.FilterdLocks(request.Path, request.Ref.Name, "", 1);
            if (lockFound != null && lockFound.Item1.Count<Lock>() > 0)
            {
                return Conflict(lockFound);
            }

            var l = await this.lockManager.CreateLock(request.Path, user, request.Ref.Name);
            var response = new LockResponse() {
                Lock = l,
                Message = "Successfully locked."
            };
            return Created("", response);
//            return new ActionResult<LockResponse>(response);
        }

        [HttpPost("{id}/unlock")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UnlockResponse>> UnlockAsync([FromRoute] string id, [FromBody] UnlockRequest request, [FromHeader(Name = "USER")] string user)
        {
            var lockDeleted = await this.lockManager.DeleteLock(id, user, request.Ref.Name, request.Force);
            if (lockDeleted == null)
            {
                return NotFound();
            }
            var unlockResponse = new UnlockResponse() { Lock = lockDeleted, Message = "Successfully deleted." };

            return Ok(unlockResponse);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<LockList>> LocksAsync([FromQuery] string path, [FromQuery] string refSpec, [FromQuery] string cursor, [FromQuery] int limit)
        {
            var tupleLocksFound = await this.lockManager.FilterdLocks(path, refSpec, cursor, limit);
            var locksResponse = new LockList()
            {
                Locks = tupleLocksFound.Item1,
                NextCursor = tupleLocksFound.Item2

            };
            return Ok(locksResponse);
        }

        [HttpPost("verify")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<VerifiableLockList>> VerifyLocksAsync([FromBody] VerifiableLockRequest request, [FromHeader] string user)
        {
            var tupleLocksFound = await this.lockManager.FilterdLocks("", request.Ref.Name, request.Cursor, request.Limit);
            var locksOurs = new List<Lock>();
            var locksTheirs = new List<Lock>();
            foreach (var l in tupleLocksFound.Item1)
            {
                if (l.Owner.Name.Equals(user))
                {
                    locksOurs.Add(l);
                } else
                {
                    locksTheirs.Add(l);
                }
            }
            var locksResponse = new VerifiableLockList()
            {
                Ours = locksOurs,
                Theirs = locksTheirs,
                NextCursor = tupleLocksFound.Item2

            };
            return Ok(locksResponse);
        }
    }
}
