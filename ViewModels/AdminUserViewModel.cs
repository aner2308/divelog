using Microsoft.AspNetCore.Identity;

namespace divelog.ViewModels
{
    public class AdminUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}