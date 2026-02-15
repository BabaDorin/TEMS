using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Security.Claims;
using Tems.Common.Identity;
using Tems.IdentityServer.Quickstart.Account;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Tems.IdentityServer.Tests")]

namespace Tems.IdentityServer.Quickstart.Account;

[SecurityHeaders]
[AllowAnonymous]
public class AccountController : Controller
{
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IClientStore _clientStore;
    private readonly IAuthenticationSchemeProvider _schemeProvider;
    private readonly IEventService _events;
    private readonly IMongoCollection<User> _users;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        IIdentityServerInteractionService interaction,
        IClientStore clientStore,
        IAuthenticationSchemeProvider schemeProvider,
        IEventService events,
        IMongoDatabase database,
        IPasswordHasher<User> passwordHasher,
        ILogger<AccountController> logger)
    {
        _interaction = interaction;
        _clientStore = clientStore;
        _schemeProvider = schemeProvider;
        _events = events;
        _users = database.GetCollection<User>("users");
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Login(string? returnUrl)
    {
        var vm = await BuildLoginViewModelAsync(returnUrl);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginInputModel model, string? button)
    {
        var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

        if (button != "login")
        {
            if (context != null)
            {
                await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);
                return Redirect(model.ReturnUrl ?? "~/");
            }
            return Redirect("~/");
        }

        if (ModelState.IsValid)
        {
            var user = await _users.Find(u => u.Username == model.Username).FirstOrDefaultAsync();

            if (user != null && user.IsActive)
            {
                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

                if (result == PasswordVerificationResult.Success || 
                    result == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    await _events.RaiseAsync(new UserLoginSuccessEvent(user.Username, user.Id, user.Username, clientId: context?.Client.ClientId));

                    var isuser = new IdentityServerUser(user.Id)
                    {
                        DisplayName = user.Username
                    };

                    await HttpContext.SignInAsync(isuser);

                    _logger.LogInformation("User {Username} logged in successfully. ReturnUrl: {ReturnUrl}, HasContext: {HasContext}", 
                        user.Username, model.ReturnUrl, context != null);

                    if (context != null)
                    {
                        if (string.IsNullOrEmpty(model.ReturnUrl))
                        {
                            throw new Exception("Return URL is required for external authentication");
                        }
                        _logger.LogInformation("Redirecting to: {ReturnUrl}", model.ReturnUrl);
                        return Redirect(model.ReturnUrl);
                    }

                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else if (string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        return Redirect("~/");
                    }
                    else
                    {
                        throw new Exception("invalid return URL");
                    }
                }
            }

            await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials", clientId: context?.Client.ClientId));
            _logger.LogWarning("Login failed for user: {Username}. ReturnUrl: {ReturnUrl}", model.Username, model.ReturnUrl);
            ModelState.AddModelError(string.Empty, "Invalid username or password");
        }

        var vm = await BuildLoginViewModelAsync(model);
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Logout(string? logoutId)
    {
        var vm = await BuildLogoutViewModelAsync(logoutId);

        if (vm.ShowLogoutPrompt == false)
        {
            return await Logout(vm);
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(LogoutInputModel model)
    {
        var vm = await BuildLoggedOutViewModelAsync(model.LogoutId);

        if (User?.Identity?.IsAuthenticated == true)
        {
            await HttpContext.SignOutAsync();
            await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
        }

        if (vm.TriggerExternalSignout)
        {
            string url = Url.Action("Logout", new { logoutId = vm.LogoutId }) ?? "";
            return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme ?? "");
        }

        return View("LoggedOut", vm);
    }

    [HttpGet]
    public IActionResult Register(string? returnUrl)
    {
        var vm = new RegisterViewModel { ReturnUrl = returnUrl };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterInputModel model)
    {
        if (ModelState.IsValid)
        {
            // Check if username already exists
            var existingUsername = await _users.Find(u => u.Username == model.Username).FirstOrDefaultAsync();
            if (existingUsername != null)
            {
                ModelState.AddModelError("Username", "Username is already taken");
                return View(new RegisterViewModel 
                { 
                    Username = model.Username, 
                    Email = model.Email, 
                    FullName = model.FullName,
                    ReturnUrl = model.ReturnUrl 
                });
            }

            // Check if email already exists
            var existingEmail = await _users.Find(u => u.Email == model.Email).FirstOrDefaultAsync();
            if (existingEmail != null)
            {
                ModelState.AddModelError("Email", "Email is already registered");
                return View(new RegisterViewModel 
                { 
                    Username = model.Username, 
                    Email = model.Email, 
                    FullName = model.FullName,
                    ReturnUrl = model.ReturnUrl 
                });
            }

            // Create the user
            var user = new User
            {
                Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                Username = model.Username,
                Email = model.Email,
                FullName = model.FullName ?? model.Username,
                IsActive = true,
                Roles = new List<string> { "User" },
                Claims = new Dictionary<string, string>
                {
                    { "can_view_entities", "true" }
                },
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

            try
            {
                await _users.InsertOneAsync(user);
                _logger.LogInformation("User {Username} registered successfully", model.Username);

                // Redirect to login page with success message
                TempData["SuccessMessage"] = "Registration successful! Please log in with your new account.";
                return RedirectToAction("Login", new { returnUrl = model.ReturnUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user {Username}", model.Username);
                ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again.");
            }
        }

        return View(new RegisterViewModel 
        { 
            Username = model.Username, 
            Email = model.Email, 
            FullName = model.FullName,
            ReturnUrl = model.ReturnUrl 
        });
    }

    private async Task<LoginViewModel> BuildLoginViewModelAsync(string? returnUrl)
    {
        var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
        
        return new LoginViewModel
        {
            ReturnUrl = returnUrl,
            Username = context?.LoginHint ?? ""
        };
    }

    private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
    {
        var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
        vm.Username = model.Username;
        vm.RememberLogin = model.RememberLogin;
        return vm;
    }

    private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string? logoutId)
    {
        var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = true };

        if (User?.Identity?.IsAuthenticated != true)
        {
            vm.ShowLogoutPrompt = false;
            return vm;
        }

        var context = await _interaction.GetLogoutContextAsync(logoutId);
        if (context?.ShowSignoutPrompt == false)
        {
            vm.ShowLogoutPrompt = false;
            return vm;
        }

        return vm;
    }

    private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string? logoutId)
    {
        var logout = await _interaction.GetLogoutContextAsync(logoutId);

        var vm = new LoggedOutViewModel
        {
            AutomaticRedirectAfterSignOut = true,
            PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
            ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
            SignOutIframeUrl = logout?.SignOutIFrameUrl,
            LogoutId = logoutId
        };

        if (User?.Identity?.IsAuthenticated == true)
        {
            var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
            if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
            {
                var schemes = HttpContext.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
                var scheme = await schemes.GetSchemeAsync(idp);
                var handler = scheme?.HandlerType != null ? 
                    HttpContext.RequestServices.GetService(scheme.HandlerType) as IAuthenticationSignOutHandler : 
                    null;
                
                if (handler != null)
                {
                    if (vm.LogoutId == null)
                    {
                        vm.LogoutId = await _interaction.CreateLogoutContextAsync();
                    }

                    vm.ExternalAuthenticationScheme = idp;
                }
            }
        }

        return vm;
    }
}
