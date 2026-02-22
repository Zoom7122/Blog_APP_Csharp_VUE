namespace BlagAPP_MVC.Models
{
    public class DashboardViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public int CountPost { get; set; }
        public int CountCommentsUser { get; set; }

        public UpdateProfileViewModel UpdateProfile { get; set; } = new();
    }
}


