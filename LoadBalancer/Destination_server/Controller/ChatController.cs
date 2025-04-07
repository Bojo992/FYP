using Microsoft.AspNetCore.SignalR;
using Destination_server.DAL;

namespace Destination_server.Controller;

public class ChatController : Hub
{
    private static List<UserModel> MessageHistory = new List<UserModel>();
    public async Task PostMessage(string content)
    {
        var senderId = Context.ConnectionId;
        var userMessage = new UserModel
        {
            Sender = senderId,
            Content = content,
            SentTime = DateTime.UtcNow
        };
        MessageHistory.Add(userMessage);
        await Clients.Others.SendAsync("ReceiveMessage", senderId, content, userMessage.SentTime);
    }
    public async Task RetrieveMessageHistory() => 
        await Clients.Caller.SendAsync("MessageHistory", MessageHistory);
}