using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamBoardApiServer.Models;

namespace TeamBoardApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardsController : ControllerBase
    {
        private readonly TeamBoardApiServerContext _context;

        public BoardsController(/*TeamBoardApiServerContext context*/)
        {
            var context = new TeamBoardApiServerContext();
            _context = context;
        }

        // GET: api/Boards
        [HttpGet]
        public object GetBoards()
        {
            return _context.Boards.Select(b => new
            {
                b.Id,
                b.Description,
                ItemsCount = b.Items.Count(),
                MemberCount = b.Members.Count(),
            });
        }

        [Route("{id}/Items")]
        public async Task<ActionResult> GetBoardItems([FromRoute] Guid id)
        {
            var items = await _context.Boards
                .Where(x => x.Id == id).Select(i => i.Items)
                .FirstOrDefaultAsync();

            return Ok(items);
        }

        [Route("{id}/Members")]
        public async Task<ActionResult> GetBoardMembers([FromRoute] Guid id)
        {
            var members = await _context.Boards
                .Where(x => x.Id == id)
                .Select(i => i.Members.Select(u => new
                {
                    u.User.Id,
                    u.User.Username,
                    u.User.Firstname,
                    u.User.Lastname
                }))
                .FirstOrDefaultAsync();

            return Ok(members);
        }

        [Route("{id}/Owner")]
        public async Task<ActionResult> GetBoardOwner([FromRoute] Guid id)
        {
            var owner = await _context.Boards
                .Where(x => x.Id == id).Select(i => i.Owner)
                .FirstOrDefaultAsync();

            return Ok(new
            {
                owner.Id,
                owner.Username,
                owner.Firstname,
                owner.Lastname,
            });
        }

        // GET: api/Boards/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBoard([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var board = await _context.Boards.FindAsync(id);

            if (board == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                board.Id,
                board.Description,
                ItemsCount = board.Items.Count(),
                MemberCount = board.Members.Count(),
            });
        }

        // PUT: api/Boards/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBoard([FromRoute] Guid id, [FromBody] Board board)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != board.Id)
            {
                return BadRequest();
            }

            _context.Entry(board).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BoardExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Boards
        [HttpPost]
        public async Task<IActionResult> PostBoard([FromBody] Board board)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Boards.Add(board);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBoard", new { id = board.Id }, board);
        }

        // DELETE: api/Boards/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBoard([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var board = await _context.Boards.FindAsync(id);
            if (board == null)
            {
                return NotFound();
            }

            _context.Boards.Remove(board);
            await _context.SaveChangesAsync();

            return Ok(board);
        }

        private bool BoardExists(Guid id)
        {
            return _context.Boards.Any(e => e.Id == id);
        }
    }
}