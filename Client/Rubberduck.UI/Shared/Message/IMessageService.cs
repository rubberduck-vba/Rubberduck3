using Microsoft.Extensions.Logging;
using Rubberduck.UI.Command.SharedHandlers;
using System;

namespace Rubberduck.UI.Shared.Message
{
    public interface IMessageService
    {
        /// <summary>
        /// Displays a message to the user, requesting an action.
        /// </summary>
        /// <returns>
        /// <c>MessageActionResult.Disabled</c> if the model key is disabled.
        /// </returns>
        MessageActionResult ShowMessageRequest(MessageRequestModel model, Func<MessageActionsProvider, MessageActionCommand[]>? actions = null);

        /// <summary>
        /// Displays a message to the user.
        /// </summary>
        /// <returns>
        /// <c>MessageActionResult.Disabled</c> if the model key is disabled.
        /// </returns>
        MessageActionResult ShowMessage(MessageModel model, Func<MessageActionsProvider, MessageActionCommand[]>? actions = null);

        /// <summary>
        /// Displays user-facing exception error message, including the stack trace.
        /// </summary>
        /// <remarks>
        /// If the specified key does not exist in <c>RubberduckUI</c> resource strings, the <c>LogLevel</c> is used as a title.
        /// </remarks>
        /// <param name="key">The resource key for the message. Used for tracking whether or not this message should be shown.</param>
        /// <param name="exception">The exception to display details about.</param>
        /// <param name="level">Specify a 'level' that the implementation may use to display a corresponding icon.</param>
        void ShowError(string key, Exception exception, LogLevel level = LogLevel.Error);
    }
}
