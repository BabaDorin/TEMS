using AssetManagement.Contract.DTOs;
using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Contract.Commands;

public record CreateAssetPropertyCommand(
    string Name,
    string Description,
    string DataType,
    string Category,
    PropertyValidationDto? DefaultValidation,
    List<string> EnumValues,
    string? Unit,
    string CreatedBy) 
    : IRequest<CreateAssetPropertyResponse>;