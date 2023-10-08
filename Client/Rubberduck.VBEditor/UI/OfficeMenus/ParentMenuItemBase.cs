using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Rubberduck.Resources.Menus;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.Office;
using Rubberduck.Unmanaged.UIContext;

namespace Rubberduck.VBEditor.UI.OfficeMenus
{
    public abstract class ParentMenuItemBase : IParentMenuItem
    {
        private readonly IDictionary<IMenuItem, ICommandBarControl?> _items = new Dictionary<IMenuItem, ICommandBarControl?>();
        //private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected readonly IUiDispatcher _uiDispatcher;

        protected ParentMenuItemBase(IUiDispatcher dispatcher)
        {
            _uiDispatcher = dispatcher;
        }

        public void AddChildItem(IMenuItem item)
        {
            AddItem(item, null);
        }

        public void AddItem(IMenuItem item, ICommandBarControl? msoControl)
        {
            _items.Add(item, msoControl);
        }

        public abstract string ResourceKey { get; }
        public virtual int? BeforeIndex { get; set; }

        private ICommandBarControls? _parent;
        public ICommandBarControls? Parent
        {
            get => _parent;
            set
            {
                _parent?.Dispose();
                _parent = value;
            }
        }

        private ICommandBarPopup? _item;
        public ICommandBarPopup? Item
        {
            get => _item;
            private set
            {
                _item?.Dispose();
                _item = value;
            }
        }

        public Func<string> Caption { get { return () => RubberduckMenus.ResourceManager.GetString(ResourceKey, CultureInfo.CurrentUICulture) ?? ResourceKey; } }

        public virtual string ToolTipKey { get; set; }
        public virtual Func<string> ToolTipText
        {
            get
            {
                return () => string.IsNullOrEmpty(ToolTipKey)
                    ? string.Empty
                    : RubberduckMenus.ResourceManager.GetString(ToolTipKey, CultureInfo.CurrentUICulture) ?? ToolTipKey;
            }
        }

        public virtual bool BeginGroup { get; set; }
        public virtual int DisplayOrder { get; set; }

        public void Localize()
        {
            if (Item is null)
            {
                return;
            }
            
            Item.Caption = Caption.Invoke();
            foreach (var kvp in _items)
            {
                if (kvp.Value is null)
                {
                    continue;
                }

                kvp.Value.Caption = kvp.Key.Caption.Invoke();
                if (kvp.Key is CommandMenuItemBase command)
                {
                    ((ICommandBarButton)kvp.Value).ShortcutText = command.Command.ShortcutText;
                }

                var childMenu = kvp.Key as ParentMenuItemBase;
                childMenu?.Localize();
            }
        }

        public void Initialize()
        {
            if (Parent == null)
            {
                return;
            }

            Item =  Parent.AddPopup(BeforeIndex);

            Item.Tag = $"RD3.{ResourceKey}";

            foreach (var item in _items.Keys.OrderBy(item => item.DisplayOrder))
            {
                _items[item] = InitializeChildControl(item as ICommandMenuItem)
                            ?? InitializeChildControl(item as IParentMenuItem);
            }

            EvaluateCanExecuteAsync(null, CancellationToken.None).Wait();
        }

        public void RemoveMenu()
        {
            RemoveChildren();
            Item?.Delete();

            //This will also dispose the Item as well
            Item = null;
        }

        private void RemoveChildren()
        {
            foreach (var child in _items.Keys.Select(item => item as IParentMenuItem).Where(child => child != null))
            {
                child.RemoveMenu();
                child.Parent.Dispose();
            }
            foreach (var child in _items.Values.Select(item => item as ICommandBarButton).Where(child => child != null))
            {
                child.Click -= ChildMenuCommandClick;
                child.Delete();
                child.Dispose();
            }
        }

        public async Task EvaluateCanExecuteAsync(object parameter, CancellationToken token)
        {
            foreach (var (key, value) in _items)
            {
                switch (key)
                {
                    case IParentMenuItem parentItem:
                        await parentItem.EvaluateCanExecuteAsync(parameter, token);
                        continue;
                    case ICommandMenuItem commandItem when value != null:
                        _uiDispatcher.InvokeAsync(() =>
                        {
                            try
                            {
                                value.IsEnabled = commandItem.EvaluateCanExecute(parameter);
                            }
                            catch (Exception)
                            {
                                value.IsEnabled = false;
                                //Logger.Error(exception, "Could not evaluate availability of commmand menu item {0}.", value.Tag ?? "{Unknown}");
                            }
                        });
                        break;
                }
            }
        }

        private ICommandBarControl InitializeChildControl(IParentMenuItem item)
        {
            if (item == null)
            {
                return null;
            }

            item.Parent = Item.Controls;
            item.Initialize();
            return item.Item;
        }

        private ICommandBarControl InitializeChildControl(ICommandMenuItem item)
        {
            if (item == null)
            {
                return null;
            }

            ICommandBarButton child;
            using (var controls = Item.Controls)
            {
                child = controls.AddButton();                
            }
            child.Picture = item.Image;
            child.Mask = item.Mask;
            child.ApplyIcon();

            child.BeginsGroup = item.BeginGroup;
            using (var itemParent = Item.Parent)
            {
                child.Tag = $"{itemParent.Name}::{Item.Tag}::{item.GetType().Name}";
            }
            child.Caption = item.Caption.Invoke();
            var command = item.Command; // todo: add 'ShortcutText' to a new 'interface CommandBase : System.Windows.Input.CommandBase'
            child.ShortcutText = command != null
                ? command.ShortcutText
                : string.Empty;

            child.Click += ChildMenuCommandClick!;
            return child;
        }

        private void ChildMenuCommandClick(object sender, CommandBarButtonClickEventArgs e)
        {
            if (!(_items.Select(kvp => kvp.Key).SingleOrDefault(menu => e.Tag.EndsWith(menu.GetType().Name)) is ICommandMenuItem item))
            {
                return;
            }

            //Logger.Debug("({0}) Executing click handler for menu item '{1}', hash code {2}", GetHashCode(), e.Caption, e.TargetHashCode);
            item.Command.Execute(null);
        }
    }
}
