namespace Destination_server.DAL;

public class UserModel
{
    public required string Sender { get; set; }
    public required string Content { get; set; }
    public DateTime SentTime { get; set; }
}