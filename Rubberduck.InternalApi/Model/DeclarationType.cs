using System;

namespace Rubberduck.InternalApi.Model
{
    [Flags]
    public enum DeclarationType : long
    {
        Project = 1L << 0,
        Module = 1L << 1,
        ProceduralModule = 1L << 2 | Module,
        ClassModule = 1L << 3 | Module,
        UserForm = 1L << 4 | ClassModule,
        Document = 1L << 5 | ClassModule,
        VbForm = 1L << 6 | ClassModule,
        Member = 1L << 7,
        Procedure = 1L << 8 | Member,
        Function = 1L << 9 | Member,
        Property = 1L << 10 | Member,
        PropertyGet = 1L << 11 | Property | Function,
        PropertyLet = 1L << 12 | Property | Procedure,
        PropertySet = 1L << 13 | Property | Procedure,
        Parameter = 1L << 14,
        Variable = 1L << 15,
        Control = 1L << 16 | Variable,
        Constant = 1L << 17,
        Enumeration = 1L << 18,
        EnumerationMember = 1L << 19,
        Event = 1L << 20,
        UserDefinedType = 1L << 21,
        UserDefinedTypeMember = 1L << 22,
        LibraryFunction = 1L << 23 | Function,
        LibraryProcedure = 1L << 24 | Procedure,
        LineLabel = 1L << 25,
        UnresolvedMember = 1L << 26,
        BracketedExpression = 1L << 27,
        ComAlias = 1L << 28,
        MdiForm = 1L << 29 | VbForm,
        ResFile = 1L << 30,
        PropPage = 1L << 31 | ClassModule,
        UserControl = 1L << 32 | ClassModule,
        DocObject = 1L << 33 | ClassModule,
        RelatedDocument = 1L << 34,
        ActiveXDesigner = 1L << 35 | ClassModule
    }
}
