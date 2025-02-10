using ChatRoomSystem.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    private readonly ChatDbContext _context;

    public ChatHub(ChatDbContext context)
    {
        _context = context;
    }

    public async Task SendMessage(string user, string message)
    {
        var sender = await _context.Users.FirstOrDefaultAsync(u => u.UserName == user);
        if (sender == null) return;

        var msg = new Message
        {
            Id = Guid.NewGuid(),
            Content = message,
            Timestamp = DateTime.UtcNow,
            SenderId = sender.Id
        };

        _context.Messages.Add(msg);
        await _context.SaveChangesAsync();

        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}
