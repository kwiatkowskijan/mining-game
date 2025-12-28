using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace MiningGame.SaveSystem
{
    /// <summary>
    /// Śledzi wszystkie zmiany w świecie (zniszczone bloki, postawione budynki).
    /// Dodaj do tego samego obiektu co SaveManager lub osobno w scenie.
    /// </summary>
    public class WorldChangeTracker : MonoBehaviour
    {
        public static WorldChangeTracker Instance { get; private set; }

        // Lista zniszczonych bloków (pozycje tile'ów które zostały usunięte)
        private HashSet<Vector3Int> _destroyedTiles = new HashSet<Vector3Int>();
        
        // Lista postawionych budynków
        private List<PlacedBuildingData> _placedBuildings = new List<PlacedBuildingData>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning($"WorldChangeTracker: DUPLICATE DETECTED! Destroying new instance. Existing instance: {Instance.gameObject.name}");
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            Debug.Log($"WorldChangeTracker: Initialized. Instance set to: {gameObject.name}");
        }

        /// <summary>
        /// Wywołaj gdy gracz zniszczy blok
        /// </summary>
        public void RegisterDestroyedTile(Vector3Int tilePos)
        {
            _destroyedTiles.Add(tilePos);
            Debug.Log($"WorldChangeTracker: Tile destroyed at {tilePos}. Total destroyed: {_destroyedTiles.Count}");
        }

        /// <summary>
        /// Wywołaj gdy gracz postawi budynek
        /// </summary>
        public void RegisterPlacedBuilding(string buildingType, Vector3 worldPos)
        {
            _placedBuildings.Add(new PlacedBuildingData
            {
                buildingType = buildingType,
                posX = worldPos.x,
                posY = worldPos.y,
                posZ = worldPos.z
            });
            Debug.Log($"WorldChangeTracker: Building '{buildingType}' placed at {worldPos}. Total buildings: {_placedBuildings.Count}");
        }

        /// <summary>
        /// Pobierz wszystkie zniszczone tile'e do zapisu
        /// </summary>
        public List<DestroyedBlockData> GetDestroyedTiles()
        {
            var list = new List<DestroyedBlockData>();
            foreach (var pos in _destroyedTiles)
            {
                list.Add(new DestroyedBlockData
                {
                    tileX = pos.x,
                    tileY = pos.y,
                    tileZ = pos.z
                });
            }
            return list;
        }

        /// <summary>
        /// Pobierz wszystkie postawione budynki do zapisu
        /// </summary>
        public List<PlacedBuildingData> GetPlacedBuildings()
        {
            return new List<PlacedBuildingData>(_placedBuildings);
        }

        /// <summary>
        /// Wczytaj zniszczone tile'e z save'a
        /// </summary>
        public void LoadDestroyedTiles(List<DestroyedBlockData> data, Tilemap tilemap)
        {
            _destroyedTiles.Clear();
            
            if (data == null || tilemap == null) return;

            foreach (var block in data)
            {
                Vector3Int pos = new Vector3Int(block.tileX, block.tileY, block.tileZ);
                _destroyedTiles.Add(pos);
                tilemap.SetTile(pos, null); // Usuń tile
            }

            Debug.Log($"WorldChangeTracker: Loaded {_destroyedTiles.Count} destroyed tiles.");
        }

        /// <summary>
        /// Wyczyść wszystko (nowa gra)
        /// </summary>
        public void Clear()
        {
            _destroyedTiles.Clear();
            _placedBuildings.Clear();
            Debug.Log("WorldChangeTracker: Cleared all tracked changes.");
        }
    }
}

