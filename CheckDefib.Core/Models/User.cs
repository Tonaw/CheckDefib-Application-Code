using System;
namespace CheckDefib.Core.Models
{
    // Add User roles relevant to your application
    public enum Role { Admin, Manager, Staff }

    public enum Role2 {Manager, Staff} // for manager 
    
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        // User role within application
        public Role Role { get; set; }

         public Role Role2 { get; set; } // for manager 

        public IList<User> Users {get; set; } = new List<User>();

    }
}
