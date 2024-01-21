namespace Rubberduck.InternalApi.Model.Declarations
{
    public enum ComponentKind
    {
        ComComponent = -1,
        Undefined = 0,

        StandardModule,
        ClassModule,
        DocumentModule,
        UserFormModule,

        ResourceFile,
        VBForm,
        MDIForm,
        PropertyPage,
        UserControl,
        DocObject,
        RelatedDocument,
        ActiveXDesigner,
    }
}