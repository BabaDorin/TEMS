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
