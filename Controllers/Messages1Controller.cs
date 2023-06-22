using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimal_Chat_App.Models;
using Minimal_Chat_App.Services;

namespace Minimal_Chat_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Messages1Controller : ControllerBase
    {
        private readonly AppDbContext _context;

        public Messages1Controller(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Messages1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
        {
          if (_context.Messages == null)
          {
              return NotFound();
          }
            return await _context.Messages.ToListAsync();
        }

        // GET: api/Messages1/5


        [HttpGet("api/conversations/{id}")]
        public IActionResult GetConversationHistory(string id, DateTime? before = null, int count = 20, string sort = "asc")
        {
            // TODO: Validate the bearer token and check for authorization

            // Filter the messages based on the user ID
            var userMessages = _context.Messages.Where(m => m.SenderId == id || m.ReceiverId == id);

            // Apply the before timestamp filter if provided
            if (before != null)
            {
                userMessages = userMessages.Where(m => m.Timestamp < before.Value);
            }

            // Sort the messages based on the sort parameter
            if (sort.ToLower() == "desc")
            {
                userMessages = userMessages.OrderByDescending(m => m.Timestamp);
            }
            else
            {
                userMessages = userMessages.OrderBy(m => m.Timestamp);
            }

            // Take the specified number of messages
            userMessages = userMessages.Take(count);

            // Return the conversation history
            return Ok(userMessages);
        }


        // PUT: api/Messages1/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMessage(string id, Message message)
        {
            if (id != message.MessageId)
            {
                return BadRequest();
            }

            _context.Entry(message).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessageExists(id))
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

        // POST: api/Messages1
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Message>> PostMessage(Message message)
        {
          if (_context.Messages == null)
          {
              return Problem("Entity set 'AppDbContext.Messages'  is null.");
          }
            _context.Messages.Add(message);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MessageExists(message.MessageId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMessage", new { id = message.MessageId }, message);
        }

        // DELETE: api/Messages1/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(string id)
        {
            if (_context.Messages == null)
            {
                return NotFound();
            }
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MessageExists(string id)
        {
            return (_context.Messages?.Any(e => e.MessageId == id)).GetValueOrDefault();
        }
    }
}
