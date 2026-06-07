using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OODGame.Networking.Protocol
{
    public static class ProtocolJson
    {
        public static JsonSerializerOptions Options { get; } = CreateOptions();

        public static MessageEnvelope CreateEnvelope<TPayload>(ProtocolMessageType type, int? playerId, TPayload payload)
        {
            return new MessageEnvelope
            {
                Type = type,
                PlayerId = playerId,
                TimestampUtc = DateTime.UtcNow,
                Payload = JsonSerializer.SerializeToElement(payload, Options)
            };
        }

        public static bool TryGetPayload<TPayload>(MessageEnvelope envelope, out TPayload? payload)
        {
            payload = default;
            try
            {
                payload = envelope.Payload.Deserialize<TPayload>(Options);
                return payload is not null;
            }
            catch
            {
                return false;
            }
        }

        public static string Serialize(MessageEnvelope envelope)
            => JsonSerializer.Serialize(envelope, Options);

        public static MessageEnvelope? Deserialize(string json)
            => JsonSerializer.Deserialize<MessageEnvelope>(json, Options);

        private static JsonSerializerOptions CreateOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }
    }
}
