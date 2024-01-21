using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.InternalApi.Model.Declarations
{
    public abstract record class Symbol : DocumentSymbol
    {
        protected Symbol(SymbolKind kind, string name, Uri uri)
        {
            Kind = kind;
            Name = name;
            Uri = uri;
        }

        public bool IsUserDefined { get; init; }
        public Uri Uri { get; init; }

        public Accessibility DeclaredAccessibility { get; init; }
        public Accessibility EffectiveAccessibility => DeclaredAccessibility.Effective();
    }

    public record class ProjectSymbol : Symbol
    {
        public ProjectSymbol(string name, Uri uri, IEnumerable<Symbol> children)
            : base(SymbolKind.Package, name, uri)
        {
            DeclaredAccessibility = Accessibility.Global;
            Children = new(children);
        }
    }

    public record class FileSymbol : Symbol
    {
        public FileSymbol(string name, Uri uri, IEnumerable<ComponentSymbol> children)
            : base(SymbolKind.File, name, uri)
        {
            DeclaredAccessibility = Accessibility.Global;
            Children = new(children);
        }
    }

    public abstract record class ComponentSymbol : Symbol
    {
        protected ComponentSymbol(SymbolKind kind, Accessibility accessibility, string name, Uri uri, IEnumerable<Symbol> children)
            : base(kind, name, uri)
        {
            DeclaredAccessibility = accessibility;
            Children = new(children);
        }

        
    }

    public record class StandardModuleSymbol : ComponentSymbol
    {
        public StandardModuleSymbol(string name, Uri uri, IEnumerable<Symbol> children)
            : base(SymbolKind.Module, Accessibility.Global, name, uri, children)
        {
        }
    }

    public record class ClassModuleSymbol : ComponentSymbol
    {
        public ClassModuleSymbol(Accessibility accessibility, string name, Uri uri, IEnumerable<Symbol> children)
            : base(SymbolKind.Class, accessibility, name, uri, children)
        {
        }

        public bool HasPredeclaredId { get; init; }
        public bool IsExposed { get; init; }
        public bool IsExtensible { get; init; }

        public bool HasDefaultMember { get; }
        public bool IsInterface => Subtypes.Any(type => type.IsUserDefined);

        public IEnumerable<string> SupertypeNames { get; init; } = [];
        public IEnumerable<ClassModuleSymbol>? Supertypes { get; init; } = default;

        public IEnumerable<ClassModuleSymbol> Subtypes { get; init; } = [];
    }
}
