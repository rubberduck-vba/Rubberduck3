using System.Collections.Generic;
using IOException = System.IO.IOException;
using Path = System.IO.Path;
using System.Text.RegularExpressions;
using VB = Microsoft.Vbe.Interop;
using System;
using Rubberduck.Unmanaged;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB.Enums;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB;

// ReSharper disable once CheckNamespace - Special dispensation due to conflicting file vs namespace priorities
namespace Rubberduck.VBEditor.SafeComWrappers.VBA
{
    public class VBProject : SafeComWrapper<VB.VBProject>, IVBProject
    {
        public VBProject(VB.VBProject target, bool rewrapping = false)
            :base(target, rewrapping)
        {
        }

        public IApplication Application => new Application((IsWrappingNullReference ? null : Target.Application)!);

        public IApplication Parent => new Application((IsWrappingNullReference ? null : Target.Parent)!);

        public Uri? Uri => IsSaved ? new Uri(new System.IO.DirectoryInfo(FileName).Parent!.FullName) : null;

        public string HelpFile
        {
            get => IsWrappingNullReference ? string.Empty : Target.HelpFile;
            set { if (!IsWrappingNullReference) Target.HelpFile = value; }
        }

        public string Description 
        {
            get => IsWrappingNullReference ? string.Empty : Target.Description;
            set { if (!IsWrappingNullReference) Target.Description = value; } 
        }

        public string Name
        {
            get => IsWrappingNullReference ? string.Empty : Target.Name;
            set { if (!IsWrappingNullReference) Target.Name = value; }
        }

        public EnvironmentMode Mode => IsWrappingNullReference ? 0 : (EnvironmentMode)Target.Mode;

        public IVBProjects Collection => new VBProjects((IsWrappingNullReference ? null : Target.Collection)!);

        public IReferences References => new References((IsWrappingNullReference ? null : Target.References)!);

        public IVBComponents VBComponents => new VBComponents((IsWrappingNullReference || Protection == ProjectProtection.Locked ? null : Target.VBComponents)!);

        public ProjectProtection Protection => IsWrappingNullReference ? 0 : (ProjectProtection)Target.Protection;

        public bool IsSaved => !IsWrappingNullReference && Target.Saved;

        public ProjectType Type => IsWrappingNullReference ? 0 : (ProjectType)Target.Type;

        public string FileName
        {
            get
            {
                try
                {
                    return IsWrappingNullReference ? string.Empty : Target.FileName;
                }
                catch (IOException)
                {
                    // thrown by the VBIDE API when wrapped VBProject has no filename yet.
                    return string.Empty;
                }
            }
        }

        public string BuildFileName => IsWrappingNullReference ? string.Empty : Target.BuildFileName;

        public IVBE VBE => new VBE((IsWrappingNullReference ? null : Target.VBE)!);

        public void SaveAs(string fileName)
        {
            if (!IsWrappingNullReference) Target.SaveAs(fileName);
        }

        public void MakeCompiledFile()
        {
            if (!IsWrappingNullReference) Target.MakeCompiledFile();
        }

        public override bool Equals(ISafeComWrapper<VB.VBProject> other)
        {
            return IsEqualIfNull(other) || (other != null && other.Target == Target);
        }

        public bool Equals(IVBProject? other)
        {
            return Equals((other as SafeComWrapper<VB.VBProject>)!);
        }

        public override int GetHashCode()
        {
            return IsWrappingNullReference ? 0 
                : HashCode.Combine(Target);
        }

        public IReadOnlyList<string> ComponentNames()
        {
            var names = new List<string>();
            using var components = VBComponents;
            foreach (var component in components)
            {
                using (component)
                {
                    names.Add(component.Name);
                }
            }

            return names.ToArray();
        }

        /// <summary>
        /// Exports all code modules in the VbProject to a destination directory. Files are given the same name as their parent code Module name and file extensions are based on what type of code Module it is.
        /// </summary>
        /// <param name="folder">The destination directory path.</param>
        public void ExportSourceFiles(string folder)
        {
            foreach (var component in VBComponents)
            {
                component.ExportAsSourceFile(folder);
            }
        }

        private string? _displayName;
        /// <summary>
        /// WARNING: This property might have has side effects. If the filename cannot be accessed, it changes the ActiveVBProject, which causes a flicker in the VBE.
        /// This should only be called if it is *absolutely* necessary.
        /// </summary>
        public string ProjectDisplayName
        {
            get
            {
                if (_displayName != null)
                {
                    return _displayName;
                }

                if (IsWrappingNullReference)
                {
                    _displayName = string.Empty;
                    return _displayName;
                }

                _displayName = DisplayNameFromFileName();

                if (string.IsNullOrEmpty(_displayName))
                {
                    _displayName = DisplayNameFromWindowCaption();
                }

                if (string.IsNullOrEmpty(_displayName)
                    || _displayName.EndsWith("..."))
                {
                    var nameFromBuildFileName = DisplayNameFromBuildFileName();
                    if (!string.IsNullOrEmpty(nameFromBuildFileName) 
                        && nameFromBuildFileName.Length > _displayName.Length - 3) //Otherwise, we got more of the name from the previous attempt.
                    {
                        _displayName = nameFromBuildFileName;
                    }
                }

                return _displayName;
            }
        }

        private string DisplayNameFromFileName()
        {
            return Path.GetFileName(FileName);
        }

        private string DisplayNameFromBuildFileName()
        {
            var pseudoDllName = Path.GetFileName(BuildFileName);
            return pseudoDllName == null || pseudoDllName.Length <= 4 //Should not happen as the string should always end in .DLL.
                ? string.Empty
                : pseudoDllName[..^4];
        }

        private static readonly Regex CaptionProjectRegex = new(@"^(?:[^-]+)(?:\s-\s)(?<project>.+)(?:\s-\s.*)?$");
        private static readonly Regex OpenModuleRegex = new(@"^(?<project>.+)(?<module>\s-\s\[.*\((Code|UserForm)\)\])$");
        private static readonly Regex PartialOpenModuleRegex = new(@"^(?<project>.+)(\s-\s\[)");
        private static readonly Regex NearlyOnlyProject = new(@"^(?<project>.+)(\s-?\s?)$");

        private string DisplayNameFromWindowCaption()
        {
            using var vbe = VBE;
            using var activeProject = vbe.ActiveVBProject;
            using var mainWindow = vbe.MainWindow;
            try
            {
                if (Uri != activeProject.Uri)
                {
                    vbe.ActiveVBProject = this;
                }

                var caption = mainWindow.Caption;
                if (caption.Length > 99)
                {
                    //The value returned will be truncated at character 99 and the rest is garbage due to a bug in the VBE API.
                    caption = caption[..99];

                    if (CaptionProjectRegex.IsMatch(caption))
                    {
                        var projectRelatedPartOfCaption = CaptionProjectRegex
                            .Matches(caption)[0]
                            .Groups["project"]
                            .Value;

                        if (PartialOpenModuleRegex.IsMatch(projectRelatedPartOfCaption))
                        {
                            return PartialOpenModuleRegex
                                .Matches(projectRelatedPartOfCaption)[0]
                                .Groups["project"]
                                .Value;
                        }

                        if (NearlyOnlyProject.IsMatch(projectRelatedPartOfCaption))
                        {
                            return NearlyOnlyProject
                                .Matches(projectRelatedPartOfCaption)[0]
                                .Groups["project"]
                                .Value;
                        }

                        return $"{projectRelatedPartOfCaption}...";
                    }
                }
                else
                {
                    if (CaptionProjectRegex.IsMatch(caption))
                    {
                        var projectRelatedPartOfCaption = CaptionProjectRegex
                            .Matches(caption)[0]
                            .Groups["project"]
                            .Value;

                        if (OpenModuleRegex.IsMatch(projectRelatedPartOfCaption))
                        {
                            return OpenModuleRegex
                                .Matches(projectRelatedPartOfCaption)[0]
                                .Groups["project"]
                                .Value;
                        }
                    }
                }
            }
            catch
            {
                return string.Empty;
            }

            return string.Empty;
        }

        protected override void Dispose(bool disposing) => base.Dispose(disposing);
    }
}
