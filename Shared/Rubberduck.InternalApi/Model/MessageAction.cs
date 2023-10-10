using Rubberduck.Resources;
using System;

namespace Rubberduck.InternalApi.Model
{
    public class MessageAction : IEquatable<MessageAction>
    {
        public static MessageAction AcceptAction { get; } = new MessageAction(nameof(RubberduckUI.OK), nameof(RubberduckUI.OK), true);
        public static MessageAction CancelAction { get; } = new MessageAction(nameof(RubberduckUI.CancelButtonText), nameof(RubberduckUI.CancelButtonText));
        public static MessageAction CloseAction { get; } = new MessageAction(nameof(RubberduckUI.CloseButtonText), nameof(RubberduckUI.CloseButtonText), true);

        private MessageAction(string key, string? tooltipKey = default, bool isDefaultAction = false) 
        {
            ResourceKey = key;
            ToolTipKey = tooltipKey;

            IsDefaultAction = isDefaultAction;
        }

        public string ResourceKey { get; init; }
        public string? ToolTipKey { get; init; }

        public bool IsDefaultAction { get; init; }

        public string Text => RubberduckUI.ResourceManager.GetString(ResourceKey) ?? $"[MissingKey:{ResourceKey}]";
        public string? ToolTip => (ToolTip is not null) ? (RubberduckUI.ResourceManager.GetString(ToolTip) ?? $"[MissingKey:{ToolTipKey}]") : null;

        public override bool Equals(object? obj)
        {
            if (obj is null || obj.GetType() != GetType())
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return Equals(obj as MessageAction);
        }

        public bool Equals(MessageAction? other)
        {
            if (other is null)
            {
                return false;
            }

            return other.ResourceKey.Equals(this.ResourceKey);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.ResourceKey);
        }
    }
}
