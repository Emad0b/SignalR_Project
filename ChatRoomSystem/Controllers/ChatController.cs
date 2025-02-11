using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using ChatRoomSystem.Models;

namespace ChatRoomSystem.Controllers
{
    //    [Route("api/chat")]
    //    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ChatDbContext _context;

        public ChatController(ChatDbContext context)
        {
            _context = context;
        }

        // Retrieve all chat messages
        [HttpGet]
        public JsonResult GetMessages()
        {
            var messages = _context.Messages
                .Include(m => m.Sender)
                .OrderBy(m => m.Timestamp)
                .Select(m => new
                {
                    Id = m.Id,
                    Content = m.Content,
                    Timestamp = m.Timestamp,
                    Sender = m.Sender.UserName
                })
                .ToList();

            return new JsonResult(new { success = true, data = messages });
        }

        // Save a new message
        [HttpPost]
        public JsonResult SendMessage([FromBody] MessageDto obj)
        {
            var sender =  _context.Users.FirstOrDefault(u => u.Id == obj.SenderId);
            if (sender == null)
            {
                return new JsonResult(new { success = false, message = "User not found" });
            }

            var msg = new Message
            {
                Content = obj.Content,
                SenderId = sender.Id,
                Timestamp = DateTime.Now
            };

            _context.Messages.Add(msg);
             _context.SaveChangesAsync();

            return new JsonResult(new { success = true, message = "Message saved successfully" });
        }
    }

    public class MessageDto
    {
        public string Content { get; set; }
        public Guid SenderId { get; set; }
    }
}