using Resx = Rubberduck.Resources.v3.RubberduckMessages;

namespace Rubberduck.UI.Shared.Message
{
    public class MessageAction
    {
        public static MessageAction Undefined { get; } = new MessageAction();

        public static MessageAction AcceptAction { get; } = new MessageAction(nameof(Resx.MessageActionButton_Accept), isDefaultAction: true);
        public static MessageAction AcceptConfirmAction { get; } = new MessageAction(nameof(Resx.MessageActionButton_Confirm), isDefaultAction: true);
        public static MessageAction AcceptYesAction { get; } = new MessageAction(nameof(Resx.MessageActionButton_Yes), isDefaultAction: true);
        public static MessageAction DeclineNoAction { get; } = new MessageAction(nameof(Resx.MessageActionButton_No));
        public static MessageAction DefaultCancelAction { get; } = new MessageAction(nameof(Resx.MessageActionButton_Cancel), nameof(Resx.MessageActionButton_Cancel), isDefaultAction: true);
        public static MessageAction CancelAction { get; } = new MessageAction(nameof(Resx.MessageActionButton_Cancel), nameof(Resx.MessageActionButton_Cancel));
        public static MessageAction CloseAction { get; } = new MessageAction(nameof(Resx.MessageActionButton_Close), nameof(Resx.MessageActionButton_Close), isDefaultAction: true);

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

        public string Text => Resx.ResourceManager.GetString(ResourceKey) ?? $"[MissingKey:{ResourceKey}]";
        public string? ToolTip => ToolTipKey is not null ? Resx.ResourceManager.GetString(ToolTipKey) ?? $"[MissingKey:{ToolTipKey}]" : null;
    }
}
