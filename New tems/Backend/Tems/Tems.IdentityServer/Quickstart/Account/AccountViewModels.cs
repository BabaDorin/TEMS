using System.ComponentModel.DataAnnotations;

namespace Tems.IdentityServer.Quickstart.Account;

public class LoginInputModel
{
    [Required]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
    
    public bool RememberLogin { get; set; }
    
    public string? ReturnUrl { get; set; }
}

public class LoginViewModel : LoginInputModel
{
}

public class RegisterInputModel
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    public string Username { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string Email { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string? FullName { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
    public string Password { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Please confirm your password")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;
    
    public string? ReturnUrl { get; set; }
}

public class RegisterViewModel : RegisterInputModel
{
}

public class LogoutInputModel
{
    public string? LogoutId { get; set; }
}

public class LogoutViewModel : LogoutInputModel
{
    public bool ShowLogoutPrompt { get; set; } = true;
}

public class LoggedOutViewModel
{
    public string? PostLogoutRedirectUri { get; set; }
    public string? ClientName { get; set; }
    public string? SignOutIframeUrl { get; set; }
    public bool AutomaticRedirectAfterSignOut { get; set; }
    public string? LogoutId { get; set; }
    public bool TriggerExternalSignout => ExternalAuthenticationScheme != null;
    public string? ExternalAuthenticationScheme { get; set; }
}
