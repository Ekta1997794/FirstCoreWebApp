using FirstCoreWebApp.Model;
using System.Data;

namespace FirstCoreWebApp
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public bool IsRefreshTokenRevoked { get; set; } = false;
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
