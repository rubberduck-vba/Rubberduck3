﻿using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO.Abstractions;

using MemoryStream = System.IO.MemoryStream;
using StreamWriter = System.IO.StreamWriter;
using Path = System.IO.Path;

namespace Rubberduck.SettingsProvider
{
    public class XmlPersistenceService<T> : XmlPersistenceServiceBase<T> 
        where T : class, IEquatable<T>, new()
    {
        private const string DefaultConfigFile = "rubberduck.config";

        public XmlPersistenceService(
            IPersistencePathProvider pathProvider,
            IFileSystem fileSystem) 
            : base(pathProvider, fileSystem) { }

        protected override string FilePath => Path.Combine(RootPath, DefaultConfigFile);

        protected override T Read(string path)
        {
            var doc = GetConfigurationDoc(path);
            var node = GetNodeByName(doc, typeof(T).Name);
            if (node is null)
            {
                return default;
            }

            using (var reader = node.CreateReader())
            {
                var deserializer = new XmlSerializer(typeof(T));
                try
                {
                    return (T)deserializer.Deserialize(reader);
                }
                catch
                {
                    return default;
                }
            }
        }

        protected override void Write(T toSerialize, string path)
        {
            var doc = GetConfigurationDoc(path);
            var node = GetNodeByName(doc, typeof(T).Name);
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, toSerialize, EmptyNamespace);
                var settings = XElement.Parse(OutputEncoding.GetString(stream.ToArray()), LoadOptions.SetBaseUri);

                if (node is object)
                {
                    node.ReplaceWith(settings);
                }
                else
                {
                    GetNodeByName(doc, RootElement).Add(settings);
                }
            }

            using (var xml = XmlWriter.Create(path, OutputXmlSettings))
            {
                doc.WriteTo(xml);
            }
        }
    }
}