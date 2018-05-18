
namespace Meticulos.Api.App.AppUsers
{
    public class AppUser : Entity
    {
        public string EmailAddress { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string LastWorkspace { get; set; }
    }
}
