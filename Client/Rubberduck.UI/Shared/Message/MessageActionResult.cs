namespace Rubberduck.UI.Shared.Message
{
    public record class MessageActionResult
    {
        public static MessageActionResult Default { get; } = new MessageActionResult
        {
            IsEnabled = true,
            MessageAction = MessageAction.Undefined,
        };

        public static MessageActionResult Disabled { get; } = new MessageActionResult
        {
            IsEnabled = false,
            MessageAction = MessageAction.Undefined,
        };

        /// <summary>
        /// Represents the action (button) selected by the user.
        /// </summary>
        public MessageAction MessageAction { get; init; } = MessageAction.Undefined;

        /// <summary>
        /// <c>false</c> if the user has checked a <em>do not show this message again</em> checkbox.
        /// </summary>
        public bool IsEnabled { get; init; } = true;
    }
}
