using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.Unmanaged;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.Office;
using System;
using System.Collections.Generic;

namespace Rubberduck.VBEditor.UI.OfficeMenus
{
    public class CommandBarMenuBuilder<TMenu> where TMenu : IParentMenuItem
    {
        public class DuplicateMenuItemException : InvalidOperationException { }

        private readonly CommandBarLocation _location;
        private readonly IServiceProvider _services;
        private readonly ICommandBarControls _parent;

        private readonly Queue<IMenuItem> _items = new();
        private readonly bool _beginGroup;

        private bool _withSeparator = false;

        public CommandBarMenuBuilder(CommandBarLocation location, IServiceProvider provider, ICommandBarControls parent, bool beginGroup = false)
        {
            _location = location;
            _services = provider;
            _parent = parent;
            _beginGroup = beginGroup;
        }

        public CommandBarMenuBuilder<TMenu> WithCommandMenuItem<TItem>() where TItem : ICommandMenuItem
        {
            WithMenuItem<TItem>();
            return this;
        }

        public CommandBarMenuBuilder<TMenu> WithSubMenu<TItem>() where TItem : IParentMenuItem
        {
            WithMenuItem<TItem>();
            return this;
        }

        private void WithMenuItem<TItem>() where TItem : IMenuItem
        {
            var item = _services.GetRequiredService<TItem>();
            item.DisplayOrder = _items.Count;
            item.BeginGroup = _withSeparator;
            _withSeparator = false;

            _items.Enqueue(item);
        }

        public CommandBarMenuBuilder<TMenu> WithSeparator()
        {
            _withSeparator = true;
            return this;
        }

        public TMenu Build()
        {
            var menu = _services.GetRequiredService<TMenu>();
            menu.Parent = _parent;
            menu.BeforeIndex = FindInsertionIndex(_parent, _location.BeforeControlId);
            menu.BeginGroup = _beginGroup;

            while (_items.Count != 0)
            {
                menu.AddChildItem(_items.Dequeue());
            }

            return menu;
        }

        private int FindInsertionIndex(ICommandBarControls controls, int beforeId)
        {
            for (var index = 1; index <= controls.Count; index++)
            {
                using var item = controls[index];
                if (item.IsBuiltIn && item.Id == beforeId)
                {
                    return index;
                }
            }

            var logger = _services.GetRequiredService<ILogger<CommandBarMenuBuilder<TMenu>>>();
            logger.LogWarning("Could not find menu item id {beforeId}. Menu item will be appended to parent controls.", beforeId);
            return controls.Count;
        }
    }
}
