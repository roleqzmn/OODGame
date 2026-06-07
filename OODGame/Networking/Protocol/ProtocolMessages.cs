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
        PickupItem,
        EquipItem,
        Attack,
        Interact,
        OpenInventory,
        ShowLog,
        DropItem,
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
        [JsonPropertyName("assignedPlayerId")]
        public int AssignedPlayerId { get; set; }

        [JsonPropertyName("state")]
        public GameStateDto State { get; set; } = new GameStateDto();
    }

    public sealed class PlayerActionPayload
    {
        [JsonPropertyName("action")]
        public PlayerActionType Action { get; set; }

        [JsonPropertyName("itemIndex")]
        public int? ItemIndex { get; set; }

        [JsonPropertyName("preferredHand")]
        public string? PreferredHand { get; set; }
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

        [JsonPropertyName("enemies")]
        public List<EnemyStateDto> Enemies { get; set; } = new List<EnemyStateDto>();

        [JsonPropertyName("itemTiles")]
        public List<ItemTileStateDto> ItemTiles { get; set; } = new List<ItemTileStateDto>();
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

        [JsonPropertyName("inventoryCount")]
        public int InventoryCount { get; set; }

        [JsonPropertyName("currentLoad")]
        public int CurrentLoad { get; set; }

        [JsonPropertyName("inventoryLimit")]
        public int InventoryLimit { get; set; }

        [JsonPropertyName("strength")]
        public int Strength { get; set; }

        [JsonPropertyName("dexterity")]
        public int Dexterity { get; set; }

        [JsonPropertyName("luck")]
        public int Luck { get; set; }

        [JsonPropertyName("aggression")]
        public int Aggression { get; set; }

        [JsonPropertyName("wisdom")]
        public int Wisdom { get; set; }

        [JsonPropertyName("coins")]
        public int Coins { get; set; }

        [JsonPropertyName("gold")]
        public int Gold { get; set; }

        [JsonPropertyName("hasTwoHanded")]
        public bool HasTwoHanded { get; set; }

        [JsonPropertyName("leftHand")]
        public WeaponStateDto? LeftHand { get; set; }

        [JsonPropertyName("rightHand")]
        public WeaponStateDto? RightHand { get; set; }

        [JsonPropertyName("inventoryItems")]
        public List<InventoryItemDto> InventoryItems { get; set; } = new List<InventoryItemDto>();

        [JsonPropertyName("isInCombat")]
        public bool IsInCombat { get; set; }

        [JsonPropertyName("combat")]
        public CombatStateDto? Combat { get; set; }
    }

    public sealed class CombatStateDto
    {
        [JsonPropertyName("enemyName")]
        public string EnemyName { get; set; } = string.Empty;

        [JsonPropertyName("enemyHealth")]
        public int EnemyHealth { get; set; }

        [JsonPropertyName("enemyMaxHealth")]
        public int EnemyMaxHealth { get; set; }

        [JsonPropertyName("enemyArmor")]
        public int EnemyArmor { get; set; }

        [JsonPropertyName("enemyDamage")]
        public int EnemyDamage { get; set; }

        [JsonPropertyName("lastLog")]
        public string LastLog { get; set; } = string.Empty;

        [JsonPropertyName("actionNames")]
        public List<string> ActionNames { get; set; } = new List<string>();
    }

    public sealed class WeaponStateDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("damage")]
        public int Damage { get; set; }

        [JsonPropertyName("range")]
        public int Range { get; set; }
    }

    public sealed class InventoryItemDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("symbol")]
        public char Symbol { get; set; }

        [JsonPropertyName("weight")]
        public short Weight { get; set; }
    }

    public sealed class EnemyStateDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("species")]
        public string Species { get; set; } = string.Empty;

        [JsonPropertyName("x")]
        public int X { get; set; }

        [JsonPropertyName("y")]
        public int Y { get; set; }

        [JsonPropertyName("health")]
        public int Health { get; set; }

        [JsonPropertyName("maxHealth")]
        public int MaxHealth { get; set; }
    }

    public sealed class ItemTileStateDto
    {
        [JsonPropertyName("x")]
        public int X { get; set; }

        [JsonPropertyName("y")]
        public int Y { get; set; }

        [JsonPropertyName("itemCount")]
        public int ItemCount { get; set; }

        [JsonPropertyName("items")]
        public List<InventoryItemDto> Items { get; set; } = new List<InventoryItemDto>();
    }
}
