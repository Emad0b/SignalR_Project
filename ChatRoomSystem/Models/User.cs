namespace ChatRoomSystem.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string UserName { get; set; } 
        public required string PasswordHash { get; set; }  
        public required string Email { get; set; }
        public bool Active { get; set; }=true;




    }
}
