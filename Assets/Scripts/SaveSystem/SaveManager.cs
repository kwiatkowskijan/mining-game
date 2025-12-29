using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using MiningGame.Player;
using MiningGame.Managers;
using MiningGame.WorldGeneration;

namespace MiningGame.SaveSystem
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        [Header("Save Settings")]
        [SerializeField] private string saveFileName = "savegame.json";
        [SerializeField] private float autoSaveIntervalMinutes = 5f;
        [SerializeField] private bool enableAutoSave = true;

        private float _sessionStartTime;
        private float _previousPlayTime;
        private bool _shouldLoadAfterSceneLoad = false;

        private string SaveFilePath => Path.Combine(Application.persistentDataPath, saveFileName);

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning($"SaveManager: DUPLICATE DETECTED! Destroying new instance. Existing instance: {Instance.gameObject.name}");
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            _sessionStartTime = Time.time;
            
            Debug.Log($"SaveManager: Initialized. Save path: {SaveFilePath}");
            Debug.Log($"SaveManager: Instance set to: {gameObject.name}");
        }

        private void Start()
        {
            if (enableAutoSave)
            {
                StartCoroutine(AutoSaveCoroutine());
            }
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"SaveManager: Scene loaded - {scene.name}");
            
            if (_shouldLoadAfterSceneLoad)
            {
                _shouldLoadAfterSceneLoad = false;
                StartCoroutine(LoadGameAfterDelay());
            }
        }

        private IEnumerator LoadGameAfterDelay()
        {
            yield return new WaitForSeconds(1f);
            
            Debug.Log("SaveManager: Auto-loading save file after scene load...");
            LoadGame();
        }

        private void OnApplicationQuit()
        {
            SaveGame();
            Debug.Log("SaveManager: Game saved on quit.");
        }

        private IEnumerator AutoSaveCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(autoSaveIntervalMinutes * 60f);
                SaveGame();
                Debug.Log($"SaveManager: Auto-save at {DateTime.Now}");
            }
        }

        // ==========================================
        // ZNAJDOWANIE OBIEKTÓW W SCENIE
        // ==========================================

        private GameObject FindPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("SaveManager: Player with tag 'Player' NOT FOUND!");
            }
            else
            {
                Debug.Log($"SaveManager: Found player '{player.name}' at {player.transform.position}");
            }
            return player;
        }

        // ==========================================
        // SAVE / LOAD / NEW GAME
        // ==========================================

        public void SaveGame()
        {
            Debug.Log("SaveManager: === SAVING GAME ===");
            
            var player = FindPlayer();
            if (player == null)
            {
                Debug.LogError("SaveManager: Cannot save - no player found!");
                return;
            }

            var stats = player.GetComponent<Stats>();
            
            GameSaveData data = new GameSaveData();
            
            data.saveVersion = "1.0";
            data.saveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            data.totalPlayTime = _previousPlayTime + (Time.time - _sessionStartTime);
            
            data.playerPosX = player.transform.position.x;
            data.playerPosY = player.transform.position.y;
            data.playerPosZ = player.transform.position.z;
            
            if (stats != null)
            {
                data.playerHealth = stats.CurrentHealth;
                data.playerMaxHealth = stats.maxHealth;
                data.playerMoney = stats.CurrentMoney;
                data.playerRubble = stats.CurrentRubble;
                data.playerMaxRubble = stats.MaxRubble;
                
                var field = typeof(Stats).GetField("mineralAmounts", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    var mineralAmounts = field.GetValue(stats) as Dictionary<Mineral, int>;
                    if (mineralAmounts != null)
                    {
                        foreach (var kvp in mineralAmounts)
                        {
                            data.collectedMinerals.Add(new MineralSaveData
                            {
                                mineralName = kvp.Key.mineralName,
                                amount = kvp.Value
                            });
                        }
                    }
                }
            }

            // Equipment
            var equipment = player.GetComponentInChildren<Equipment>();
            if (equipment == null) equipment = FindFirstObjectByType<Equipment>();
            if (equipment != null)
            {
                data.chosenSlot = equipment.chosenSlot;
            }
            
            if (MineralsManager.Instance != null && MineralsManager.Instance.minerals != null)
            {
                foreach (var mineral in MineralsManager.Instance.minerals)
                {
                    if (mineral.isDiscovered)
                    {
                        data.discoveredMineralNames.Add(mineral.mineralName);
                    }
                }
            }
            
            if (WorldChangeTracker.Instance != null)
            {
                data.destroyedBlocks = WorldChangeTracker.Instance.GetDestroyedTiles();
                data.placedBuildings = WorldChangeTracker.Instance.GetPlacedBuildings();
                Debug.Log($"SaveManager: Saving {data.destroyedBlocks.Count} destroyed blocks, {data.placedBuildings.Count} buildings");
            }
            
            try
            {
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(SaveFilePath, json);
                
                Debug.Log($"SaveManager: SAVED! Position: ({data.playerPosX:F2}, {data.playerPosY:F2}), Money: {data.playerMoney}, Health: {data.playerHealth}");
                Debug.Log($"SaveManager: File saved to: {SaveFilePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"SaveManager: SAVE FAILED - {e.Message}");
            }
        }
        
        public void RequestLoadAfterSceneLoad()
        {
            _shouldLoadAfterSceneLoad = true;
            Debug.Log("SaveManager: Will auto-load save after next scene loads.");
        }

        public void LoadGame()
        {
            Debug.Log("SaveManager: === LOADING GAME ===");

            if (!SaveExists())
            {
                Debug.LogError("SaveManager: No save file exists!");
                return;
            }

            var player = FindPlayer();
            if (player == null)
            {
                Debug.LogError("SaveManager: Cannot load - no player found!");
                return;
            }

            try
            {
                string json = File.ReadAllText(SaveFilePath);
                GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

                Debug.Log($"SaveManager: Save file loaded. Date: {data.saveDate}, Version: {data.saveVersion}");
                Debug.Log($"SaveManager: Data to restore - Position: ({data.playerPosX:F2}, {data.playerPosY:F2}), Money: {data.playerMoney}, Health: {data.playerHealth}");

                // Player Position
                player.transform.position = new Vector3(data.playerPosX, data.playerPosY, data.playerPosZ);
                Debug.Log($"SaveManager: Player moved to {player.transform.position}");

                // Player Stats
                var stats = player.GetComponent<Stats>();
                if (stats != null)
                {
                    stats.SetHealthFromSave(data.playerHealth);
                    stats.SetMoneyFromSave(data.playerMoney);
                    stats.SetRubbleFromSave(data.playerRubble);
                    
                    // Minerały
                    if (MineralsManager.Instance != null)
                    {
                        var mineralAmounts = new Dictionary<Mineral, int>();
                        foreach (var saved in data.collectedMinerals)
                        {
                            var mineral = MineralsManager.Instance.minerals.Find(m => m.mineralName == saved.mineralName);
                            if (mineral != null)
                            {
                                mineralAmounts[mineral] = saved.amount;
                            }
                        }
                        stats.SetMineralAmountsFromSave(mineralAmounts);
                    }
                    
                    Debug.Log($"SaveManager: Stats loaded - Health: {data.playerHealth}, Money: {data.playerMoney}, Rubble: {data.playerRubble}");
                }
                else
                {
                    Debug.LogError("SaveManager: Stats component not found on player!");
                }

                // Equipment
                var equipment = player.GetComponentInChildren<Equipment>();
                if (equipment == null) equipment = FindFirstObjectByType<Equipment>();
                if (equipment != null)
                {
                    equipment.chosenSlot = data.chosenSlot;
                    equipment.SlotSwitch(data.chosenSlot);
                }

                // Discovered Minerals
                if (MineralsManager.Instance != null && MineralsManager.Instance.minerals != null)
                {
                    foreach (var mineral in MineralsManager.Instance.minerals)
                    {
                        mineral.isDiscovered = data.discoveredMineralNames.Contains(mineral.mineralName);
                    }
                }
                
                if (data.destroyedBlocks != null && data.destroyedBlocks.Count > 0)
                {
                    var mineGenerator = FindFirstObjectByType<MineGenerator>();
                    if (mineGenerator != null)
                    {
                        var tilemap = mineGenerator.GetComponentInChildren<Tilemap>();
                        if (tilemap == null)
                        {
                            var tilemapObj = GameObject.Find("MineTilemap");
                            if (tilemapObj != null) tilemap = tilemapObj.GetComponent<Tilemap>();
                        }
                        
                        if (tilemap != null && WorldChangeTracker.Instance != null)
                        {
                            WorldChangeTracker.Instance.LoadDestroyedTiles(data.destroyedBlocks, tilemap);
                        }
                    }
                    Debug.Log($"SaveManager: Loaded {data.destroyedBlocks.Count} destroyed blocks");
                }

                _previousPlayTime = data.totalPlayTime;
                _sessionStartTime = Time.time;

                Debug.Log($"SaveManager: LOADED! Health: {data.playerHealth}, Money: {data.playerMoney}");
            }
            catch (Exception e)
            {
                Debug.LogError($"SaveManager: LOAD FAILED - {e.Message}\n{e.StackTrace}");
            }
        }

        public void NewGame()
        {
            Debug.Log("SaveManager: === NEW GAME ===");

            if (SaveExists())
            {
                File.Delete(SaveFilePath);
                Debug.Log("SaveManager: Old save deleted.");
            }
            
            if (WorldChangeTracker.Instance != null)
            {
                WorldChangeTracker.Instance.Clear();
            }
            
            var player = FindPlayer();
            float startHealth = 100f;
            float startMaxHealth = 100f;
            float startMoney = 10000f;
            float startRubble = 0f;
            float startMaxRubble = 10f;
            Vector3 startPosition = Vector3.zero;
            
            if (player != null)
            {
                var stats = player.GetComponent<Stats>();
                if (stats != null)
                {
                    startHealth = stats.maxHealth;
                    startMaxHealth = stats.maxHealth;
                    startMoney = stats.CurrentMoney;
                    startRubble = stats.CurrentRubble;
                    startMaxRubble = stats.MaxRubble;
                }
                startPosition = player.transform.position;
            }

            GameSaveData data = new GameSaveData
            {
                saveVersion = "1.0",
                saveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                totalPlayTime = 0f,
                playerHealth = startHealth,
                playerMaxHealth = startMaxHealth,
                playerMoney = startMoney,
                playerRubble = startRubble,
                playerMaxRubble = startMaxRubble,
                playerPosX = startPosition.x,
                playerPosY = startPosition.y,
                playerPosZ = startPosition.z,
                chosenSlot = 0,
                worldSeed = UnityEngine.Random.Range(-1000000, 1000000)
            };

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SaveFilePath, json);

            Debug.Log($"SaveManager: New game save created with starting values - Health: {startHealth}, Money: {startMoney}, Position: {startPosition}");
        }

        public bool SaveExists()
        {
            bool exists = File.Exists(SaveFilePath);
            Debug.Log($"SaveManager: SaveExists = {exists}");
            return exists;
        }

        public void DeleteSave()
        {
            if (SaveExists())
            {
                File.Delete(SaveFilePath);
                Debug.Log("SaveManager: Save deleted.");
            }
        }

        private void SetPrivateField(object obj, string fieldName, object value)
        {
            var field = obj.GetType().GetField(fieldName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(obj, value);
            }
        }

        // ==========================================
        // DEBUG - Sprawdź co jest w SAVE
        // ==========================================
        
        [ContextMenu("Debug: Show Save Content")]
        public void DebugShowSave()
        {
            if (SaveExists())
            {
                string json = File.ReadAllText(SaveFilePath);
                Debug.Log($"=== SAVE CONTENT ===\n{json}");
            }
            else
            {
                Debug.Log("No save file exists.");
            }
        }

        [ContextMenu("Debug: Force Save Now")]
        public void DebugForceSave()
        {
            SaveGame();
        }

        [ContextMenu("Debug: Force Load Now")]
        public void DebugForceLoad()
        {
            LoadGame();
        }
    }
}


