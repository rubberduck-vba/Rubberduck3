﻿using System.Collections.Generic;
using Rubberduck.Parsing.COM.Abstract;
using Rubberduck.Parsing.Model.Symbols;
using Rubberduck.VBEditor;

namespace Rubberduck.Parsing.COM.Reflection
{
    public class SerializedReferencedDeclarationsCollector : ReferencedDeclarationsCollectorBase
    {
        private readonly IComProjectDeserializer _deserializer;

        public SerializedReferencedDeclarationsCollector(IDeclarationsFromComProjectLoader declarationsFromComProjectLoader, IComProjectDeserializer deserializer)
        :base(declarationsFromComProjectLoader)
        {
            _deserializer = deserializer;
        }

        public override IReadOnlyCollection<Declaration> ExtractDeclarations(ReferenceInfo reference)
        {
            if (!_deserializer.SerializedVersionExists(reference))
            {
                return new List<Declaration>();
            }

            return LoadDeclarationsFromProvider(reference);
        }

        private IReadOnlyCollection<Declaration> LoadDeclarationsFromProvider(ReferenceInfo reference)
        {
            var type = _deserializer.DeserializeProject(reference);
            return LoadDeclarationsFromComProject(type); 
        }
    }
}
