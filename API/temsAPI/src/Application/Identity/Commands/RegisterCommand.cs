using Application.Models;
using MediatR;

namespace Application.Identity.Commands
{
    public class RegisterCommand : IRequest<GenericResponse>
    {
        public RegisterCommand(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        public string UserName { get; }
        public string Password { get; }
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, GenericResponse>
    {
        public Task<GenericResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
