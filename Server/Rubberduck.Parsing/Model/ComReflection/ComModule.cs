using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using Rubberduck.VBEditor.Utility;
using FUNCDESC = System.Runtime.InteropServices.ComTypes.FUNCDESC;
using TYPEATTR = System.Runtime.InteropServices.ComTypes.TYPEATTR;
using VARDESC = System.Runtime.InteropServices.ComTypes.VARDESC;
using CALLCONV = System.Runtime.InteropServices.ComTypes.CALLCONV;
using Rubberduck.Unmanaged.TypeLibs.Abstract;
using Rubberduck.Unmanaged.TypeLibs.Utility;
using Rubberduck.InternalApi.Model;

namespace Rubberduck.Parsing.Model.ComReflection;

[DataContract]
[KnownType(typeof(ComType))]
public class ComModule : ComType, IComTypeWithMembers, IComTypeWithFields
{
    [DataMember(IsRequired = true)]
    private List<ComMember> _members = new();
    public IEnumerable<ComMember> Members => _members;

    public ComMember DefaultMember => null;

    public bool IsExtensible => false;

    [DataMember(IsRequired = true)]
    private List<ComField> _fields = new();
    public IEnumerable<ComField> Fields => _fields;

    public IEnumerable<ComField> Properties => Enumerable.Empty<ComField>();

    public ComModule(IComBase parent, ITypeLib typeLib, ITypeInfo info, TYPEATTR attrib, int index) : base(parent, typeLib, attrib, index)
    {
        Debug.Assert(attrib.cFuncs >= 0 && attrib.cVars >= 0);
        Type = DeclarationType.ProceduralModule;
        if (attrib.cFuncs > 0)
        {
            GetComMembers(info, attrib);
        }
        if (attrib.cVars > 0)
        {
            GetComFields(info, attrib);
        }
    }

    private void GetComFields(ITypeInfo info, TYPEATTR attrib)
    {
        var names = new string[1];
        for (var index = 0; index < attrib.cVars; index++)
        {
            info.GetVarDesc(index, out IntPtr varPtr);
            using (DisposalActionContainer.Create(varPtr, info.ReleaseVarDesc))
            {
                var desc = Marshal.PtrToStructure<VARDESC>(varPtr);
                info.GetNames(desc.memid, names, names.Length, out int length);
                Debug.Assert(length == 1);

                DeclarationType type;
                if(info is ITypeInfoWrapper wrapped && wrapped.HasVBEExtensions)
                {
                    type = desc.IsValidVBAConstant() ? DeclarationType.Constant : DeclarationType.Variable;
                }
                else
                {
                    type = desc.varkind == VARKIND.VAR_CONST ? DeclarationType.Constant : DeclarationType.Variable;
                }

                _fields.Add(new ComField(this, info, names[0], desc, index, type));
            }
        }
    }

    private void GetComMembers(ITypeInfo info, TYPEATTR attrib)
    {
        for (var index = 0; index < attrib.cFuncs; index++)
        {
            info.GetFuncDesc(index, out IntPtr memberPtr);
            using (DisposalActionContainer.Create(memberPtr, info.ReleaseFuncDesc))
            {
                var member = Marshal.PtrToStructure<FUNCDESC>(memberPtr);
                if (member.callconv != CALLCONV.CC_STDCALL)
                {
                    continue;
                }
                _members.Add(new ComMember(this, info, member));
            }
        }
    }
}
