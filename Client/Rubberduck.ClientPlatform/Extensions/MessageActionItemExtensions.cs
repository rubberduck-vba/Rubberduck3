using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Model;
using Rubberduck.UI.Message;
using System;
using System.Linq;

namespace Rubberduck.ClientPlatform.Extensions
{
    public static class MessageActionItemExtensions
    {
        public static MessageActionItem ToMessageActionItem(this MessageActionResult result, ShowMessageRequestParams request)
        {
            return request.Actions?.SingleOrDefault(e => e.Title == result.MessageAction.ResourceKey)
                ?? throw new ArgumentOutOfRangeException(nameof(result), "The specified action result does not correspond to any action items from the specified request.");
        }

        public static MessageAction ToMessageAction(this MessageActionItem item)
        {
            return new MessageAction(item.Title,
                        item.ExtensionData.TryGetValue(nameof(MessageAction.ToolTipKey), out var tooltipKeyToken) ? tooltipKeyToken.ToString() : null,
                        item.ExtensionData.TryGetValue(nameof(MessageAction.IsDefaultAction), out var isDefaultActionToken) && isDefaultActionToken.Values<bool>().Single()
                    );
        }
    }
}
