using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Minimal_Chat_App.Services;
using Minimal_Chat_App.Models;

namespace Minimal_Chat_App.Controllers
{
    [ApiController]
    [Route("api/messages")]
    public class MessageController : ControllerBase
    {
        private List<Message> messages = new List<Message>();

        [HttpPost]
        public IActionResult SendMessage([FromBody] Message message)
        {
            // Check if the required fields are present in the request
            if (string.IsNullOrEmpty(message.ReceiverId) || string.IsNullOrEmpty(message.Content))
            {
                return BadRequest("Receiver ID and message content are required.");
            }

            // TODO: Validate the bearer token and check for authorization

            // Generate a unique message ID
            message.MessageId = Guid.NewGuid().ToString();

            // Set the timestamp to the current time
            message.Timestamp = DateTime.Now;

            // Add the message to the list of messages
            messages.Add(message);

            // Return the successful response with the created message
            return Ok(message);
        }

        [HttpPut("{messageId}")]
        public IActionResult EditMessage(string messageId, [FromBody] string updatedContent)
        {
            // Find the message with the given message ID
            Message message = messages.FirstOrDefault(m => m.MessageId == messageId);

            // Check if the message exists
            if (message == null)
            {
                return NotFound("Message not found.");
            }

            // TODO: Check if the user is authorized to edit the message (compare senderId with the authenticated user)

            // Update the message content
            message.Content = updatedContent;

            // Return the successful response
            return Ok("Message edited successfully.");
        }

        [HttpDelete("{messageId}")]
        public IActionResult DeleteMessage(string messageId)
        {
            // Find the message with the given message ID
            Message message = messages.FirstOrDefault(m => m.MessageId == messageId);

            // Check if the message exists
            if (message == null)
            {
                return NotFound("Message not found.");
            }

            // TODO: Check if the user is authorized to delete the message (compare senderId with the authenticated user)

            // Remove the message from the list of messages
            messages.Remove(message);

            // Return the successful response
            return Ok("Message deleted successfully.");
        }

        [HttpGet("api/conversations/{userId}")]
        public IActionResult GetConversationHistory(string userId, DateTime? before = null, int count = 20, string sort = "asc")
        {
            // TODO: Validate the bearer token and check for authorization

            // Filter the messages based on the user ID
            var userMessages = messages.Where(m => m.SenderId == userId || m.ReceiverId == userId);

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
    }
}
