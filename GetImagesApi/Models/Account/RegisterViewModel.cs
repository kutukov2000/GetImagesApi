namespace GetImagesApi.Models.Account;

public class RegisterViewModel
{
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public IFormFile Image { get; set; }
}
