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
        
        // Flaga publiczna - inne skrypty mogą sprawdzić czy trwa ładowanie
        public static bool IsLoadingSave { get; private set; } = false;

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
            
            Debug.Log("SaveManager: Saving spawned objects...");
            var allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            
            foreach (var obj in allObjects)
            {
                if (obj == null) continue;
                
                string objName = obj.name.ToLower();
                if (objName.Contains("torch"))
                {
                    data.spawnedObjects.Add(new SpawnedObjectData
                    {
                        objectType = "Torch",
                        objectName = obj.name,
                        posX = obj.transform.position.x,
                        posY = obj.transform.position.y,
                        posZ = obj.transform.position.z
                    });
                }
                else if (objName.Contains("cart"))
                {
                    data.spawnedObjects.Add(new SpawnedObjectData
                    {
                        objectType = "Cart",
                        objectName = obj.name,
                        posX = obj.transform.position.x,
                        posY = obj.transform.position.y,
                        posZ = obj.transform.position.z
                    });
                }
            }
            
            Debug.Log($"SaveManager: Saved {data.spawnedObjects.Count} spawned objects");
            
            try
            {
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(SaveFilePath, json);
                
                Debug.Log($"SaveManager: SAVED! Position: ({data.playerPosX:F2}, {data.playerPosY:F2}), Money: {data.playerMoney}");
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
            IsLoadingSave = true;
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

                var equipment = player.GetComponentInChildren<Equipment>();
                if (equipment == null) equipment = FindFirstObjectByType<Equipment>();
                if (equipment != null)
                {
                    equipment.chosenSlot = data.chosenSlot;
                    try
                    {
                        equipment.SlotSwitch(data.chosenSlot);
                    }
                    catch
                    {
                    }
                }

                if (MineralsManager.Instance != null && MineralsManager.Instance.minerals != null)
                {
                    foreach (var mineral in MineralsManager.Instance.minerals)
                    {
                        mineral.isDiscovered = data.discoveredMineralNames.Contains(mineral.mineralName);
                    }
                }
                
                if (data.spawnedObjects != null && data.spawnedObjects.Count > 0)
                {
                    Debug.Log($"SaveManager: Loading {data.spawnedObjects.Count} spawned objects...");
                    
                    var allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
                    foreach (var obj in allObjects)
                    {
                        if (obj == null) continue;
                        string objName = obj.name.ToLower();
                        if (objName.Contains("torch") || objName.Contains("cart"))
                        {
                            Destroy(obj);
                        }
                    }
                    
                    var buildMode = FindFirstObjectByType<BuildMode>();
                    if (buildMode != null)
                    {
                        var buildModeType = typeof(BuildMode);
                        var torchPrefabField = buildModeType.GetField("torchPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        var cartPrefabField = buildModeType.GetField("cartPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        
                        GameObject torchPrefab = torchPrefabField?.GetValue(buildMode) as GameObject;
                        GameObject cartPrefab = cartPrefabField?.GetValue(buildMode) as GameObject;
                        
                        var buildTilemapField = buildModeType.GetField("buildTilemap", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        var torchTileField = buildModeType.GetField("torchTile", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        
                        Tilemap buildTilemap = buildTilemapField?.GetValue(buildMode) as Tilemap;
                        TileBase torchTile = torchTileField?.GetValue(buildMode) as TileBase;
                        
                        foreach (var objData in data.spawnedObjects)
                        {
                            Vector3 pos = new Vector3(objData.posX, objData.posY, objData.posZ);
                            
                            if (objData.objectType == "Torch" && torchPrefab != null)
                            {
                                GameObject torch = Instantiate(torchPrefab, pos, Quaternion.identity);
                                torch.name = "Torch";
                                torch.SetActive(true);
                                
                                if (buildTilemap != null && torchTile != null)
                                {
                                    Vector3Int cellPos = buildTilemap.WorldToCell(pos);
                                    buildTilemap.SetTile(cellPos, torchTile);
                                    Debug.Log($"SaveManager: Placed Torch tile at cell {cellPos}");
                                }
                                
                                Debug.Log($"SaveManager: Spawned Torch at {pos}");
                            }
                            else if (objData.objectType == "Cart" && cartPrefab != null)
                            {
                                GameObject cart = Instantiate(cartPrefab, pos, Quaternion.identity);
                                cart.name = "Cart";
                                cart.SetActive(true);
                                Debug.Log($"SaveManager: Spawned Cart at {pos}");
                            }
                        }
                        
                        Debug.Log($"SaveManager: Restored {data.spawnedObjects.Count} spawned objects");
                    }
                }

                _previousPlayTime = data.totalPlayTime;
                _sessionStartTime = Time.time;
                
                IsLoadingSave = false;

                Debug.Log($"SaveManager: LOADED! Money: {data.playerMoney}");
            }
            catch (Exception e)
            {
                Debug.LogError($"SaveManager: LOAD FAILED - {e.Message}\n{e.StackTrace}");
                IsLoadingSave = false;
            }
        }

        public void NewGame()
        {
            Debug.Log("SaveManager: === NEW GAME ===");

            IsLoadingSave = false;

            if (SaveExists())
            {
                File.Delete(SaveFilePath);
                Debug.Log("SaveManager: Old save deleted - fresh start!");
            }
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


