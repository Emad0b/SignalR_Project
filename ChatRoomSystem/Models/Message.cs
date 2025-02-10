namespace ChatRoomSystem.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public  string Content { get; set; }
        public DateTime Timestamp { get; set; }

        public Guid SenderId { get; set; }



        public  virtual User Sender { get; set; }
    }
}
