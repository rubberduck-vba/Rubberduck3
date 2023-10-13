using Rubberduck.Resources;
using System;

namespace Rubberduck.InternalApi.Model
{
    public readonly record struct MessageAction : IEquatable<MessageAction>
    {
        public static MessageAction Undefined { get; } = new MessageAction();

        public static MessageAction AcceptAction { get; } = new MessageAction(nameof(RubberduckUI.OK), nameof(RubberduckUI.OK), true);
        public static MessageAction CancelAction { get; } = new MessageAction(nameof(RubberduckUI.CancelButtonText), nameof(RubberduckUI.CancelButtonText));
        public static MessageAction CloseAction { get; } = new MessageAction(nameof(RubberduckUI.CloseButtonText), nameof(RubberduckUI.CloseButtonText), true);

        public MessageAction() : this(string.Empty) { }
        public MessageAction(string key, string? tooltipKey = default, bool isDefaultAction = false) 
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
    }
}
