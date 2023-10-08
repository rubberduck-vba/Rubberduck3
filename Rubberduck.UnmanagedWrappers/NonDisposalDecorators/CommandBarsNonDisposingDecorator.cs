using System.Collections;
using System.Collections.Generic;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.Office;

namespace Rubberduck.Unmanaged.NonDisposingDecorators
{
    public class CommandBarsNonDisposingDecorator<T> : NonDisposingDecoratorBase<T>, ICommandBars
        where T : ICommandBars
    {
        public CommandBarsNonDisposingDecorator(T commandBars)
            : base(commandBars)
        { }

        public IEnumerator<ICommandBar> GetEnumerator()
        {
            return WrappedItem.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) WrappedItem).GetEnumerator();
        }

        public int Count => WrappedItem.Count;

        public ICommandBar this[object index] => WrappedItem[index];

        public ICommandBar Add(string name)
        {
            return WrappedItem.Add(name);
        }

        public ICommandBar Add(string name, CommandBarPosition position)
        {
            return WrappedItem.Add(name, position);
        }

        public ICommandBarControl FindControl(int id)
        {
            return WrappedItem.FindControl(id);
        }

        public ICommandBarControl FindControl(ControlType type, int id)
        {
            return WrappedItem.FindControl(type, id);
        }
    }
}