using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OODGame.Networking.Protocol
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProtocolMessageType
    {
        InitialState,
        PlayerAction,
        StateUpdate,
        Error,
        Ack
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PlayerActionType
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        Interact,
        OpenInventory,
        ShowLog,
        Quit
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum StateUpdateType
    {
        FullState,
        PlayerJoined,
        PlayerMoved,
        PlayerActionRejected,
        EnemyMoved,
        EnemyDefeated,
        SoundEvent,
        Attack,
        PlayerLeft
    }

    public sealed class MessageEnvelope
    {
        [JsonPropertyName("type")]
        public ProtocolMessageType Type { get; set; }

        [JsonPropertyName("playerId")]
        public int? PlayerId { get; set; }

        [JsonPropertyName("timestampUtc")]
        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("payload")]
        public JsonElement Payload { get; set; }
    }

    public sealed class InitialStatePayload
    {
        [JsonPropertyName("state")]
        public GameStateDto State { get; set; } = new GameStateDto();
    }

    public sealed class PlayerActionPayload
    {
        [JsonPropertyName("action")]
        public PlayerActionType Action { get; set; }
    }

    public sealed class StateUpdatePayload
    {
        [JsonPropertyName("updateType")]
        public StateUpdateType UpdateType { get; set; }

        [JsonPropertyName("state")]
        public GameStateDto? State { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }

    public sealed class ErrorPayload
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }

    public sealed class AckPayload
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }

    public sealed class GameStateDto
    {
        [JsonPropertyName("currentMapX")]
        public int CurrentMapX { get; set; }

        [JsonPropertyName("currentMapY")]
        public int CurrentMapY { get; set; }

        [JsonPropertyName("currentRoomRows")]
        public List<string> CurrentRoomRows { get; set; } = new List<string>();

        [JsonPropertyName("players")]
        public List<PlayerStateDto> Players { get; set; } = new List<PlayerStateDto>();
    }

    public sealed class PlayerStateDto
    {
        [JsonPropertyName("playerId")]
        public int PlayerId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("x")]
        public int X { get; set; }

        [JsonPropertyName("y")]
        public int Y { get; set; }

        [JsonPropertyName("health")]
        public int Health { get; set; }

        [JsonPropertyName("maxHealth")]
        public int MaxHealth { get; set; }
    }
}
