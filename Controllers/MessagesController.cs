using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimal_Chat_App.Models;
using Minimal_Chat_App.Services;

namespace Minimal_Chat_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MessagesController(AppDbContext context)
        {
            _context = context;
        }


        private int GetAuthenticatedUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            int.TryParse(userIdClaim?.Value, out int userId);
            return userId;
        }





        // GET: api/Messages
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
        {
          if (_context.Messages == null)
          {
              return NotFound();
          }
            return await _context.Messages.ToListAsync();
        }

        // GET: api/Messages/5
        [Authorize]
        [HttpGet("api/conversations/{id}")]
        public IActionResult GetConversationHistory(int id, DateTime? before = null, int count = 20, string sort = "asc")
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



        // PUT: api/Messages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{messageId}")]
        [Authorize] // Requires authentication to access this endpoint
        public IActionResult EditMessage(int messageId, [FromBody] EditMessageRequest editMessageDto)
        {
            // Validate the input
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int userId = GetAuthenticatedUserId();
            if (userId == 0)
            {
                Console.WriteLine("Failed to retrieve authenticated user ID.");
                return Unauthorized();
            }

            // Get the message from the database
            var message = _context.Messages.FirstOrDefault(m => m.MessageId == messageId);

            // Check if the message exists
            if (message == null)
            {
                return NotFound();
            }

            // Check if the authenticated user is the sender of the message
            if (message.SenderId != userId)
            {
                return Unauthorized();
            }

            // Update the message content
            message.Content = editMessageDto.Content;

            // Save the changes to the database
            _context.SaveChanges();

            // Prepare the response body
            var response = new
            {
                MessageId = message.MessageId,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                Content = message.Content,
                Timestamp = message.Timestamp
            };

            return Ok(response);
        }
        //POST: api/Messages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        [Authorize]
        [HttpPost]
        // Requires authentication to access this endpoint
        public IActionResult SendMessage([FromBody] SendMessageRequest messageDto)
        {
            // Validate the input
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int senderId = GetAuthenticatedUserId();
            if (senderId == 0)
            {
                Console.WriteLine("Failed to retrieve authenticated user ID.");
               // return Unauthorized();
            }

            // Create a new Message object
            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = messageDto.ReceiverId,
                Content = messageDto.Content,
                Timestamp = DateTime.UtcNow
            };

            // Save the message to the database
            _context.Messages.Add(message);
            _context.SaveChanges();

            // Prepare the response body
            var response = new SendMessageResponse
            {
                MessageId = message.MessageId,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                Content = message.Content,
                Timestamp = message.Timestamp
            };

            return Ok(response);
        }
        [Authorize]
        // DELETE: api/Messages/5
        [HttpDelete("{Messageid}")]
        public async Task<IActionResult> DeleteMessage(int Messageid)
        {
            if (_context.Messages == null)
            {
                return NotFound();
            }
            var message = await _context.Messages.FindAsync( Messageid);
            if (message == null)
            {
                return NotFound();
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MessageExists(int id)
        {
            return (_context.Messages?.Any(e => e.MessageId == id)).GetValueOrDefault();
        }
    }
}

    

