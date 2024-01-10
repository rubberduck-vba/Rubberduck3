using Microsoft.Extensions.Logging;
using Rubberduck.Resources;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Rubberduck.UI.Message
{
    public static class MessageKeys
    {
        public const string ConfirmOverwriteExistingWorkspace = "ConfirmOverwriteExistingWorkspace";

        public static string GetMessageResourceKey(string key) => $"Message_{key}";

        public static string GetTitleResourceKey(string key) => $"MessageTitle_{key}";
    }

    public class MessageRequestModel : MessageModel
    {
        public static MessageRequestModel For(LogLevel level, string message, MessageAction[] actions)
        {
            var model = FromShowMessageParams(message, level);
            return new()
            {
                Key = model.Key,
                Title = model.Title,
                Message = model.Message,
                Verbose = model.Verbose,
                Level = model.Level,

                MessageActions = actions,
            };
        }

        public MessageAction[] MessageActions { get; init; } = new[] { MessageAction.CloseAction };
    }

    public class MessageModel
    {
        protected static string GetMessageKey(string message) => Encoding.UTF8.GetString(SHA256.HashData(Encoding.UTF8.GetBytes(message)));

        protected static bool TryDeserialize(string message, out MessageModel? model)
        {
            try
            {
                model = JsonSerializer.Deserialize<MessageModel>(message, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return true;
            }
            catch
            {
                model = null;
                return false;
            }
        }

        public static MessageModel FromShowMessageParams(string message, LogLevel level)
        {
            if (TryDeserialize(message, out var model) && model is not null)
            {
                if (string.IsNullOrWhiteSpace(model.Key))
                {
                    return new()
                    {
                        Key = model.Key,
                        Title = model.Title,
                        Message = model.Message,
                        Verbose = model.Verbose,
                        Level = level
                    };
                }
                return model;
            }
            else
            {
                return new()
                {
                    Key = GetMessageKey(message),
                    Title = RubberduckUI.Rubberduck,
                    Message = message,
                    Verbose = string.Empty,
                    Level = level
                };
            }
        }

        public static MessageModel For(LogLevel level, string key, string message, string? verbose = default)
        {
            return new()
            {
                Key = key,
                Title = RubberduckUI.Rubberduck,
                Message = message,
                Verbose = verbose ?? string.Empty,
                Level = level
            };
        }

        public static MessageModel For(Exception exception)
        {
            return new()
            {
                Key = exception.TargetSite?.Name ?? exception.GetType().Name,
                Title = exception.GetType().Name,
                Message = exception.Message,
                Verbose = exception.StackTrace ?? string.Empty,
                Level = LogLevel.Error
            };
        }

        public string Key { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
        public string Verbose { get; init; } = string.Empty;
        public string Title { get; init; } = string.Empty;

        public LogLevel Level { get; init; } = LogLevel.Information;
    }
}
