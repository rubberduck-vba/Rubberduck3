using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using System;

namespace Rubberduck.UI.Command
{
    public class MessageActionsProvider
    {
        private readonly ILogger _logger;
        private readonly ISettingsProvider<RubberduckSettings> _settings;

        public MessageActionsProvider(ILogger<MessageActionsProvider> logger, ISettingsProvider<RubberduckSettings> settings)
        {
            _logger = logger;
            _settings = settings;
        }

        /// <summary>
        /// Gets a <c>MessageActionCommand[]</c> containing a <c>CloseMessageActionCommand</c>.
        /// </summary>
        public MessageActionCommand[] Close() =>
            new MessageActionCommand[]
            {
                new CloseMessageActionCommand(_logger, _settings, MessageAction.CloseAction),
            };

        /// <summary>
        /// Gets a <c>MessageActionCommand[]</c> containing an <c>AcceptMessageActionCommand</c>.
        /// </summary>
        public MessageActionCommand[] OkOnly(Func<object?, bool>? validations = null) =>
            new MessageActionCommand[]
            {
                new AcceptMessageActionCommand(_logger, _settings, MessageAction.AcceptAction, validations),
            };

        /// <summary>
        /// Gets a <c>MessageActionCommand[]</c> containing <c>AcceptMessageActionCommand</c> and <c>CancelMessageActionCommand</c>.
        /// </summary>
        public MessageActionCommand[] OkCancel(Func<object?, bool>? validations = null) =>
            new MessageActionCommand[]
            {
                new AcceptMessageActionCommand(_logger, _settings, MessageAction.AcceptAction, validations),
                new CancelMessageActionCommand(_logger, _settings, MessageAction.CancelAction)
            };
    }
}
