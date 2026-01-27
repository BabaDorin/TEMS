using LocationManagement.Contract.Commands;
using LocationManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace LocationManagement.Api.Endpoints.Rooms;

public class CreateRoomEndpoint(IMediator mediator) : Endpoint<CreateRoomCommand, CreateRoomResponse>
{
    public override void Configure()
    {
        Post("/location/rooms");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(CreateRoomCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}

public class GetAllRoomsEndpoint(IMediator mediator) : EndpointWithoutRequest<GetAllRoomsResponse>
{
    public override void Configure()
    {
        Get("/location/rooms");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var siteId = Query<string>("siteId", false);
        var buildingId = Query<string>("buildingId", false);
        var response = await mediator.Send(new GetAllRoomsCommand(siteId, buildingId), ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}

public class GetRoomByIdEndpoint(IMediator mediator) : EndpointWithoutRequest<GetRoomByIdResponse>
{
    public override void Configure()
    {
        Get("/location/rooms/{Id}");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<string>("Id")!;
        var command = new GetRoomByIdCommand(id);
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}

public class UpdateRoomEndpoint(IMediator mediator) : Endpoint<UpdateRoomCommand, UpdateRoomResponse>
{
    public override void Configure()
    {
        Put("/location/rooms/{Id}");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(UpdateRoomCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}

public class DeleteRoomEndpoint(IMediator mediator) : Endpoint<DeleteRoomCommand, DeleteRoomResponse>
{
    public override void Configure()
    {
        Delete("/location/rooms/{Id}");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(DeleteRoomCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}

public class GetLocationHierarchyEndpoint(IMediator mediator) : EndpointWithoutRequest<GetLocationHierarchyResponse>
{
    public override void Configure()
    {
        Get("/location/hierarchy");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var response = await mediator.Send(new GetLocationHierarchyCommand(), ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
