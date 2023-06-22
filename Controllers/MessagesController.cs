using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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


        private string GetAuthenticatedUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return userIdClaim?.Value;
        }


        // GET: api/Messages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
        {
          if (_context.Messages == null)
          {
              return NotFound();
          }
            return await _context.Messages.ToListAsync();
        }

        //// GET: api/Messages/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Message>> GetMessage(string id)
        //{
        //  if (_context.Messages == null)
        //  {
        //      return NotFound();
        //  }
        //    var message = await _context.Messages.FindAsync(id);

        //    if (message == null)
        //    {
        //        return NotFound();
        //    }

        //    return message;
        //}


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



        // PUT: api/Messages/5
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




        //[HttpPost]
        //[Authorize] // Requires authentication to access this endpoint
        //public IActionResult SendMessage([FromBody] SendMessageRequest messageDto)
        //{
        //    // Validate the input
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    string senderId = GetAuthenticatedUserId();
        //    //if (string.IsNullOrEmpty(senderId))
        //    //{
        //    //    Console.WriteLine("Failed to retrieve authenticated user ID.");
        //    //    return Unauthorized();
        //    //}

        //    // Create a new Message object
        //    var message = new Message
        //    {
        //        SenderId = senderId,
        //        ReceiverId = messageDto.ReceiverId,
        //        Content = messageDto.Content,
        //        Timestamp = DateTime.UtcNow
        //    };

        //    // Save the message to the database
        //    _context.Messages.Add(message);
        //    _context.SaveChanges();

        //    // Prepare the response body
        //    var response = new SendMessageResponse
        //    {
        //        MessageId = message.MessageId,
        //        SenderId = message.SenderId,
        //        ReceiverId = message.ReceiverId,
        //        Content = message.Content,
        //        Timestamp = message.Timestamp
        //    };

        //    return Ok(response);
        //}







        //[HttpPost]
        //public async Task<IActionResult> SendMessage([FromBody] Message request)
        //{
        //    // Validate request parameters
        //    if (string.IsNullOrEmpty(request.ReceiverId))
        //        return BadRequest("Receiver ID is required.");

        //    if (string.IsNullOrEmpty(request.Content))
        //        return BadRequest("Message content is required.");

        //    // Get the current user's ID from the token
        //    var userId = GetUserIdFromToken();

        //    // Check if the sender user exists
        //    var senderExists = await _context.Users.AnyAsync(u => u.UserId.ToString() == userId);
        //    if (!senderExists)
        //        return Unauthorized("Unauthorized access.");

        //    // Check if the receiver user exists
        //    var receiverExists = await _context.Users.AnyAsync(u => u.UserId.ToString() == request.ReceiverId);
        //    if (!receiverExists)
        //        return BadRequest("Receiver user does not exist.");

        //    // Create a new message
        //    var message = new Message
        //    {
        //        MessageId = Guid.NewGuid().ToString(),
        //        SenderId = userId,
        //        ReceiverId = request.ReceiverId,
        //        Content = request.Content,
        //        Timestamp = DateTime.UtcNow
        //    };

        //    // Save the message to the database
        //    _context.Messages.Add(message);
        //    await _context.SaveChangesAsync();

        //    // Create the response DTO
        //    var response = new Message
        //    {
        //        MessageId = message.MessageId,
        //        SenderId = message.SenderId,
        //        ReceiverId = message.ReceiverId,
        //        Content = message.Content,
        //        Timestamp = message.Timestamp
        //    };

        //    return Ok(response);
        //}



        // Helper method to get the user ID from the token


        //[HttpPost]
        //public async Task<IActionResult> SendMessage(Message request)
        //{
        //    // Retrieve the sender user based on the authenticated user
        //    string senderId = User.Identity.Name;
        //    Users sender = await _context.Users.FindAsync(senderId);
        //    if (sender == null)
        //    {
        //        return NotFound("Sender not found");
        //    }

        //    // Retrieve the receiver user
        //    Users receiver = await _context.Users.FindAsync(request.ReceiverId);
        //    if (receiver == null)
        //    {
        //        return NotFound("Receiver not found");
        //    }

        //    // Create a new message
        //    Message message = new Message
        //    {
        //        Content = request.Content,
        //        Timestamp = DateTime.UtcNow,
        //        SenderId = senderId,
        //        ReceiverId = request.ReceiverId
        //    };

        //    // Add the message to the context and save changes
        //    _context.Messages.Add(message);
        //    await _context.SaveChangesAsync();

        //    // Prepare the response
        //    SendMessageResponse response = new SendMessageResponse
        //    {
        //        MessageId = message.MessageId,
        //        SenderId = senderId,
        //        ReceiverId = request.ReceiverId,
        //        Content = message.Content,
        //        Timestamp = message.Timestamp
        //    };

        //    return Ok(response);
        //}




        //[HttpPost]
        //public IActionResult SendMessage([FromBody] SendMessageRequest messageDto)
        //{
        //    // Validate the input
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    string senderId = GetAuthenticatedUserId();
        //    if (senderId == null)
        //    {
        //        return Unauthorized();
        //    }

        //    // Create a new Message object
        //    var message = new Message
        //    {
        //        SenderId = senderId,
        //        ReceiverId = messageDto.ReceiverId,
        //        Content = messageDto.Content,
        //        Timestamp = DateTime.UtcNow
        //    };

        //    // Save the message to the database
        //    _context.Messages.Add(message);
        //    _context.SaveChanges();

        //    // Prepare the response body
        //    var response = new SendMessageResponse
        //    {
        //        MessageId = message.MessageId,
        //        SenderId = message.SenderId,
        //        ReceiverId = message.ReceiverId,
        //        Content = message.Content,
        //        Timestamp = message.Timestamp
        //    };

        //    return Ok(response);
        //}





        //private string GetUserIdFromToken()
        //{
        //    // Get the authorization header from the request
        //    var authHeader = Request.Headers["Authorization"].FirstOrDefault();

        //    if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        //    {
        //        var token = authHeader.Substring("Bearer ".Length);
        //        var tokenHandler = new JwtSecurityTokenHandler();
        //        var jwtToken = tokenHandler.ReadJwtToken(token);

        //        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        //        return userId;
        //    }

        //    return null;
        //}



        //private string CreateJwt(Users user)
        //{
        //    var jwtTokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes("veryverysecret.....");
        //    var identity = new ClaimsIdentity(new Claim[]
        //    {
        //           //new Claim(ClaimTypes.Role, user.Role),
        //           new Claim(ClaimTypes.Name,$"{user.FirstName} {user.LastName}")
        //    });

        //    var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = identity,
        //        Expires = DateTime.Now.AddDays(1),
        //        SigningCredentials = credentials
        //    };
        //    var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        //    return jwtTokenHandler.WriteToken(token);
        //}






        //POST: api/Messages
        //To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Message>> PostMessage(Message message)
        {
            if (_context.Messages == null)
            {
                return Problem("Entity set 'AppDbContext.Messages'  is null.");
            }
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMessage", new { id = message.MessageId }, message);
        }

        // DELETE: api/Messages/5
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



