using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Services;
using System;
using System.Linq;

namespace Rubberduck.UI.Shared.Message
{
    public class MessageActionsProvider
    {
        private readonly UIServiceHelper _service;

        public MessageActionsProvider(UIServiceHelper service)
        {
            _service = service;
        }

        public MessageActionCommand FromMessageAction(MessageAction action, Func<object?, bool>? validations = null) =>
            action.IsDefaultAction
            ? new AcceptMessageActionCommand(_service, action, validations)
            : new CancelMessageActionCommand(_service, action);

        /// <summary>
        /// Gets a <c>MessageActionCommand[]</c> containing a <c>CloseMessageActionCommand</c>.
        /// </summary>
        public MessageActionCommand[] Close() =>
            new MessageActionCommand[]
            {
                new CloseMessageActionCommand(_service, MessageAction.CloseAction),
            };

        /// <summary>
        /// Gets a <c>MessageActionCommand[]</c> containing an <c>AcceptMessageActionCommand</c>.
        /// </summary>
        public MessageActionCommand[] OkOnly(Func<object?, bool>? validations = null) =>
            new MessageActionCommand[]
            {
                new AcceptMessageActionCommand(_service, MessageAction.AcceptAction, validations),
            };

        /// <summary>
        /// Gets a <c>MessageActionCommand[]</c> containing <c>AcceptMessageActionCommand</c> and <c>CancelMessageActionCommand</c>.
        /// </summary>
        public MessageActionCommand[] OkCancel(Func<object?, bool>? validations = null) =>
            new MessageActionCommand[]
            {
                new AcceptMessageActionCommand(_service, MessageAction.AcceptAction, validations),
                new CancelMessageActionCommand(_service, MessageAction.CancelAction)
            };
    }
}
