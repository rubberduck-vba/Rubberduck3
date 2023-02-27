﻿using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Rubberduck.Parsing.COM.Abstract;
using Rubberduck.Parsing.Model.ComReflection;
using Rubberduck.Parsing.Model.Symbols;
using Rubberduck.VBEditor;

namespace Rubberduck.Parsing.COM.Reflection
{
    public class LibraryReferencedDeclarationsCollector : ReferencedDeclarationsCollectorBase
    {
        private readonly IComLibraryProvider _comLibraryProvider;

        public LibraryReferencedDeclarationsCollector(IDeclarationsFromComProjectLoader declarationsFromComProjectLoader, IComLibraryProvider comLibraryProvider)
        :base(declarationsFromComProjectLoader)
        {
            _comLibraryProvider = comLibraryProvider;
        }

        public override IReadOnlyCollection<Declaration> ExtractDeclarations(ReferenceInfo reference)
        {
            return LoadDeclarationsFromLibrary(reference);
        }

        private IReadOnlyCollection<Declaration> LoadDeclarationsFromLibrary(ReferenceInfo reference)
        {
            var libraryPath = reference.FullPath;
            // Failure to load might mean that it's a "normal" VBProject that will get loaded through a different channel.
            var typeLibrary = GetTypeLibrary(libraryPath);
            if (typeLibrary is null)
            {
                return new List<Declaration>();
            }

            var type = new ComProject(typeLibrary, libraryPath);
            var declarations = LoadDeclarationsFromComProject(type);

            return declarations;
        }

        private ITypeLib GetTypeLibrary(string libraryPath)
        {
            return _comLibraryProvider.LoadTypeLibrary(libraryPath);
        }
    }
}
