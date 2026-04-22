namespace Huskui.Avalonia.Mvvm.States;

public sealed class DefaultViewStateFactory : IViewStateKeyFactory
{
    public string CreateKey(Type owner, string? partitionKey)
    {
        var assemblyName = owner.Assembly.GetName().FullName;
        var typeName = owner.FullName ?? owner.Name;
        return partitionKey != null ? $"{assemblyName}|{typeName}|{partitionKey}" : $"{assemblyName}|{typeName}";
    }
}
