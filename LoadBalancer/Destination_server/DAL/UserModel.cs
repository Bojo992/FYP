namespace Destination_server.DAL;

public class UserModel
{
    public int Id { get; set; }
    public required string Sender { get; set; }
    public required string Content { get; set; }
    public DateTime SentTime { get; set; }
}