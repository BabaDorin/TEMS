using Application.Models;
using MediatR;

namespace Application.Identity.Commands.Login
{
    public class LoginCommand : IRequest<GenericResponse<string>>
    {
        public LoginCommand(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; }
        public string Password { get; }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, GenericResponse<string>>
    {
        public Task<GenericResponse<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(GenericResponse<string>.Failure("test"));
        }
    }
}
