using Application.Models;
using MediatR;

namespace Application.Identity.Commands
{
    public class SignOutCommand : IRequest<GenericResponse>
    {
        public SignOutCommand(string token)
        {
            Token = token;
        }
        
        public string Token { get; }
    }

    public class SignOutCommandHandler : IRequestHandler<SignOutCommand, GenericResponse>
    {
        public Task<GenericResponse> Handle(SignOutCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
