CREATE TABLE [Declarations] (
  [Id] INTEGER NOT NULL IDENTITY(1,1)
, [IsMarkedForUpdate] INTEGER NOT NULL DEFAULT(0)
, [IsMarkedForDeletion] INTEGER NOT NULL DEFAULT(0)
, [DeclarationType] INTEGER NOT NULL
, [IsArray] INTEGER NOT NULL
, [AsTypeName] TEXT NULL
, [AsTypeDeclarationId] INTEGER NULL
, [TypeHint] TEXT NULL
, [IdentifierName] TEXT NOT NULL
, [IsUserDefined] INTEGER NOT NULL
, [ParentDeclarationId] INTEGER NULL
, [DocString] TEXT NULL
, [Annotations] TEXT NULL
, [Attributes] TEXT NULL
-- lines & offsets NULL for projects, modules, and !IsUserDefined
, [DocumentLineStart] INTEGER NULL
, [DocumentLineEnd] INTEGER NULL
, [ContextStartOffset] INTEGER NULL
, [ContextEndOffset] INTEGER NULL
, [IdentifierStartOffset] INTEGER NULL
, [IdentifierEndOffset] INTEGER NULL
, CONSTRAINT [PK_Declarations] PRIMARY KEY ([Id])
, CONSTRAINT [FK_Declaration_Parent] FOREIGN KEY ([ParentDeclarationId]) REFERENCES [Declarations] ([Id])
);

CREATE TABLE [Projects] (
  [Id] INTEGER NOT NULL IDENTITY(1,1)
, [DeclarationId] INTEGER NOT NULL
, [VBProjectId] TEXT NULL
, [Guid] TEXT NULL
, [MajorVersion] INTEGER NULL
, [MinorVersion] INTEGER NULL
, [Path] TEXT NULL
, CONSTRAINT [PK_Projects] PRIMARY KEY ([Id])
, CONSTRAINT [FK_Project_Declaration] FOREIGN KEY ([DeclarationId]) REFERENCES [Declarations] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Modules] (
  [Id] INTEGER NOT NULL IDENTITY(1,1)
, [DeclarationId] INTEGER NOT NULL
, [Folder] TEXT NULL -- from @Folder annotations
, CONSTRAINT [PK_Modules] PRIMARY KEY ([Id])
, CONSTRAINT [FK_Module_Declaration] FOREIGN KEY ([DeclarationId]) REFERENCES [Declarations] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [ModuleInterfaces] (
  [IsDeleted] INTEGER NOT NULL DEFAULT(0)
, [ModuleId] INTEGER NOT NULL
, [ImplementsModuleId] INTEGER NOT NULL
, CONSTRAINT [PK_ModuleInterfaces] PRIMARY KEY ([ModuleId], [ImplementsModuleId])
, CONSTRAINT [FK_ModuleInterface_Modules] FOREIGN KEY ([ModuleId]) REFERENCES [Modules] ([Id])
, CONSTRAINT [FK_ModuleInterface_ImplementsModules] FOREIGN KEY ([ImplementsModuleId]) REFERENCES [Modules] ([Id])
);

CREATE TABLE [Members] (
  [Id] INTEGER NOT NULL IDENTITY(1,1)
, [DeclarationId] INTEGER NOT NULL
, [ImplementsDeclarationId] INTEGER NULL
, [Accessibility] INTEGER NOT NULL
, [IsAutoAssigned] INTEGER NOT NULL
, [IsWithEvents] INTEGER NOT NULL
, [IsDimStmt] INTEGER NOT NULL
, [ValueExpression] TEXT NULL
, CONSTRAINT [PK_Members] PRIMARY KEY ([Id])
, CONSTRAINT [FK_Module_Declaration] FOREIGN KEY ([DeclarationId]) REFERENCES [Declarations] ([Id]) ON DELETE CASCADE
, CONSTRAINT [FK_Member_ImplementsDeclaration] FOREIGN KEY ([ImplementsMemberId]) REFERENCES [Declarations] ([Id])
);

CREATE TABLE [Parameters] (
  [Id] INTEGER NOT NULL IDENTITY(1,1)
, [DeclarationId] INTEGER NOT NULL
, [Position] INTEGER NOT NULL
, [IsParamArray] INTEGER NOT NULL
, [IsOptional] INTEGER NOT NULL
, [IsByRef] INTEGER NOT NULL
, [IsByVal] INTEGER NOT NULL
, [IsModifierImplicit] INTEGER NOT NULL
, [DefaultValue] TEXT NULL
, CONSTRAINT [PK_Parameters] PRIMARY KEY ([Id])
, CONSTRAINT [FK_Parameter_Declaration] FOREIGN KEY ([DeclarationId]) REFERENCES [Declarations] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Locals] (
  [Id] INTEGER NOT NULL IDENTITY(1,1)
, [DeclarationId] INTEGER NOT NULL
, [IsAutoAssigned] INTEGER NOT NULL
, [DeclaredValue] TEXT NULL
, CONSTRAINT [PK_Locals] PRIMARY KEY ([Id])
, CONSTRAINT [FK_Local_Declaration] FOREIGN KEY ([DeclarationId]) REFERENCES [Declarations] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [IdentifierReferences] (
  [Id] INTEGER NOT NULL IDENTITY(1,1)
, [IsMarkedForUpdate] INTEGER NOT NULL DEFAULT(0)
, [IsMarkedForDeletion] INTEGER NOT NULL DEFAULT(0)
, [ReferencedDeclarationId] INTEGER NOT NULL
, [ParentDeclarationId] INTEGER NOT NULL
, [QualifyingReferenceId] INTEGER NULL
, [IsAssignmentTarget] INTEGER NOT NULL
, [IsExplicitCallStatement] INTEGER NOT NULL
, [IsExplicitLetAssignment] INTEGER NOT NULL
, [IsSetAssignment] INTEGER NOT NULL
, [IsReDim] INTEGER NOT NULL
, [IsArrayAccess] INTEGER NOT NULL
, [IsProcedureCoercion] INTEGER NOT NULL
, [IsIndexedDefaultMemberAccess] INTEGER NOT NULL
, [IsNonIndexedDefaultMemberAccess] INTEGER NOT NULL
, [IsInnerRecursiveDefaultMemberAccess] INTEGER NOT NULL
, [DefaultMemberRecursionDepth] INTEGER NULL
, [TypeHint] TEXT NULL
, [Annotations] TEXT NULL
, [DocumentLineStart] INTEGER NOT NULL
, [DocumentLineEnd] INTEGER NOT NULL
, [ContextStartOffset] INTEGER NOT NULL
, [ContextEndOffset] INTEGER NOT NULL
, [IdentifierStartOffset] INTEGER NOT NULL
, [IdentifierEndOffset] INTEGER NOT NULL
, CONSTRAINT [PK_IdentifierReferences] PRIMARY KEY ([Id])
, CONSTRAINT [FK_IdentifierReference_Qualifier] FOREIGN KEY ([QualifyingReferenceId]) REFERENCES [IdentifierReferences] ([Id])
, CONSTRAINT [FK_Module_Declarations] FOREIGN KEY ([ReferencedDeclarationId]) REFERENCES [Declarations] ([Id])
, CONSTRAINT [FK_Module_IdentifierReferences] FOREIGN KEY ([ModuleId]) REFERENCES [Modules] ([Id])
, CONSTRAINT [FK_Member_IdentifierReferences] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([Id])
);

CREATE TABLE [DeclarationAnnotations] (
  [Id] INTEGER NOT NULL IDENTITY(1,1)
, [DeclarationId] INTEGER NOT NULL
, [AnnotationName] TEXT NOT NULL
, [AnnotationArgs] TEXT NULL
, [DocumentLineStart] INTEGER NOT NULL
, [DocumentLineEnd] INTEGER NOT NULL
, [ContextStartOffset] INTEGER NOT NULL
, [ContextEndOffset] INTEGER NOT NULL
, [IdentifierStartOffset] INTEGER NOT NULL
, [IdentifierEndOffset] INTEGER NOT NULL
, CONSTRAINT [PK_DeclarationAnnotations] PRIMARY KEY ([Id])
, CONSTRAINT [FK_DeclarationAnnotations_Declaration] FOREIGN KEY ([DeclarationId]) REFERENCES [Declarations] ([Id])
);

CREATE TABLE [IdentifierReferenceAnnotations] (
  [Id] INTEGER NOT NULL IDENTITY(1,1)
, [IdentifierReferenceId] INTEGER NOT NULL
, [AnnotationName] TEXT NOT NULL
, [AnnotationArgs] TEXT NULL
, [DocumentLineStart] INTEGER NOT NULL
, [DocumentLineEnd] INTEGER NOT NULL
, [ContextStartOffset] INTEGER NOT NULL
, [ContextEndOffset] INTEGER NOT NULL
, [IdentifierStartOffset] INTEGER NOT NULL
, [IdentifierEndOffset] INTEGER NOT NULL
, CONSTRAINT [PK_IdentifierReferenceAnnotations] PRIMARY KEY ([Id])
, CONSTRAINT [FK_IdentifierReferenceAnnotations_IdentifierReference] FOREIGN KEY ([IdentifierReferenceId]) REFERENCES [IdentifierReferences] ([Id])
);

CREATE TABLE [ProjectSettings] (
  [Id] INTEGER NOT NULL
 ,[ProjectId] INTEGER NOT NULL
 ,[DataType] INTEGER NOT NULL
 ,[Key] TEXT NOT NULL
 ,[Value] TEXT NOT NULL
 ,CONSTRAINT [PK_ProjectSettings] PRIMARY KEY ([Id])
 ,CONSTRAINT [NK_ProjectSettings] UNIQUE ([ProjectId],[Key])
 ,CONSTRAINT [FK_Project_ProjectSettings] FOREIGN KEY ([ProjectId]) REFERENCES [Projects] ([Id])
);


GO
CREATE VIEW [AsTypeNamesByProjectId]
AS
	SELECT
		 [ProjectId] = p.[Id]
		,[ProjectDeclarationId] = pd.[Id]
		
	FROM [Modules] m
	INNER JOIN [Declarations] md ON m.[DeclarationId] = md.[Id]
	INNER JOIN [Declarations] pd ON m.[ParentDeclarationId] = pd.[Id]
	INNER JOIN [Projects] p ON pd.[Id] = p.[DeclarationId]

;