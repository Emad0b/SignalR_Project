using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using ChatRoomSystem.Data;
using ChatRoomSystem.Models;

namespace ChatRoomSystem.Controllers
{
    [Route("api/chat")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly MyProjectContext _context;

        public ChatController(MyProjectContext context)
        {
            _context = context;
        }

        // Retrieve all chat messages
        [HttpGet("messages")]
        public async Task<JsonResult> GetMessages()
        {
            var messages = await _context.Messages
                .Include(m => m.Sender)
                .OrderBy(m => m.Timestamp)
                .Select(m => new
                {
                    Id = m.Id,
                    Content = m.Content,
                    Timestamp = m.Timestamp,
                    Sender =  m.Sender.UserName 
                })
                .ToListAsync();

            return new JsonResult(new { success = true, data = messages });
        }

        // Save a new message
        [HttpPost("send")]
        public async Task<JsonResult> SendMessage([FromBody] MessageDto messageDto)
        {
            var sender = await _context.Users.FirstOrDefaultAsync(u => u.Id == messageDto.SenderId);
            if (sender == null)
            {
                return new JsonResult(new { success = false, message = "User not found" });
            }

            var msg = new Message
            {
                Content = messageDto.Content,
                SenderId = sender.Id,
                Timestamp = DateTime.Now
            };

            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true, message = "Message saved successfully" });
        }
    }

    public class MessageDto
    {
        public string Content { get; set; }
        public Guid SenderId { get; set; }
    }
}
