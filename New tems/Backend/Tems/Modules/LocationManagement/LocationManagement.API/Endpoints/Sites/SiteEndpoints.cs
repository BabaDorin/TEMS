using LocationManagement.Contract.Commands;
using LocationManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace LocationManagement.Api.Endpoints.Sites;

public class CreateSiteEndpoint(IMediator mediator) : Endpoint<CreateSiteCommand, CreateSiteResponse>
{
    public override void Configure()
    {
        Post("/location/sites");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(CreateSiteCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}

public class GetAllSitesEndpoint(IMediator mediator) : EndpointWithoutRequest<GetAllSitesResponse>
{
    public override void Configure()
    {
        Get("/location/sites");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var response = await mediator.Send(new GetAllSitesCommand(), ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}

public class GetSiteByIdEndpoint(IMediator mediator) : Endpoint<GetSiteByIdCommand, GetSiteByIdResponse>
{
    public override void Configure()
    {
        Get("/location/sites/{Id}");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(GetSiteByIdCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}

public class UpdateSiteEndpoint(IMediator mediator) : Endpoint<UpdateSiteCommand, UpdateSiteResponse>
{
    public override void Configure()
    {
        Put("/location/sites/{Id}");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(UpdateSiteCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}

public class DeleteSiteEndpoint(IMediator mediator) : Endpoint<DeleteSiteCommand, DeleteSiteResponse>
{
    public override void Configure()
    {
        Delete("/location/sites/{Id}");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(DeleteSiteCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
