using LocationManagement.Contract.Commands;
using LocationManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace LocationManagement.Api.Endpoints.Buildings;

public class CreateBuildingEndpoint(IMediator mediator) : Endpoint<CreateBuildingCommand, CreateBuildingResponse>
{
    public override void Configure()
    {
        Post("/location/buildings");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(CreateBuildingCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}

public class GetAllBuildingsEndpoint(IMediator mediator) : EndpointWithoutRequest<GetAllBuildingsResponse>
{
    public override void Configure()
    {
        Get("/location/buildings");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var siteId = Query<string>("siteId", false);
        var response = await mediator.Send(new GetAllBuildingsCommand(siteId), ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}

public class GetBuildingByIdEndpoint(IMediator mediator) : Endpoint<GetBuildingByIdCommand, GetBuildingByIdResponse>
{
    public override void Configure()
    {
        Get("/location/buildings/{Id}");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(GetBuildingByIdCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}

public class UpdateBuildingEndpoint(IMediator mediator) : Endpoint<UpdateBuildingCommand, UpdateBuildingResponse>
{
    public override void Configure()
    {
        Put("/location/buildings/{Id}");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(UpdateBuildingCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}

public class DeleteBuildingEndpoint(IMediator mediator) : Endpoint<DeleteBuildingCommand, DeleteBuildingResponse>
{
    public override void Configure()
    {
        Delete("/location/buildings/{Id}");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(DeleteBuildingCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
