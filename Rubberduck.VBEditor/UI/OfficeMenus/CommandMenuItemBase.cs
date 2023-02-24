using Rubberduck.Resources.Menus;
using Rubberduck.UI.Command;
using Rubberduck.VBEditor.SafeComWrappers.Office;
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
        }

        public IMenuCommand Command { get; }

        public abstract string Key { get; }

        public virtual Func<string> Caption
        {
            get
            {
                return () => string.IsNullOrEmpty(Key)
                    ? string.Empty
                    : RubberduckMenus.ResourceManager.GetString(Key, CultureInfo.CurrentUICulture);
            }
        }

        public virtual string ToolTipKey { get; set; }
        public virtual Func<string> ToolTipText
        {
            get
            {
                return () => string.IsNullOrEmpty(ToolTipKey)
                    ? string.Empty
                    : RubberduckMenus.ResourceManager.GetString(ToolTipKey, CultureInfo.CurrentUICulture);
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
        public virtual bool BeginGroup => false;
        public virtual int DisplayOrder => default;
        public virtual Image Image => null;
        public virtual Image Mask => null;
    }
}
