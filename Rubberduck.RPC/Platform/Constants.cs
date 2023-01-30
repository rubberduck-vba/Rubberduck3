using System;
using System.ComponentModel.DataAnnotations;

namespace Rubberduck.RPC.Platform
{
    public class Constants
    {
        public class Console
        {
            /// <summary>
            /// Controls whether <c>Message</c> and <c>Verbose</c> messages are output.
            /// </summary>
            public class VerbosityOptions
            {
                public enum AsStringEnum
                {
                    /// <summary>
                    /// Tracing is disabled.
                    /// </summary>
                    Off,
                    /// <summary>
                    /// Tracing logs messages.
                    /// </summary>
                    Message,
                    /// <summary>
                    /// Tracing logs verbose messages.
                    /// </summary>
                    Verbose,
                }

                /// <summary>
                /// Tracing is disabled.
                /// </summary>
                public const string Off = "off";
                /// <summary>
                /// Tracing logs messages.
                /// </summary>
                public const string Message = "message";
                /// <summary>
                /// Tracing logs verbose messages.
                /// </summary>
                public const string Verbose = "verbose";
            }

            public class FontWeightOptions
            {
                [Flags]
                public enum AsFlagsEnum
                {
                    Normal = FontWeightOptions.Normal,
                    Light = FontWeightOptions.Light,
                    SemiBold = FontWeightOptions.SemiBold,
                    Bold = FontWeightOptions.Bold,
                    Black = FontWeightOptions.Black,
                }

                public const int Normal = 0;
                public const int Light = 1;
                public const int SemiBold = 2;
                public const int Bold = 4;
                public const int Black = 8;
            }
        }

        public class LocalDb
        {

        }

        public class LSP
        {
            public class WorkDoneProgressKind
            {
                public enum AsStringEnum
                {
                    Begin,
                    Report,
                    End,
                }

                public const string Begin = "begin";
                public const string Report = "report";
                public const string End = "end";
            }

            public class FailureHandlingKind
            {
                public enum AsStringEnum
                {
                    /// <summary>
                    /// Applying the workspace change is simply aborted if one of the changes provided fails. All operations executed before the failing operation stay executed.
                    /// </summary>
                    Abort,
                    /// <summary>
                    /// All operations are executed transactionally. That means they either all succeed, or no changes at all are applied to the workspace.
                    /// </summary>
                    Transactional,
                    /// <summary>
                    /// The client tries to undo the operations already executed. Note: there is no guarantee that this is succeeding.
                    /// </summary>
                    Undo,
                    /// <summary>
                    /// If the workspace edit contains only textual file changes they are executed transactionally. If resource changes are part of the change, the failure handling strategy is 'abort'.
                    /// </summary>
                    TextOnlyTransactional,
                }

                /// <summary>
                /// Applying the workspace change is simply aborted if one of the changes provided fails. All operations executed before the failing operation stay executed.
                /// </summary>
                public const string Abort = "abort";
                /// <summary>
                /// All operations are executed transactionally. That means they either all succeed, or no changes at all are applied to the workspace.
                /// </summary>
                public const string Transactional = "transactional";
                /// <summary>
                /// The client tries to undo the operations already executed. Note: there is no guarantee that this is succeeding.
                /// </summary>
                public const string Undo = "undo";
                /// <summary>
                /// If the workspace edit contains only textual file changes they are executed transactionally. If resource changes are part of the change, the failure handling strategy is 'abort'.
                /// </summary>
                public const string TextOnlyTransactional = "textOnlyTransactional";
            }
        }

        /// <summary>
        /// Strings that are used for categorizing code actions.
        /// </summary>
        public class CodeActionKind
        {
            public enum AsStringEnum
            {
                [Display(Name = CodeActionKind.Empty)]
                Empty,
                [Display(Name = CodeActionKind.QuickFix)]
                QuickFix,
                [Display(Name = CodeActionKind.Refactor)]
                Refactor,
                [Display(Name = CodeActionKind.RefactorExtract)]
                RefactorExtract,
                [Display(Name = CodeActionKind.RefactorInline)]
                RefactorInline,
                [Display(Name = CodeActionKind.RefactorRewrite)]
                RefactorRewrite,
                [Display(Name = CodeActionKind.Source)]
                SourceCode,
                [Display(Name = CodeActionKind.SourceFixAll)]
                SourceCodeFixAll
            }

            /// <summary>
            /// Empty code action kind.
            /// </summary>
            public const string Empty = "";
            /// <summary>
            /// Base kind for quickfix actions.
            /// </summary>
            public const string QuickFix = "quickfix";
            /// <summary>
            /// Base kind for refactor actions.
            /// </summary>
            public const string Refactor = "refactor";

            /// <summary>
            /// Base kind for 'extract' refactor actions.
            /// </summary>
            /// <example>refactor.extract.method</example>
            /// <example>refactor.extract.interface</example>
            /// <example>refactor.extract.variable</example>
            public const string RefactorExtract = "refactor.extract";
            /// <summary>
            /// Base kind for 'inline' refactor actions.
            /// </summary>
            /// <example>refactor.inline.method</example>
            /// <example>refactor.inline.variable</example>
            /// <example>refactor.inline.constant</example>
            public const string RefactorInline = "refactor.inline";
            /// <summary>
            /// Base kind for 'rewrite' refactor actions.
            /// </summary>
            /// <example>refactor.rewrite.remove-parameter</example>
            /// <example>refactor.rewrite.encapsulate-field</example>
            public const string RefactorRewrite = "refactor.rewrite";

            /// <summary>
            /// Base kind for 'source' actions.
            /// </summary>
            /// <remarks>
            /// Source code actions apply to the entire file.
            /// </remarks>
            /// <example>source.reorder-members</example>
            public const string Source = "source";

            /// <summary>
            /// Base kind for a 'fix all' source action.
            /// </summary>
            /// <remarks>
            /// Automatically fixes errors that have a clear fix that does not require user input.
            /// </remarks>
            public const string SourceFixAll = "source.fixAll";
        }

        public class CodeActionTriggerKind
        {
            public enum AsEnum
            {
                Invoked = CodeActionTriggerKind.Invoked,
                Automatic = CodeActionTriggerKind.Automatic,
            }

            public const int Invoked = 1;
            public const int Automatic = 2;
        }

        public class CompletionItemKind
        {
            public enum AsEnum
            {
                Text = CompletionItemKind.Text,
                Method = CompletionItemKind.Method,
                Function = CompletionItemKind.Function,
                Constructor = CompletionItemKind.Constructor,
                Field = CompletionItemKind.Field,
                Variable = CompletionItemKind.Variable,
                Class = CompletionItemKind.Class,
                Interface = CompletionItemKind.Interface,
                Module = CompletionItemKind.Module,
                Property = CompletionItemKind.Property,
                Unit = CompletionItemKind.Unit,
                Enum = CompletionItemKind.Enum,
                Keyword = CompletionItemKind.Keyword,
                Snippet = CompletionItemKind.Snippet,
                Color = CompletionItemKind.Color,
                File = CompletionItemKind.File,
                Reference = CompletionItemKind.Reference,
                Folder = CompletionItemKind.Folder,
                EnumMember = CompletionItemKind.EnumMember,
                Constant = CompletionItemKind.Constant,
                Struct = CompletionItemKind.Struct,
                Event = CompletionItemKind.Event,
                Operator = CompletionItemKind.Operator,
                TypeParameter = CompletionItemKind.TypeParameter,
            }

            public const int Text = 1;
            public const int Method = 2;
            public const int Function = 3;
            public const int Constructor = 4;
            public const int Field = 5;
            public const int Variable = 6;
            public const int Class = 7;
            public const int Interface = 8;
            public const int Module = 9;
            public const int Property = 10;
            public const int Unit = 11;
            public const int Value = 12;
            public const int Enum = 13;
            public const int Keyword = 14;
            public const int Snippet = 15;
            public const int Color = 16;
            public const int File = 17;
            public const int Reference = 18;
            public const int Folder = 19;
            public const int EnumMember = 20;
            public const int Constant = 21;
            public const int Struct = 22;
            public const int Event = 23;
            public const int Operator = 24;
            public const int TypeParameter = 25;
        }

        /// <summary>
        /// Extra annotations that tweak the rendering of a completion item.
        /// </summary>
        public class CompletionItemTag
        {
            public enum AsEnum
            { 
                None = 0,
                /// <summary>
                /// Render a completion as obsolete, usually using a strike-out.
                /// </summary>
                Deprecated = CompletionItemTag.Deprecated,
            }

            /// <summary>
            /// Render a completion as obsolete, usually using a strike-out.
            /// </summary>
            public const int Deprecated = 1;
        }

        public class CompletionTriggerKind
        {
            public enum AsEnum
            {
                Invoked = CompletionTriggerKind.Invoked,
                TriggerCharacter = CompletionTriggerKind.TriggerCharacter,
                TriggerForIncompleteCompletions = CompletionTriggerKind.TriggerForIncompleteCompletions,
            }

            /// <summary>
            /// Completion was triggered by typing an identifier, manual invocation, or via an API.
            /// </summary>
            public const int Invoked = 1;
            /// <summary>
            /// Completion was triggered by a trigger character specified by the configured 'triggerCharacters'.
            /// </summary>
            public const int TriggerCharacter = 2;
            /// <summary>
            /// Completion was re-triggered as the current completion list is incomplete.
            /// </summary>
            public const int TriggerForIncompleteCompletions = 3;
        }

        public class DocumentDiagnosticReportKind
        {
            public enum AsStringEnum
            {
                Unchanged,
                Full,
            }

            public const string Full = "full";
            public const string Unchanged = "unchanged";
        }

        /// <summary>
        /// Describes the severity level of a diagnostic.
        /// </summary>
        public class DiagnosticSeverity
        {
            public enum AsEnum
            {
                /// <summary>
                /// Reports an error diagnostic.
                /// </summary>
                Error = DiagnosticSeverity.Error,
                /// <summary>
                /// Reports a warning diagnostic.
                /// </summary>
                Warning = DiagnosticSeverity.Warning,
                /// <summary>
                /// Reports an informational diagnostic.
                /// </summary>
                Information = DiagnosticSeverity.Information,
                /// <summary>
                /// Reports a hint diagnostic.
                /// </summary>
                Hint = DiagnosticSeverity.Hint,
            }

            /// <summary>
            /// Reports an error diagnostic.
            /// </summary>
            public const int Error = 1;
            /// <summary>
            /// Reports a warning diagnostic.
            /// </summary>
            public const int Warning = 2;
            /// <summary>
            /// Reports an informational diagnostic.
            /// </summary>
            public const int Information = 3;
            /// <summary>
            /// Reports a hint diagnostic.
            /// </summary>
            public const int Hint = 4;
        }

        /// <summary>
        /// Describes tags that can annotate a diagnostic.
        /// </summary>
        public class DiagnosticTags
        {
            public enum AsEnum
            {
                None = 0,
                /// <summary>
                /// Unused or unnecessary code.
                /// </summary>
                /// <remarks>
                /// Clients are allowed to render diagnostics with this tag faded out instead of having an error squiggle.
                /// </remarks>
                Unnecessary = DiagnosticTags.Unnecessary,
                /// <summary>
                /// Deprecated or obsolete code.
                /// </summary>
                /// <remarks>
                /// Clients are allowed to rendered diagnostics with this tag strike through.
                /// </remarks>
                Obsolete = DiagnosticTags.Obsolete,
            }

            /// <summary>
            /// Unused or unnecessary code.
            /// </summary>
            /// <remarks>
            /// Clients are allowed to render diagnostics with this tag faded out instead of having an error squiggle.
            /// </remarks>
            public const int Unnecessary = 1;
            /// <summary>
            /// Deprecated or obsolete code.
            /// </summary>
            /// <remarks>
            /// Clients are allowed to rendered diagnostics with this tag strike through.
            /// </remarks>
            public const int Obsolete = 2;
        }

        public class DocumentHighlightKind
        {
            public enum AsEnum
            {
                Text = DocumentHighlightKind.Text,
                Read = DocumentHighlightKind.Read,
                Write = DocumentHighlightKind.Write,
            }

            public const int Text = 1;
            public const int Read = 2;
            public const int Write = 3;
        }

        public class FileChangeType
        {
            public enum AsEnum
            {
                /// <summary>
                /// File/folder was created.
                /// </summary>
                Created = FileChangeType.Created,
                /// <summary>
                /// File/folder was modified.
                /// </summary>
                Modified = FileChangeType.Modified,
                /// <summary>
                /// File/folder was deleted.
                /// </summary>
                Deleted = FileChangeType.Deleted,
            }

            /// <summary>
            /// File/folder was created.
            /// </summary>
            public const int Created = 1;
            /// <summary>
            /// File/folder was modified.
            /// </summary>
            public const int Modified = 2;
            /// <summary>
            /// File/folder was deleted.
            /// </summary>
            public const int Deleted = 3;
        }

        public class FileOperationPatternKind
        {
            public enum AsStringEnum
            {
                File,
                Folder,
            }

            public const string File = "file";
            public const string Folder = "folder";
        }

        /// <summary>
        /// An extensible set of predefined range kind <c>string</c> values.
        /// </summary>
        public class FoldingRangeKind
        {
            public enum AsStringEnum
            {
                /// <summary>
                /// Folds attribute lines.
                /// </summary>
                Attribute,
                /// <summary>
                /// Folds a multiline comment block.
                /// </summary>
                Comment,
                /// <summary>
                /// Folds an annotation-defined (<c>@region</c>, <c>@endregion</c>) block.
                /// </summary>
                Region,
                /// <summary>
                /// Folds a syntax code block, e.g. <c>For...Next</c>
                /// </summary>
                Block,
                /// <summary>
                /// Folds a scope block, i.e. <c>Function</c>, <c>Property</c>, and <c>Sub</c> blocks.
                /// </summary>
                Scope,
            }

            public const string Attribute = "attribute";
            public const string Comment = "comment";
            public const string Region = "region";
            public const string Block = "block";
            public const string Scope = "scope";
        }

        public class InlayHintKind
        {
            public enum AsEnum
            {
                /// <summary>
                /// An inlay hint that is for a type annotation.
                /// </summary>
                Type = InlayHintKind.Type,
                /// <summary>
                /// An inlay hint that is for a parameter.
                /// </summary>
                Parameter = InlayHintKind.Parameter,
            }

            /// <summary>
            /// An inlay hint that is for a type annotation.
            /// </summary>
            public const int Type = 1;
            /// <summary>
            /// An inlay hint that is for a parameter.
            /// </summary>
            public const int Parameter = 2;
        }

        public class InsertTextMode
        {
            public enum AsEnum
            {
                /// <summary>
                /// Inserts text as-is into the document.
                /// </summary>
                AsIs = InsertTextMode.AsIs,
                /// <summary>
                /// Adjusts indentation before inserting into the document.
                /// </summary>
                AdjustIndentation = InsertTextMode.AdjustIndentation,
            }

            /// <summary>
            /// Inserts text as-is into the document.
            /// </summary>
            public const int AsIs = 1;
            /// <summary>
            /// Adjusts indentation before inserting into the document.
            /// </summary>
            public const int AdjustIndentation = 2;
        }

        /// <summary>
        /// Describes the content type that a client supports in various result literals like `Hover`, `ParameterInfo` or `CompletionItem`.
        /// </summary>
        public class MarkupKind
        {
            public enum AsStringEnum
            {
                /// <summary>
                /// Plain text is supported as a content format.
                /// </summary>
                PlainText,
                /// <summary>
                /// Markdown is supported as a content format.
                /// </summary>
                Markdown
            }

            /// <summary>
            /// Plain text is supported as a content format.
            /// </summary>
            public const string PlainText = "plaintext";
            /// <summary>
            /// Markdown is supported as a content format.
            /// </summary>
            public const string Markdown = "markdown";
        }

        public class MessageType
        {
            public enum AsEnum
            {
                /// <summary>
                /// An error message.
                /// </summary>
                Error = MessageType.Error,
                /// <summary>
                /// A warning message.
                /// </summary>
                Warning = MessageType.Warning,
                /// <summary>
                /// An information message.
                /// </summary>
                Info = MessageType.Info,
                /// <summary>
                /// A log (debug/trace) message.
                /// </summary>
                Log = MessageType.Log,
            }

            /// <summary>
            /// An error message.
            /// </summary>
            public const int Error = 1;
            /// <summary>
            /// A warning message.
            /// </summary>
            public const int Warning = 2;
            /// <summary>
            /// An information message.
            /// </summary>
            public const int Info = 3;
            /// <summary>
            /// A log (debug/trace) message.
            /// </summary>
            public const int Log = 4;
        }

        public class MonikerUniquenessLevel
        {
            public enum AsStringEnum
            {
                /// <summary>
                /// The moniker is only unique inside a document.
                /// </summary>
                Document,
                /// <summary>
                /// The moniker is unique inside a project for which a dump got created.
                /// </summary>
                Project,
                /// <summary>
                /// The moniker is unique inside the group to which a project belongs.
                /// </summary>
                Group,
                /// <summary>
                /// The moniker is unique inside the moniker scheme.
                /// </summary>
                Scheme,
                /// <summary>
                /// The moniker is globally unique.
                /// </summary>
                Global,
            }

            /// <summary>
            /// The moniker is only unique inside a document.
            /// </summary>
            public const string Document = "document";
            /// <summary>
            /// The moniker is unique inside a project for which a dump got created.
            /// </summary>
            public const string Project = "project";
            /// <summary>
            /// The moniker is unique inside the group to which a project belongs.
            /// </summary>
            public const string Group = "group";
            /// <summary>
            /// The moniker is unique inside the moniker scheme.
            /// </summary>
            public const string Scheme = "scheme";
            /// <summary>
            /// The moniker is globally unique.
            /// </summary>
            public const string Global = "global";
        }

        public class MonikerKind
        {
            public enum AsStringEnum
            {
                /// <summary>
                /// The moniker represents a symbol that is imported into a project.
                /// </summary>
                Import,
                /// <summary>
                /// The moniker represents a symbol that is exported from a project.
                /// </summary>
                Export,
                /// <summary>
                /// The moniker represents a symbol that is local to a project.
                /// </summary>
                Local,
            }

            /// <summary>
            /// The moniker represents a symbol that is imported into a project.
            /// </summary>
            public const string Import = "import";

            /// <summary>
            /// The moniker represents a symbol that is exported from a project.
            /// </summary>
            public const string Export = "export";

            /// <summary>
            /// The moniker represents a symbol that is local to a project.
            /// </summary>
            public const string Local = "local";
        }

        public class PositionEncodingKind
        {
            public enum AsStringEnum
            {
                Utf8,
                Utf16,
                Utf32,
            }

            public const string UTF8 = "utf-8";
            public const string UTF16 = "utf-16";
            public const string UTF32 = "utf-32";
        }

        public class PrepareSupportDefaultBehavior
        {
            public enum AsEnum
            {
                None = 0,
                /// <summary>
                /// The client's default behavior is to select the identifier according to the language's syntax rules.
                /// </summary>
                Identifier = PrepareSupportDefaultBehavior.Identifier,
            }

            /// <summary>
            /// The client's default behavior is to select the identifier according to the language's syntax rules.
            /// </summary>
            public const int Identifier = 1;
        }

        /// <summary>
        /// Describes operations that can be executed against file or folder resources.
        /// </summary>
        public class ResourceOperationKind
        {
            public enum AsStringEnum
            {
                /// <summary>
                /// A 'create' operation creates a new file or folder.
                /// </summary>
                Create,
                /// <summary>
                /// A 'rename' operation renames an existing file or folder.
                /// </summary>
                Rename,
                /// <summary>
                /// A 'delete' operation deletes an existing file or folder.
                /// </summary>
                Delete,
            }

            /// <summary>
            /// A 'create' operation creates a new file or folder.
            /// </summary>
            public const string Create = "create";
            /// <summary>
            /// A 'rename' operation renames an existing file or folder.
            /// </summary>
            public const string Rename = "rename";
            /// <summary>
            /// A 'delete' operation deletes an existing file or folder.
            /// </summary>
            public const string Delete = "delete";
        }

        public class TextDocumentSaveReason
        {
            public enum AsEnum
            {
                /// <summary>
                /// Manually triggered, e.g. user pressing 'Save', debugging session started, or via API call.
                /// </summary>
                Manual = TextDocumentSaveReason.Manual,
                /// <summary>
                /// Automatic save after a delay.
                /// </summary>
                AfterDelay = TextDocumentSaveReason.AfterDelay,
                /// <summary>
                /// The editor lost focus.
                /// </summary>
                FocusOut = TextDocumentSaveReason.FocusOut,
            }

            /// <summary>
            /// Manually triggered, e.g. user pressing 'Save', debugging session started, or via API call.
            /// </summary>
            public const int Manual = 1;
            /// <summary>
            /// Automatic save after a delay.
            /// </summary>
            public const int AfterDelay = 2;
            /// <summary>
            /// The editor lost focus.
            /// </summary>
            public const int FocusOut = 3;
        }

        public class SignatureHelpTriggerKind
        {
            public enum AsEnum
            {
                Invoked = SignatureHelpTriggerKind.Invoked,
                TriggerCharacter = SignatureHelpTriggerKind.TriggerCharacter,
                ContentChange = SignatureHelpTriggerKind.ContentChange,
            }

            public const int Invoked = 1;
            public const int TriggerCharacter = 2;
            public const int ContentChange = 3;
        }

        /// <summary>
        /// Describes the kind of a symbol.
        /// </summary>
        public class SymbolKind
        {
            public enum AsEnum
            {
                File = SymbolKind.File,
                Module = SymbolKind.Module,
                Namespace = SymbolKind.Namespace,
                Package = SymbolKind.Package,
                Class = SymbolKind.Class,
                Method = SymbolKind.Method,
                Property = SymbolKind.Property,
                Field = SymbolKind.Field,
                Enum = SymbolKind.Enum,
                Interface = SymbolKind.Interface,
                Function = SymbolKind.Function,
                Variable = SymbolKind.Variable,
                Constant = SymbolKind.Constant,
                String = SymbolKind.String,
                Number =  SymbolKind.Number,
                Boolean = SymbolKind.Boolean,
                Array = SymbolKind.Array,
                Object = SymbolKind.Object,
                Key = SymbolKind.Key,
                Null = SymbolKind.Null,
                EnumMember = SymbolKind.EnumMember,
                Struct = SymbolKind.Struct,
                Event = SymbolKind.Event,
                Operator = SymbolKind.Operator,
                TypeParameter = SymbolKind.TypeParameter,
            }

            public const int File = 1;
            public const int Module = 2;
            public const int Namespace = 3;
            public const int Package = 4;
            public const int Class = 5;
            public const int Method = 6;
            public const int Property = 7;
            public const int Field = 8;
            public const int Constructor = 9;
            public const int Enum = 10;
            public const int Interface = 11;
            public const int Function = 12;
            public const int Variable = 13;
            public const int Constant = 14;
            public const int String = 15;
            public const int Number = 16;
            public const int Boolean = 17;
            public const int Array = 18;
            public const int Object = 19;
            public const int Key = 20;
            public const int Null = 21;
            public const int EnumMember = 22;
            public const int Struct = 23;
            public const int Event = 24;
            public const int Operator = 25;
            public const int TypeParameter = 26;
        }

        /// <summary>
        /// Extra annotations that tweak the rendering of a symbol.
        /// </summary>
        public class SymbolTag
        {
            public enum AsEnum
            {
                None = 0,
                /// <summary>
                /// Render a symbol as obsolete, usually using a strike-out.
                /// </summary>
                Deprecated = SymbolTag.Deprecated,
            }

            /// <summary>
            /// Render a symbol as obsolete, usually using a strike-out.
            /// </summary>
            public const int Deprecated = 1;
        }

        /// <summary>
        /// LSP predefined/default semantic token types. Client may announce additional supported values in the corresponding client capability.
        /// </summary>
        public class SemanticTokenTypes
        {
            public enum AsStringEnum
            {
                Type,
                Namespace,
                Class,
                Enum,
                Interface,
                Struct,
                TypeParameter,
                Parameter,
                EnumMember,
                Event,
                Function,
                Method,
                Macro,
                Keyword,
                Modifier,
                Comment,
                String,
                Number,
                RegExp,
                Operator,
                Decorator,
            }

            /// <summary>
            /// Represents a generic type. Acts as a fallback for token types that can't be mapped.
            /// </summary>
            public const string Type = "type";
            public const string Namespace = "namespace";
            public const string Class = "class";
            public const string Enum = "enum";
            public const string Interface = "interface";
            public const string Struct = "struct";
            public const string TypeParameter = "typeParameter";
            public const string Parameter = "parameter";
            public const string Variable = "variable";
            public const string Property = "property";
            public const string EnumMember = "enumMember";
            public const string Event = "event";
            public const string Function = "function";
            public const string Method = "method";
            public const string Macro = "macro";
            public const string Keyword = "keyword";
            public const string Modifier = "modifier";
            public const string Comment = "comment";
            public const string String = "string";
            public const string Number = "number";
            public const string RegExp = "regexp";
            public const string Operator = "operator";
            public const string Decorator = "decorator";
        }

        /// <summary>
        /// LSP predefined/default semantic token modifiers. Client may announce additional supported values in the corresponding client capability.
        /// </summary>
        public class SemanticTokenModifiers
        {
            public enum AsStringEnum
            {
                Declaration,
                Definition,
                ReadOnly,
                Static,
                Deprecated,
                Abstract,
                Async,
                Modification,
                Documentation,
                DefaultLibrary,
            }

            public const string Declaration = "declaration";
            public const string Definition = "definition";
            public const string ReadOnly = "readonly";
            public const string Static = "static";
            public const string Deprecated = "deprecated";
            public const string Abstract = "abstract";
            public const string Async = "async";
            public const string Modification = "modification";
            public const string Documentation = "documentation";
            public const string DefaultLibrary = "defaultLibrary";
        }

        /// <summary>
        /// Defines how the host/editor should sync document changes to the language server.
        /// </summary>
        public class TextDocumentSyncKind
        {
            public enum AsEnum
            {
                /// <summary>
                /// Documents should not be synced at all.
                /// </summary>
                None = TextDocumentSyncKind.None,
                /// <summary>
                /// Documents are synced by always sending the full content of the document.
                /// </summary>
                Full = TextDocumentSyncKind.Full,
                /// <summary>
                /// Documents are synced by sending the full content on open, then only incremental updates.
                /// </summary>
                Incremental = TextDocumentSyncKind.Incremental,
            }

            /// <summary>
            /// Documents should not be synced at all.
            /// </summary>
            public const int None = 0;

            /// <summary>
            /// Documents are synced by always sending the full content of the document.
            /// </summary>
            public const int Full = 1;

            /// <summary>
            /// Documents are synced by sending the full content on open, then only incremental updates.
            /// </summary>
            public const int Incremental = 2;
        }

        public class TokenFormat
        {
            public enum AsStringEnum
            {
                Relative,
            }

            public const string Relative = "relative";
        }

        /// <summary>
        /// Represents the level of verbosity with which the server systematically reports its execution trace using '$/logTrace' notifications.
        /// </summary>
        /// <remarks>
        /// Client may set this verbosity level post-initialization using a '$/setTrace' request.
        /// </remarks>
        public class TraceValue
        {
            public enum AsStringEnum
            {
                /// <summary>
                /// Trace output is off: server produces no output.
                /// </summary>
                Off,
                /// <summary>
                /// Trace output is enabled, but only the 'message' field is output.
                /// </summary>
                Messages,
                /// <summary>
                /// Trace output is enabled, and both 'message' and 'verbose' fields are output.
                /// </summary>
                Verbose,
            }

            /// <summary>
            /// Trace output is off: server produces no output.
            /// </summary>
            public const string Off = "off";
            /// <summary>
            /// Trace output is enabled, but only the 'message' field is output.
            /// </summary>
            public const string Messages = "messages";
            /// <summary>
            /// Trace output is enabled, and both 'message' and 'verbose' fields are output.
            /// </summary>
            public const string Verbose = "verbose";
        }
    }
}
