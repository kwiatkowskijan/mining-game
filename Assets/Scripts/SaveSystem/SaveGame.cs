using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

namespace MiningGame.SaveSystem
{
    /// <summary>
    /// Główna klasa danych save'a - zawiera WSZYSTKIE dane do zapisania.
    /// Dodawaj tutaj nowe pola gdy pojawią się nowe systemy w grze.
    /// </summary>
    [Serializable]
    public class GameSaveData
    {
        // === METADATA ===
        public string saveVersion = "1.0";
        public string saveDate;
        public float totalPlayTime;

        // === PLAYER STATS ===
        public float playerHealth;
        public float playerMaxHealth;
        public float playerMoney;
        public float playerRubble;
        public float playerMaxRubble;

        // === PLAYER POSITION ===
        public float playerPosX;
        public float playerPosY;
        public float playerPosZ;

        // === MINERALS (ilości zebrane przez gracza) ===
        public List<MineralSaveData> collectedMinerals = new List<MineralSaveData>();

        // === DISCOVERED MINERALS ===
        public List<string> discoveredMineralNames = new List<string>();

        // === EQUIPMENT ===
        public int chosenSlot;
        public List<EquipmentSlotData> equipmentSlots = new List<EquipmentSlotData>();

        // === WORLD GENERATION ===
        public int worldSeed;
        public List<ChunkData> generatedChunks = new List<ChunkData>();
        public List<DestroyedBlockData> destroyedBlocks = new List<DestroyedBlockData>();
        public List<PlacedBuildingData> placedBuildings = new List<PlacedBuildingData>();
        
        // === SPAWNED OBJECTS (Torch, Cart w hierarchii) ===
        public List<SpawnedObjectData> spawnedObjects = new List<SpawnedObjectData>();

        // === SHOP (opcjonalnie) ===
        public List<string> currentShopItems = new List<string>();
    }

    [Serializable]
    public class MineralSaveData
    {
        public string mineralName;
        public int amount;
    }

    [Serializable]
    public class EquipmentSlotData
    {
        public int slotIndex;
        public string itemSpriteName; // nazwa sprite'a przypisanego do slotu
        public bool isActive;
    }

    [Serializable]
    public class ChunkData
    {
        public int chunkX;
        public int chunkY;
    }

    [Serializable]
    public class DestroyedBlockData
    {
        public int tileX;
        public int tileY;
        public int tileZ;
    }

    [Serializable]
    public class PlacedBuildingData
    {
        public string buildingType; // "Ladder", "Torch", "Rope", "Cart", "Rails"
        public float posX;
        public float posY;
        public float posZ;
    }
    
    /// <summary>
    /// Spawowany GameObject w hierarchii (Torch, Cart)
    /// </summary>
    [Serializable]
    public class SpawnedObjectData
    {
        public string objectType;  // "Torch", "Cart"
        public string objectName;  // nazwa z hierarchii
        public float posX;
        public float posY;
        public float posZ;
    }
}
