using Rubberduck.Resources.Menus;
using Rubberduck.UI.Command;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.Office;
using System;
using System.Drawing;
using System.Globalization;

namespace Rubberduck.VBEditor.UI.OfficeMenus
{
    public abstract class CommandMenuItemBase : ICommandMenuItem
    {
        protected CommandMenuItemBase(IMenuCommand command)
        {
            Command = command;
            ToolTipKey = string.Empty;
        }

        public IMenuCommand Command { get; }

        public abstract string ResourceKey { get; }

        public virtual Func<string> Caption
        {
            get
            {
                return () => string.IsNullOrEmpty(ResourceKey)
                    ? string.Empty
                    : RubberduckMenus.ResourceManager.GetString(ResourceKey, CultureInfo.CurrentUICulture) ?? $"[{ResourceKey}]";
            }
        }

        public virtual string ToolTipKey { get; set; }
        public virtual Func<string> ToolTipText
        {
            get
            {
                return () => string.IsNullOrEmpty(ToolTipKey)
                    ? string.Empty
                    : RubberduckMenus.ResourceManager.GetString(ToolTipKey, CultureInfo.CurrentUICulture) ?? $"[{ToolTipKey}]";
            }
        }

        /// <summary>
        /// Evaluates whether the associated command can be executed.
        /// </summary>
        /// <returns>Returns <c>true</c> if command can be executed.</returns>
        /// <remarks>Returns <c>true</c> unless overridden.</remarks>
        public virtual bool EvaluateCanExecute(object parameter)
        {
            return true;
        }

        public virtual ButtonStyle ButtonStyle => ButtonStyle.IconAndCaption;
        public virtual bool HiddenWhenDisabled => false;
        public virtual bool IsVisible => true;
        public virtual bool BeginGroup { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual Image Image => null!;
        public virtual Image Mask => null!;
    }
}
