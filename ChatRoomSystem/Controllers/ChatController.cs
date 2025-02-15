using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using ChatRoomSystem.Models;

namespace ChatRoomSystem.Controllers
{
    public class ChatController : ControllerBase
    {
        private readonly ChatDbContext _context;

        public ChatController(ChatDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public JsonResult GetMessages()
        {
            var messages = _context.Messages
                .Include(m => m.Sender)
                .OrderByDescending(m => m.Timestamp)
                .Select(m => new
                {
                    id = m.Id,  
                    content = m.Content, 
                    timestamp = m.Timestamp,
                    senderId = m.Sender.Id,
                    senderName = m.Sender.UserName
                })
                .ToList();

            return new JsonResult(new { success = true, data = messages });
        }



        [HttpPost]
        public JsonResult SendMessage(Guid senderId, string content)
        {
            var sender = _context.Users.FirstOrDefault(u => u.Id == senderId);
            if (sender == null)
            {
                return new JsonResult(new { success = false, message = "User not found" });
            }

            var msg = new Message
            {
                Content = content,
                SenderId = sender.Id,
                Timestamp = DateTime.Now
            };

            _context.Messages.Add(msg);
            _context.SaveChanges();

            return new JsonResult(new { success = true, message = "Message saved successfully" });
        }

    }

}