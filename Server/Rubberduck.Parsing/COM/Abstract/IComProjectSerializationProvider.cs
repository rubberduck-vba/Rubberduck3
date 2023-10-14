using Rubberduck.Parsing.Model.ComReflection;

namespace Rubberduck.Parsing.COM.Abstract;

public interface IComProjectSerializationProvider : IComProjectDeserializer
{
    string Target { get; }
    void SerializeProject(ComProject project);
}
