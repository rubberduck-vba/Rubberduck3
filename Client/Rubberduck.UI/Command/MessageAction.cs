using Rubberduck.Resources.Messages;

namespace Rubberduck.UI.Command
{
    public class MessageAction
    {
        public static MessageAction Undefined { get; } = new MessageAction();

        public static MessageAction AcceptAction { get; } = new MessageAction(nameof(RubberduckMessages.MessageActionButton_Accept), isDefaultAction: true);
        public static MessageAction AcceptConfirmAction { get; } = new MessageAction(nameof(RubberduckMessages.MessageActionButton_Confirm), isDefaultAction: true);
        public static MessageAction AcceptYesAction { get; } = new MessageAction(nameof(RubberduckMessages.MessageActionButton_Yes), isDefaultAction: true);
        public static MessageAction DeclineNoAction { get; } = new MessageAction(nameof(RubberduckMessages.MessageActionButton_No));
        public static MessageAction CancelAction { get; } = new MessageAction(nameof(RubberduckMessages.MessageActionButton_Cancel), nameof(RubberduckMessages.MessageActionButton_Cancel));
        public static MessageAction CloseAction { get; } = new MessageAction(nameof(RubberduckMessages.MessageActionButton_Close), nameof(RubberduckMessages.MessageActionButton_Close), isDefaultAction: true);

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

        public string Text => RubberduckMessages.ResourceManager.GetString(ResourceKey) ?? $"[MissingKey:{ResourceKey}]";
        public string? ToolTip => ToolTipKey is not null ? RubberduckMessages.ResourceManager.GetString(ToolTipKey) ?? $"[MissingKey:{ToolTipKey}]" : null;
    }
}
