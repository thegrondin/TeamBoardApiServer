using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamBoardApiServer.Models;

namespace TeamBoardApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly TeamBoardApiServerContext _context;

        public UsersController(/*TeamBoardApiServerContext context*/)
        {

            var context = new TeamBoardApiServerContext();

            _context = context;

            /*_context.Boards.Add(new Board
            {
                Description = "Board 3",
                Owner = _context.Users.Find(Guid.Parse("45e3d615-5fbd-4d89-d708-08d6bfa945f1"))
            });

            _context.SaveChanges();*/
        }

        // GET: api/Users
        [HttpGet]
        public object GetUsers()
        {

            return _context.Users
                .Include(b => b.Boards)
                .Select(x => new
                {
                    x.Firstname,
                    x.Lastname,
                    x.Id,
                    x.Username,
                    Boards = x.Boards.Select(g => g.Board.Id)
                }).ToList();

        }

        [Route("{id}/boards/")]
        public object GetUserBoards(Guid id)
        {

            return _context.Users.Include(b => b.Boards)
                .Where(u => u.Id == id)
                .Select(b => b.Boards.Select(x => new
                {
                    x.Board.Id,
                    x.Board.Description,
                    Owner = new {
                        x.Board.Owner.Id
                    }
                }))
                .FirstOrDefault();
                
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(new {
                user.Id,
                user.Username,
                user.Firstname,
                user.Lastname
            });
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser([FromRoute] Guid id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }


        [HttpPost]
        [Route("{userId}/addParticipationTo/{boardId}")]
        public async Task<IActionResult> AddParticipationBoardToUser([FromRoute] Guid boardId, [FromRoute] Guid userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User user = await _context.Users
                .Include(b => b.Boards)
                .FirstOrDefaultAsync(i => i.Id == userId);

            user.Boards.Add(new BoardToUser
            {
                Board = await _context.Boards.FindAsync(boardId),
                User = user
            });

            _context.Entry(user).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }


        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}