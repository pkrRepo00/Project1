namespace FileConverterSuite.Domain.Interfaces;

public interface IConversionRegistry
{
    IReadOnlyCollection<IConversionPlugin> Plugins { get; }
    void Register(IConversionPlugin plugin);
    IConversionPlugin Resolve(string sourceExtension, string targetExtension);
}
