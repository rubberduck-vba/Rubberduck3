using Rubberduck.Unmanaged.Abstract.SafeComWrappers;
using System.Collections.Generic;

namespace Rubberduck.Unmanaged.NonDisposingDecorators
{
    public class AddInNonDisposingDecorator<T> : NonDisposingDecoratorBase<T>, IAddIn
        where T : IAddIn
    {
        public AddInNonDisposingDecorator(T addIn)
            : base(addIn)
        { }

        public bool Equals(IAddIn other)
        {
            return WrappedItem.Equals(other);
        }

        public string ProgId => WrappedItem.ProgId;

        public string Guid => WrappedItem.Guid;

        public string Description
        {
            get => WrappedItem.Description;
            set => WrappedItem.Description = value;
        }

        public bool Connect
        {
            get => WrappedItem.Connect;
            set => WrappedItem.Connect = value;
        }

        public object Object
        {
            get => WrappedItem.Object;
            set => WrappedItem.Object = value;
        }

        public IVBE VBE => WrappedItem.VBE;

        public IAddIns Collection => WrappedItem.Collection;

        public IReadOnlyDictionary<CommandBarSite, CommandBarLocation> CommandBarLocations => WrappedItem.CommandBarLocations;
    }
}