using Application.Identity.Commands;
using Application.Identity.Commands.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.ViewModels.Identity;

namespace Presentation.Controllers
{
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IMediator mediator;

        public IdentityController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogIn(
            [FromBody] LogInViewModel loginViewModel, CancellationToken cancellationToken)
        {
            var command = new LoginCommand(loginViewModel.Username, loginViewModel.Password);
            var result = await mediator.Send(command, cancellationToken);

            return result.HasError
                ? BadRequest(result.ErrorCode)
                : Ok(result.Value);
        }

        [HttpPost("signout")]
        public async Task<IActionResult> SignOut(
            [FromBody] string token, CancellationToken cancellationToken)
        {
            var command = new SignOutCommand(token);
            var result = await mediator.Send(command, cancellationToken);

            return result.HasError 
                ? BadRequest(result.ErrorCode) 
                : Ok();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterViewModel registerViewModel, CancellationToken cancellationToken)
        {
            var command = new RegisterCommand(registerViewModel.Username, registerViewModel.Password);
            var result = await mediator.Send(command, cancellationToken);

            return result.HasError
                ? BadRequest(result.ErrorCode)
                : Ok();
        }
    }
}
