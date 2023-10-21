using Microsoft.Extensions.Logging;
using Rubberduck.Editor.Command;
using Rubberduck.Editor.Message;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using System;

namespace Rubberduck.Editor.FileMenu
{
    public class MessageActionsProvider
    {
        private readonly ILogger _logger;
        private readonly ISettingsProvider<RubberduckSettings> _settings;

        public MessageActionsProvider(ILogger logger, ISettingsProvider<RubberduckSettings> settings)
        {
            _logger = logger;
            _settings = settings;
        }

        public MessageActionCommand[] Close() =>
            new MessageActionCommand[]
            {
                new CloseMessageActionCommand(_logger, _settings, MessageAction.CloseAction),
            };

        public MessageActionCommand[] OkOnly(Func<object?, bool>? validations = null) =>
            new MessageActionCommand[]
            {
                new AcceptMessageActionCommand(_logger, _settings, MessageAction.AcceptAction, validations),
            };

        public MessageActionCommand[] OkCancel(Func<object?, bool>? validations = null) =>
            new MessageActionCommand[]
            {
                new AcceptMessageActionCommand(_logger, _settings, MessageAction.AcceptAction, validations),
                new CancelMessageActionCommand(_logger, _settings, MessageAction.CancelAction)
            };
    }
}
