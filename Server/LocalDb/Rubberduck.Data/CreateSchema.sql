DROP TABLE IF EXISTS [Declarations];
GO
CREATE TABLE [Declarations] (
  [Id] INTEGER PRIMARY KEY NOT NULL
, [DeclarationType] INTEGER NOT NULL
, [IdentifierName] TEXT NOT NULL
, [DocString] TEXT NULL -- from @Description annotations; COM docstring when !IsUserDefined
, [IsUserDefined] INTEGER NOT NULL
, [AsTypeName] TEXT NULL
, [AsTypeDeclarationId] INTEGER NULL
, [IsArray] INTEGER NOT NULL
, [TypeHint] TEXT NULL
, [ParentDeclarationId] INTEGER NULL
-- lines & offsets NULL for projects, modules, and !IsUserDefined
, [ContextStartOffset] INTEGER NULL
, [ContextEndOffset] INTEGER NULL
, [IdentifierStartOffset] INTEGER NULL
, [IdentifierEndOffset] INTEGER NULL
, CONSTRAINT [FK_Declaration_Parent] FOREIGN KEY ([ParentDeclarationId]) REFERENCES [Declarations] ([Id])
, CONSTRAINT [FK_Declaration_AsType] FOREIGN KEY ([AsTypeDeclarationId]) REFERENCES [Declarations] ([Id])
);
GO

DROP TABLE IF EXISTS [Projects];
GO
CREATE TABLE [Projects] (
  [Id] INTEGER PRIMARY KEY NOT NULL
, [DeclarationId] INTEGER NOT NULL
, [VBProjectId] TEXT NULL
, [Guid] TEXT NULL
, [MajorVersion] INTEGER NULL
, [MinorVersion] INTEGER NULL
, [Path] TEXT NULL
, CONSTRAINT [FK_Project_Declaration] FOREIGN KEY ([DeclarationId]) REFERENCES [Declarations] ([Id]) ON DELETE CASCADE
);
GO

DROP TABLE IF EXISTS [Modules];
GO
CREATE TABLE [Modules] (
  [Id] INTEGER PRIMARY KEY NOT NULL
, [DeclarationId] INTEGER NOT NULL
, [Folder] TEXT NULL -- from @Folder annotations
, CONSTRAINT [FK_Module_Declaration] FOREIGN KEY ([DeclarationId]) REFERENCES [Declarations] ([Id]) ON DELETE CASCADE
);
GO

DROP TABLE IF EXISTS [DeclarationInterfaces];
GO
CREATE TABLE [DeclarationInterfaces] (
  [DeclarationId] INTEGER NOT NULL
, [ImplementsDeclarationId] INTEGER NOT NULL
, CONSTRAINT [FK_DeclarationInterface_Declarations] FOREIGN KEY ([DeclarationId]) REFERENCES [Declarations] ([Id])
, CONSTRAINT [FK_DeclarationInterface_ImplementsDeclaration] FOREIGN KEY ([ImplementsDeclarationId]) REFERENCES [Declarations] ([Id])
);
GO

DROP TABLE IF EXISTS [Members];
GO
CREATE TABLE [Members] (
  [Id] INTEGER PRIMARY KEY NOT NULL
, [DeclarationId] INTEGER NOT NULL
, [ImplementsDeclarationId] INTEGER NULL
, [Accessibility] INTEGER NOT NULL
, [IsAutoAssigned] INTEGER NOT NULL
, [IsWithEvents] INTEGER NOT NULL
, [IsDimStmt] INTEGER NOT NULL
, [ValueExpression] TEXT NULL
, CONSTRAINT [FK_Module_Declaration] FOREIGN KEY ([DeclarationId]) REFERENCES [Declarations] ([Id]) ON DELETE CASCADE
, CONSTRAINT [FK_Member_ImplementsDeclaration] FOREIGN KEY ([ImplementsDeclarationId]) REFERENCES [Declarations] ([Id])
);
GO

DROP TABLE IF EXISTS [Parameters];
GO
CREATE TABLE [Parameters] (
  [Id] INTEGER PRIMARY KEY NOT NULL
, [DeclarationId] INTEGER NOT NULL
, [Position] INTEGER NOT NULL
, [IsParamArray] INTEGER NOT NULL
, [IsOptional] INTEGER NOT NULL
, [IsByRef] INTEGER NOT NULL
, [IsByVal] INTEGER NOT NULL
, [IsModifierImplicit] INTEGER NOT NULL
, [DefaultValue] TEXT NULL
, CONSTRAINT [FK_Parameter_Declaration] FOREIGN KEY ([DeclarationId]) REFERENCES [Declarations] ([Id]) ON DELETE CASCADE
);
GO

DROP TABLE IF EXISTS [Locals];
GO
CREATE TABLE [Locals] (
  [Id] INTEGER PRIMARY KEY NOT NULL
, [DeclarationId] INTEGER NOT NULL
, [IsAutoAssigned] INTEGER NOT NULL
, [DeclaredValue] TEXT NULL
, CONSTRAINT [FK_Local_Declaration] FOREIGN KEY ([DeclarationId]) REFERENCES [Declarations] ([Id]) ON DELETE CASCADE
);
GO

DROP TABLE IF EXISTS [IdentifierReferences];
GO
CREATE TABLE [IdentifierReferences] (
  [Id] INTEGER PRIMARY KEY NOT NULL
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
, [ContextStartOffset] INTEGER NOT NULL
, [ContextEndOffset] INTEGER NOT NULL
, [IdentifierStartOffset] INTEGER NOT NULL
, [IdentifierEndOffset] INTEGER NOT NULL
, CONSTRAINT [FK_IdentifierReference_Qualifier] FOREIGN KEY ([QualifyingReferenceId]) REFERENCES [IdentifierReferences] ([Id])
, CONSTRAINT [FK_Module_Declarations] FOREIGN KEY ([ReferencedDeclarationId]) REFERENCES [Declarations] ([Id])
, CONSTRAINT [FK_Module_Parent] FOREIGN KEY ([ParentDeclarationId]) REFERENCES [Declarations] ([Id])
);
GO

DROP TABLE IF EXISTS [DeclarationAnnotations];
GO
CREATE TABLE [DeclarationAnnotations] (
  [Id] INTEGER PRIMARY KEY NOT NULL
, [DeclarationId] INTEGER NOT NULL
, [AnnotationName] TEXT NOT NULL
, [AnnotationArgs] TEXT NULL
, [ContextStartOffset] INTEGER NOT NULL
, [ContextEndOffset] INTEGER NOT NULL
, [IdentifierStartOffset] INTEGER NOT NULL
, [IdentifierEndOffset] INTEGER NOT NULL
, CONSTRAINT [FK_DeclarationAnnotations_Declaration] FOREIGN KEY ([DeclarationId]) REFERENCES [Declarations] ([Id])
);
GO

DROP TABLE IF EXISTS [DeclarationAttributes]
GO
CREATE TABLE [DeclarationAttributes] (
  [Id] INTEGER PRIMARY KEY NOT NULL
, [DeclarationId] INTEGER NOT NULL
, [AttributeName] TEXT NOT NULL
, [AttributeValues] TEXT NULL
, [ContextStartOffset] INTEGER NOT NULL
, [ContextEndOffset] INTEGER NOT NULL
, [IdentifierStartOffset] INTEGER NOT NULL
, [IdentifierEndOffset] INTEGER NOT NULL
, CONSTRAINT [FK_DeclarationAttributes_Declaration] FOREIGN KEY ([DeclarationId]) REFERENCES [Declarations] ([Id])
);
GO

DROP TABLE IF EXISTS [IdentifierReferenceAnnotations]
GO
CREATE TABLE [IdentifierReferenceAnnotations] (
  [Id] INTEGER PRIMARY KEY NOT NULL
, [IdentifierReferenceId] INTEGER NOT NULL
, [AnnotationName] TEXT NOT NULL
, [AnnotationArgs] TEXT NULL
, [ContextStartOffset] INTEGER NOT NULL
, [ContextEndOffset] INTEGER NOT NULL
, [IdentifierStartOffset] INTEGER NOT NULL
, [IdentifierEndOffset] INTEGER NOT NULL
, CONSTRAINT [FK_IdentifierReferenceAnnotations_IdentifierReference] FOREIGN KEY ([IdentifierReferenceId]) REFERENCES [IdentifierReferences] ([Id])
);

DROP TABLE IF EXISTS [ProjectSettings]
GO
CREATE TABLE [ProjectSettings] (
  [Id] INTEGER PRIMARY KEY NOT NULL
 ,[ProjectId] INTEGER NOT NULL
 ,[DataType] INTEGER NOT NULL
 ,[Key] TEXT NOT NULL
 ,[Value] TEXT NOT NULL
 ,CONSTRAINT [NK_ProjectSettings] UNIQUE ([ProjectId],[Key])
 ,CONSTRAINT [FK_Project_ProjectSettings] FOREIGN KEY ([ProjectId]) REFERENCES [Projects] ([Id]) ON DELETE CASCADE
);
GO

DROP VIEW IF EXISTS [Projects_v1];
GO
CREATE VIEW [Projects_v1]
AS
	SELECT
	  [Id] = src.[Id]
	 ,[VBProjectId] = src.[VBProjectId]
	 ,[Guid] = src.[Guid]
	 ,[MajorVersion] = src.[MajorVersion]
	 ,[MinorVersion] = src.[MinorVersion]
	 ,[Path] = src.[Path]

	 ,[DeclarationId] = src.[DeclarationId]
	 ,[DeclarationType] = d.[DeclarationType]
	 ,[IdentifierName] = d.[IdentifierName]
	 ,[IsUserDefined] = d.[IsUserDefined]

	FROM [Projects] src
    INNER JOIN [Declarations] d		ON src.[DeclarationId] = d.[Id];
GO

DROP VIEW IF EXISTS [Modules_v1];
GO
CREATE VIEW [Modules_v1]
AS
	SELECT
	  [Id] = src.[Id]
	 ,[Folder] = src.[Folder]

	 ,[DeclarationId] = src.[DeclarationId]
	 ,[DeclarationType] = d.[DeclarationType]
	 ,[IdentifierName] = d.[IdentifierName]
	 ,[DocString] = d.[DocString]
	 ,[IsUserDefined] = d.[IsUserDefined]

	 ,[ProjectDeclarationId] = p.[DeclarationId]
	 ,[ProjectName] = pd.[IdentifierName]
	 ,[VBProjectId] = p.[VBProjectId]

	FROM [Modules] src
    INNER JOIN [Declarations] d		ON d.[Id] = src.[DeclarationId]
	INNER JOIN [Projects] p			ON p.[DeclarationId] = d.[ParentDeclarationId]
	INNER JOIN [Declarations] pd	ON pd.[Id] = p.[DeclarationId];
GO

DROP VIEW IF EXISTS [Members_v1];
GO
CREATE VIEW [Members_v1]
AS
	SELECT
	  [Id] = src.[Id]
	 ,[ImplementsDeclarationId] = src.[ImplementsDeclarationId]
	 ,[Accessibility] = src.[Accessibility]
	 ,[IsAutoAssigned] = src.[IsAutoAssigned]
	 ,[IsWithEvents] = src.[IsWithEvents]
	 ,[IsDimStmt] = src.[IsDimStmt]
	 ,[ValueExpression] = src.[ValueExpression]

	 ,[DeclarationId] = src.[DeclarationId]
	 ,[DeclarationType] = d.[DeclarationType]
	 ,[IdentifierName] = d.[IdentifierName]
	 ,[DocString] = d.[DocString]
	 ,[IsUserDefined] = d.[IsUserDefined]

	 ,[AsTypeName] = d.[AsTypeName]
	 ,[AsTypeDeclarationId] = d.[AsTypeDeclarationId]
	 ,[IsArray] = d.[IsArray]

	 ,[TypeHint] = d.[TypeHint]

	 ,[ContextStartOffset] = d.[ContextStartOffset]
	 ,[ContextEndOffset] = d.[ContextEndOffset]
	 ,[IdentifierStartOffset] = d.[IdentifierStartOffset]
	 ,[IdentifierEndOffset] = d.[IdentifierEndOffset]

	 ,[ModuleDeclarationId] = m.[DeclarationId]
	 ,[ModuleName] = md.[IdentifierName]
	 ,[Folder] = m.[Folder]

	 ,[ProjectDeclarationId] = p.[DeclarationId]
	 ,[ProjectName] = pd.[IdentifierName]
	 ,[VBProjectId] = p.[VBProjectId]

	FROM [Members] src
	INNER JOIN [Declarations] d		ON d.[Id] = src.[DeclarationId] 
	INNER JOIN [Modules] m			ON m.[DeclarationId] = d.[ParentDeclarationId]
    INNER JOIN [Declarations] md	ON md.[Id] = m.[DeclarationId]
	INNER JOIN [Projects] p			ON p.[DeclarationId] = md.[ParentDeclarationId]
	INNER JOIN [Declarations] pd	ON pd.[Id] = p.[DeclarationId];
GO

DROP VIEW IF EXISTS [Parameters_v1];
GO
CREATE VIEW [Parameters_v1]
AS
	SELECT
	  [Id] = src.[Id]
	 ,[Position] = src.[Position]
	 ,[IsParamArray] = src.[IsParamArray]
	 ,[IsOptional] = src.[IsOptional]
	 ,[IsByRef] = src.[IsByRef]
	 ,[IsByVal] = src.[IsByVal]
	 ,[IsModifierImplicit] = src.[IsModifierImplicit]
	 ,[DefaultValue] = src.[DefaultValue]

	 ,[DeclarationId] = src.[DeclarationId]
	 ,[DeclarationType] = d.[DeclarationType]
	 ,[IdentifierName] = d.[IdentifierName]
	 ,[DocString] = d.[DocString]
	 ,[IsUserDefined] = d.[IsUserDefined]

	 ,[AsTypeName] = d.[AsTypeName]
	 ,[AsTypeDeclarationId] = d.[AsTypeDeclarationId]
	 ,[IsArray] = d.[IsArray]
	 ,[TypeHint] = d.[TypeHint]
	 
	 ,[ContextStartOffset] = d.[ContextStartOffset]
	 ,[ContextEndOffset] = d.[ContextEndOffset]
	 ,[IdentifierStartOffset] = d.[IdentifierStartOffset]
	 ,[IdentifierEndOffset] = d.[IdentifierEndOffset]

	 ,[MemberDeclarationId] = mb.[DeclarationId]
	 ,[MemberDeclarationType] = mbd.[DeclarationType]
	 ,[MemberName] = mbd.[IdentifierName]

	 ,[ModuleDeclarationId] = m.[DeclarationId]
	 ,[ModuleDeclarationType] = md.[DeclarationType]
	 ,[ModuleName] = md.[IdentifierName]
	 ,[Folder] = m.[Folder]

	 ,[ProjectDeclarationId] = p.[DeclarationId]
	 ,[ProjectName] = pd.[IdentifierName]
	 ,[VBProjectId] = p.[VBProjectId]

	FROM [Parameters] src
	INNER JOIN [Declarations] d		ON d.[Id] = src.[DeclarationId] 
	INNER JOIN [Members] mb			ON mb.[DeclarationId] = d.[ParentDeclarationId]
	INNER JOIN [Declarations] mbd	ON mbd.[Id] = mb.[DeclarationId]
	INNER JOIN [Modules] m			ON m.[DeclarationId] = mbd.[ParentDeclarationId]
    INNER JOIN [Declarations] md	ON md.[Id] = m.[DeclarationId]
	INNER JOIN [Projects] p			ON p.[DeclarationId] = md.[ParentDeclarationId]
	INNER JOIN [Declarations] pd	ON pd.[Id] = p.[DeclarationId];
GO

DROP VIEW IF EXISTS [Locals_v1];
GO
CREATE VIEW [Locals_v1]
AS
	SELECT
	  [Id] = src.[Id]
	 ,[IsAutoAssigned] = src.[IsAutoAssigned]
	 ,[ValueExpression] = src.[ValueExpression]

	 ,[DeclarationId] = src.[DeclarationId]
	 ,[DeclarationType] = d.[DeclarationType]
	 ,[IdentifierName] = d.[IdentifierName]
	 ,[DocString] = d.[DocString]
	 ,[IsUserDefined] = d.[IsUserDefined]

	 ,[AsTypeName] = d.[AsTypeName]
	 ,[AsTypeDeclarationId] = d.[AsTypeDeclarationId]
	 ,[IsArray] = d.[IsArray]
	 ,[TypeHint] = d.[TypeHint]

	 ,[ContextStartOffset] = d.[ContextStartOffset]
	 ,[ContextEndOffset] = d.[ContextEndOffset]
	 ,[IdentifierStartOffset] = d.[IdentifierStartOffset]
	 ,[IdentifierEndOffset] = d.[IdentifierEndOffset]

	 ,[MemberDeclarationId] = mb.[DeclarationId]
	 ,[MemberDeclarationType] = mbd.[DeclarationType]
	 ,[MemberName] = mbd.[IdentifierName]

	 ,[ModuleDeclarationId] = m.[DeclarationId]
	 ,[ModuleDeclarationType] = md.[DeclarationType]
	 ,[ModuleName] = md.[IdentifierName]
	 ,[Folder] = m.[Folder]

	 ,[ProjectDeclarationId] = p.[DeclarationId]
	 ,[ProjectName] = pd.[IdentifierName]
	 ,[VBProjectId] = p.[VBProjectId]

	FROM [Locals] src
	INNER JOIN [Declarations] d		ON d.[Id] = src.[DeclarationId] 
	INNER JOIN [Members] mb			ON mb.[DeclarationId] = d.[ParentDeclarationId]
	INNER JOIN [Declarations] mbd	ON mbd.[Id] = mb.[DeclarationId]
	INNER JOIN [Modules] m			ON m.[DeclarationId] = mbd.[ParentDeclarationId]
    INNER JOIN [Declarations] md	ON md.[Id] = m.[DeclarationId]
	INNER JOIN [Projects] p			ON p.[DeclarationId] = md.[ParentDeclarationId]
	INNER JOIN [Declarations] pd	ON pd.[Id] = p.[DeclarationId];
GO

DROP VIEW IF EXISTS [IdentifierReferences_v1];
GO
CREATE VIEW [IdentifierReferences_v1]
AS

	SELECT 
	  [Id] = src.[Id]
	 ,[TypeHint] = src.[TypeHint]

	 ,[IsAssignmentTarget] = src.[IsAssignmentTarget]
	 ,[IsExplicitCallStatement] = src.[IsExplicitCallStatement]
	 ,[IsExplicitLetAssignment] = src.[IsExplicitLetAssignment]
	 ,[IsSetAssignment] = src.[IsSetAssignment]
	 ,[IsReDim] = src.[IsReDim]
	 ,[IsArrayAccess] = src.[IsArrayAccess]
	 ,[IsProcedureCoercion] = src.[IsProcedureCoercion]
	 ,[IsIndexedDefaultMemberAccess] = src.[IsIndexedDefaultMemberAccess]
	 ,[IsNonIndexedDefaultMemberAccess] = src.[IsNonIndexedDefaultMemberAccess]
	 ,[IsInnerRecursiveDefaultMemberAccess] = src.[IsInnerRecursiveDefaultMemberAccess]
	 ,[DefaultMemberRecursionDepth] = src.[DefaultMemberRecursionDepth]
	 
	 ,[ReferenceDeclarationId] = d.[Id]
	 ,[ReferenceDeclarationType] = d.[DeclarationType]
	 ,[ReferenceIdentifierName] = d.[IdentifierName]
	 ,[ReferenceIsUserDefined] = d.[IsUserDefined]
	 ,[ReferenceDocString] = d.[DocString]

     ,[QualifyingReferenceId] = src.[QualifyingReferenceId]

	 ,[ContextStartOffset] = d.[ContextStartOffset]
	 ,[ContextEndOffset] = d.[ContextEndOffset]
	 ,[IdentifierStartOffset] = d.[IdentifierStartOffset]
	 ,[IdentifierEndOffset] = d.[IdentifierEndOffset]
	 
	 ,[MemberDeclarationId] = null
	 ,[MemberDeclarationType] = null
	 ,[MemberName] = null
	 
     ,[ModuleDeclarationId] = md.[Id]
     ,[ModuleDeclarationType] = md.[DeclarationType]
     ,[ModuleName] = md.[IdentifierName]
     ,[Folder] = m.[Folder]

	 ,[ProjectDeclarationId] = p.[DeclarationId]
	 ,[ProjectName] = pd.[IdentifierName]
	 ,[VBProjectId] = p.[VBProjectId]

	FROM [IdentifierReferences] src
	INNER JOIN [Declarations] d		ON d.[Id] = src.[ReferencedDeclarationId]
	INNER JOIN [Modules] m			ON m.[DeclarationId] = src.[ParentDeclarationId] --> filters out member parents
	INNER JOIN [Declarations] md	ON md.[Id] = m.[DeclarationId]
	INNER JOIN [Projects] p			ON p.[DeclarationId] = md.[ParentDeclarationId]
	INNER JOIN [Declarations] pd	ON pd.[Id] = p.[DeclarationId]

	UNION ALL

	SELECT 
	  [Id] = src.[Id]
	 ,[TypeHint] = src.[TypeHint]

	 ,[IsAssignmentTarget] = src.[IsAssignmentTarget]
	 ,[IsExplicitCallStatement] = src.[IsExplicitCallStatement]
	 ,[IsExplicitLetAssignment] = src.[IsExplicitLetAssignment]
	 ,[IsSetAssignment] = src.[IsSetAssignment]
	 ,[IsReDim] = src.[IsReDim]
	 ,[IsArrayAccess] = src.[IsArrayAccess]
	 ,[IsProcedureCoercion] = src.[IsProcedureCoercion]
	 ,[IsIndexedDefaultMemberAccess] = src.[IsIndexedDefaultMemberAccess]
	 ,[IsNonIndexedDefaultMemberAccess] = src.[IsNonIndexedDefaultMemberAccess]
	 ,[IsInnerRecursiveDefaultMemberAccess] = src.[IsInnerRecursiveDefaultMemberAccess]
	 ,[DefaultMemberRecursionDepth] = src.[DefaultMemberRecursionDepth]
	 
	 ,[ReferenceDeclarationId] = d.[Id]
	 ,[ReferenceDeclarationType] = d.[DeclarationType]
	 ,[ReferenceIdentifierName] = d.[IdentifierName]
	 ,[ReferenceIsUserDefined] = d.[IsUserDefined]
	 ,[ReferenceDocString] = d.[DocString]

     ,[QualifyingReferenceId] = src.[QualifyingReferenceId]

     ,[ContextStartOffset] = d.[ContextStartOffset]
	 ,[ContextEndOffset] = d.[ContextEndOffset]
	 ,[IdentifierStartOffset] = d.[IdentifierStartOffset]
	 ,[IdentifierEndOffset] = d.[IdentifierEndOffset]
	 
	 ,[MemberDeclarationId] = mbd.[Id]
	 ,[MemberDeclarationType] = mbd.[DeclarationType]
	 ,[MemberName] = mbd.[IdentifierName]
	 
     ,[ModuleDeclarationId] = md.[Id]
     ,[ModuleDeclarationType] = md.[DeclarationType]
     ,[ModuleName] = md.[IdentifierName]
     ,[Folder] = m.[Folder]

	 ,[ProjectDeclarationId] = p.[DeclarationId]
	 ,[ProjectName] = pd.[IdentifierName]
	 ,[VBProjectId] = p.[VBProjectId]

	FROM [IdentifierReferences] src
	INNER JOIN [Declarations] d		ON src.[ReferencedDeclarationId] = d.[Id]
	INNER JOIN [Members] mb			ON mb.[DeclarationId] = src.[ParentDeclarationId] --> filters out module parents
	INNER JOIN [Declarations] mbd	ON mbd.[Id] = mb.[DeclarationId]
	INNER JOIN [Modules] m			ON m.[DeclarationId] = mbd.[ParentDeclarationId]
    INNER JOIN [Declarations] md	ON md.[Id] = m.[DeclarationId]
	INNER JOIN [Projects] p			ON p.[DeclarationId] = md.[ParentDeclarationId]
	INNER JOIN [Declarations] pd	ON pd.[Id] = p.[DeclarationId];
GO

DROP VIEW IF EXISTS [ImplementedInterfaces_v1];
GO
CREATE VIEW [ImplementedInterfaces_v1]
AS
	SELECT
	  [Id] = src.[Id]
	 ,[Folder] = src.[Folder]

	 ,[DeclarationId] = src.[DeclarationId]
	 ,[DeclarationType] = d.[DeclarationType]
	 ,[IdentifierName] = d.[IdentifierName]
	 ,[IsUserDefined] = d.[IsUserDefined]
	 ,[DocString] = d.[DocString]

	 ,[Annotations] = d.[Annotations]
	 ,[Attributes] = d.[Attributes]

	 ,[ProjectId] = p.[Id]
	 ,[ProjectDeclarationId] = p.[DeclarationId]
	 ,[ProjectName] = pd.[IdentifierName]
	 ,[VBProjectId] = p.[VBProjectId]

	 ,[IDeclarationId] = dd.[Id]
	 ,[IDeclarationType] = dd.[DeclarationType]
	 ,[IIdentifierName] = dd.[IdentifierName]
	 ,[IIsUserDefined] = dd.[IsUserDefined]
	 ,[IDocString] = dd.[DocString]

	 ,[IAnnotations] = dd.[Annotations]
	 ,[IAttributes] = dd.[Attributes]

	 ,[IProjectId] = pp.[Id]
	 ,[IProjectDeclarationId] = pp.[DeclarationId]
	 ,[IProjectName] = ppd.[IdentifierName]
	 ,[IVBProjectId] = pp.[VBProjectId]

	FROM [Modules] src
    INNER JOIN [Declarations] d			ON d.[Id] = src.[DeclarationId]
	INNER JOIN [Projects] p				ON p.[DeclarationId] = d.[ParentDeclarationId]
	INNER JOIN [Declarations] pd		ON pd.[Id] = p.[DeclarationId]
	INNER JOIN [ModuleInterfaces] mi	ON mi.[ModuleId] = src.[Id]
	INNER JOIN [Modules] i				ON i.[Id] = mi.[ImplementsModuleId]
	INNER JOIN [Declarations] dd		ON dd.[Id] = i.[DeclarationId]
	INNER JOIN [Projects] pp			ON pp.[DeclarationId] = dd.[ParentDeclarationId]
	INNER JOIN [Declarations] ppd		ON ppd.[Id] = pp.[DeclarationId];
GO

DROP VIEW IF EXISTS [EventHandlers_v1];
GO
CREATE VIEW [EventHandlers_v1]
AS
	SELECT
	  [Id] = src.[Id]
	 ,[ImplementsDeclarationId] = src.[ImplementsDeclarationId]
	 ,[Accessibility] = src.[Accessibility]
	 ,[IsAutoAssigned] = src.[IsAutoAssigned]

	 ,[DeclarationId] = src.[DeclarationId]
	 ,[DeclarationType] = d.[DeclarationType]
	 ,[IdentifierName] = d.[IdentifierName]
	 ,[IsUserDefined] = d.[IsUserDefined]

	 ,[AsTypeName] = d.[AsTypeName]
	 ,[AsTypeDeclarationId] = d.[AsTypeDeclarationId]
	 ,[IsArray] = d.[IsArray]
	 ,[DocString] = d.[DocString]

	 ,[Annotations] = d.[Annotations]
	 ,[Attributes] = d.[Attributes]

	 ,[ContextStartOffset] = d.[ContextStartOffset]
	 ,[ContextEndOffset] = d.[ContextEndOffset]
	 ,[IdentifierStartOffset] = d.[IdentifierStartOffset]
	 ,[IdentifierEndOffset] = d.[IdentifierEndOffset]

	 ,[ModuleId] = m.[Id]
	 ,[ModuleDeclarationId] = m.[DeclarationId]
	 ,[ModuleName] = md.[IdentifierName]
	 ,[Folder] = m.[Folder]

	 ,[ProjectId] = p.[Id]
	 ,[ProjectDeclarationId] = p.[DeclarationId]
	 ,[ProjectName] = pd.[IdentifierName]
	 ,[VBProjectId] = p.[VBProjectId]

	 ,[TargetModuleId] = i.[Id]
	 ,[TargetModuleDeclarationId] = i.[DeclarationId]
	 ,[TargetFolder] = i.[Folder]
	 ,[TargetDeclarationId] = src.[ImplementsDeclarationId]
	 ,[TargetDeclarationType] = dd.[DeclarationType]
	 ,[TargetIdentifierName] = dd.[IdentifierName]
	 ,[TargetIsUserDefined] = dd.[IsUserDefined]
	 ,[TargetDocString] = dd.[DocString]
	 ,[TargetAnnotations] = dd.[Annotations]
	 ,[TargetAttributes] = dd.[Attributes]
	 ,[TargetContextStartOffset] = dd.[ContextStartOffset]
	 ,[TargetContextEndOffset] = dd.[ContextEndOffset]
	 ,[TargetHighlightStartOffset] = dd.[IdentifierStartOffset]
	 ,[TargetHighlightEndOffset] = dd.[IdentifierEndOffset]
	 ,[TargetProjectId] = pp.[Id]
	 ,[TargetProjectDeclarationId] = pp.[DeclarationId]
	 ,[TargetProjectName] = ppd.[IdentifierName]
	 ,[TargetVBProjectId] = pp.[VBProjectId]

	FROM [Members] src
	INNER JOIN [Declarations] d		ON d.[Id] = src.[DeclarationId]
	INNER JOIN [Modules] m			ON m.[DeclarationId] = d.[ParentDeclarationId]
    INNER JOIN [Declarations] md	ON md.[Id] = m.[DeclarationId]
	INNER JOIN [Projects] p			ON p.[DeclarationId] = md.[ParentDeclarationId]
	INNER JOIN [Declarations] pd	ON pd.[Id] = p.[DeclarationId]
	INNER JOIN [Modules] i			ON i.[DeclarationId] = src.[ImplementsDeclarationId]
	INNER JOIN [Declarations] dd	ON dd.[Id] = i.[DeclarationId]
	INNER JOIN [Projects] pp		ON pp.[DeclarationId] = i.[ParentDeclarationId]
	INNER JOIN [Declarations] ppd	ON ppd.[Id] = pp.[DeclarationId];
GO