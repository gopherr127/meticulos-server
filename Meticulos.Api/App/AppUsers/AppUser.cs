
using System;

namespace Meticulos.Api.App.AppUsers
{
    public class AppUser : Entity
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public DateTime Last_Login { get; set; }
        //public string Password { get; set; }
        //public string LastWorkspace { get; set; }
    }
}
