using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Services;
using System;

namespace Rubberduck.UI.Command
{
    public class MessageActionsProvider
    {
        private readonly ServiceHelper _service;

        public MessageActionsProvider(ServiceHelper service)
        {
            _service = service;
        }

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
