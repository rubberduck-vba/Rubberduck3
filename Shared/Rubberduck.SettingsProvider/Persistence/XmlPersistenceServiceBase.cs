using System;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

// ReSharper disable StaticMemberInGenericType
namespace Rubberduck.SettingsProvider
{
    public abstract class XmlPersistenceServiceBase<T> : IAsyncPersistenceService<T> where T : class, IEquatable<T>, new()
    {
        protected readonly string RootPath;
        protected const string RootElement = "Configuration";

        protected static readonly XmlSerializerNamespaces EmptyNamespace = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
        protected static readonly UTF8Encoding OutputEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        
        protected static readonly XmlWriterSettings OutputXmlSettings = new XmlWriterSettings
        {
            NamespaceHandling = NamespaceHandling.OmitDuplicates,
            Encoding = OutputEncoding,
            Indent = true
        };

        protected readonly IFileSystem FileSystem;

        protected XmlPersistenceServiceBase(
            IPersistencePathProvider pathProvider,
            IFileSystem fileSystem)
        {
            RootPath = pathProvider.DataRootPath;
            FileSystem = fileSystem;
        }
        
        protected abstract string FilePath { get; }

        T IPersistenceService<T>.Load(string path)
        {
            return Read(string.IsNullOrEmpty(path) ? FilePath : path);
        }

        void IPersistenceService<T>.Save(T toSerialize, string path)
        {
            var targetPath = string.IsNullOrEmpty(path) ? FilePath : path;
            EnsureDirectoryExists(targetPath);
            Write(toSerialize, targetPath);
        }

        public async Task<T> LoadAsync()
        {
            return await ReadAsync(FilePath);
        }

        public async Task<T> LoadAsync(string path)
        {
            return await ReadAsync(path);
        }

        public async Task SaveAsync(T toSerialize)
        {
            EnsureDirectoryExists(FilePath);
            await WriteAsync(toSerialize, FilePath);
        }

        public async Task SaveAsync(T toSerialize, string path)
        {
            EnsureDirectoryExists(path);
            await WriteAsync(toSerialize, path);
        }

        protected virtual Task<T> ReadAsync(string path)
        {
            return Task.Run(() => Read(path));
        }

        protected abstract T Read(string path);

        protected async Task WriteAsync(T toSerialize, string path)
        {
            await Task.Run(() => Write(toSerialize, path));
        }

        protected abstract void Write(T toSerialize, string path);

        protected XDocument GetConfigurationDoc(string file)
        {
            XDocument output;
            if (FileSystem.File.Exists(file))
            {
                try
                {
                    output = XDocument.Load(file);
                    if (output.Descendants().FirstOrDefault(e => e.Name.LocalName.Equals(RootElement)) != null)
                    {
                        return output;
                    }
                }
                catch
                {
                    // this is fine - we'll just return an empty XDocument.
                }
            }

            output = new XDocument();
            output.Add(new XElement(RootElement));
            return output;
        }

        protected static XElement GetNodeByName(XContainer doc, string name)
        {
            return doc.Descendants().FirstOrDefault(e => e.Name.LocalName.Equals(name));
        }

        protected void EnsureDirectoryExists(string filePath)
        {
            var folder = FileSystem.Path.GetDirectoryName(filePath);
            if (folder != null && !FileSystem.Directory.Exists(folder))
            {
                FileSystem.Directory.CreateDirectory(folder);
            }
        }

        protected async Task EnsureDirectoryExistsAsync(string filePath)
        {
            await Task.Run(() =>
            {
                var folder = FileSystem.Path.GetDirectoryName(filePath);
                if (folder != null && !FileSystem.Directory.Exists(folder))
                {
                    FileSystem.Directory.CreateDirectory(folder);
                }
            });
        }
    }
}
