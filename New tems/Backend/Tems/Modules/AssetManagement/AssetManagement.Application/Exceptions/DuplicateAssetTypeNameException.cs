using System;

namespace AssetManagement.Application.Exceptions;

public class DuplicateAssetTypeNameException(string name) : Exception($"An asset type with name '{name}' already exists.")
{
    public string Name { get; } = name;
}
