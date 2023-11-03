using Rubberduck.Parsing.Model.ComReflection;
using Rubberduck.Unmanaged.Model;

namespace Rubberduck.Parsing.COM.Abstract;

public interface IComProjectDeserializer
{
    ComProject DeserializeProject(ReferenceInfo reference);
    bool SerializedVersionExists(ReferenceInfo reference);
}