using Rubberduck.Parsing.Model.ComReflection;
using Rubberduck.VBEditor;

namespace Rubberduck.Parsing.COM.Abstract
{
    public interface IComProjectDeserializer
    {
        ComProject DeserializeProject(ReferenceInfo reference);
        bool SerializedVersionExists(ReferenceInfo reference);
    }
}