using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Code.Characters;
using Code.Characters.Enemies;
using Code.Managers;
using Code.Utils;
using MyBox;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Map {
    [Serializable]
    public class C_Node {
        [SerializeField] private protected Vector2Int m_NodePosition;
        [SerializeField] private protected Vector3 m_WorldPosition;
        [SerializeField] private protected MB_Cell m_Cell;

        public Vector2Int NodePosition { get => this.m_NodePosition; set => this.m_NodePosition = value; }
        public Vector3 WorldPosition { get => this.m_WorldPosition; set => this.m_WorldPosition = value; }
        public MB_Cell Cell { get => this.m_Cell; set => this.m_Cell = value; }
        public HashSet<C_Node> Neighbours { get; } = new();

        public void AddNeighbour(C_Node node) => this.Neighbours.Add(node);

        public override string ToString() => $"{this.NodePosition} - {this.WorldPosition}";
    }

    [SelectionBase]
    public class MB_Room : MonoBehaviour {
        #region Members
        [Foldout("MB_Room", true)]
        [SerializeField] private protected float m_ShowDuration;
        [SerializeField] private protected float m_ShowSpeed;
        [SerializeField] private protected MB_Cell m_SpawnCell;
        [SerializeField] private protected Transform m_EnemiesParent;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;
        [ReadOnly][SerializeField] private protected CollectionWrapper<MB_Cell> m_Cells;
        [ReadOnly][SerializeField] private protected Vector2Int m_PlayerNodePosition;

        [ReadOnly][SerializeField] private protected CollectionWrapperList<MB_EnemySpawner> m_EnemySpawners;
        [ReadOnly][SerializeField] private protected CollectionWrapperList<AMB_Enemy> m_Enemies;
        [ReadOnly][SerializeField] private protected int m_CurrentWave = 0;

        [ReadOnly][SerializeField] private protected int m_InitializedCells;
        #endregion

        #region Getters / Setters
        private float ShowDuration { get => this.m_ShowDuration; }
        private float ShowSpeed { get => this.m_ShowSpeed; }
        public MB_Cell SpawnCell { get => this.m_SpawnCell; }
        public Transform EnemiesParent { get => this.m_EnemiesParent; }

        private MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; set => this.m_ObjectsManager = value; }
        private CollectionWrapper<MB_Cell> Cells { get => this.m_Cells; set => this.m_Cells = value; }
        public Vector2Int PlayerNodePosition { get => this.m_PlayerNodePosition; private set => this.m_PlayerNodePosition = value; }

        private CollectionWrapperList<MB_EnemySpawner> EnemySpawners { get => this.m_EnemySpawners; }
        private CollectionWrapperList<AMB_Enemy> Enemies { get => this.m_Enemies; }
        public int CurrentWave { get => this.m_CurrentWave; private set => this.m_CurrentWave = value; }
        private bool IsEmpty { get => this.Enemies.Count == 0 && this.EnemySpawners.Count == 0; }

        private int InitializedCells { get => this.m_InitializedCells; set => this.m_InitializedCells = value; }
        public bool Ready { get => this.InitializedCells == this.Cells.Length; }

        private Dictionary<Vector2Int, MB_Cell> CellsDictionary { get; } = new();

        private Dictionary<Vector2Int, C_Node> GroundNodes { get; set; } = new();
        private Dictionary<Vector2Int, C_Node> HoleNodes { get; set; } = new();

        // Chunk for faster search
        private Dictionary<Vector2Int, List<C_Node>> GroundChunks { get; } = new();
        private Dictionary<Vector2Int, List<C_Node>> HoleChunks { get; } = new();
        #endregion

        #region Static / Readonly / Const
        private static readonly Vector2 CHUNK_SIZE = new(3, 3);
        #endregion

        #region Unity methods
        private void Awake() {
            this.ObjectsManager = FindFirstObjectByType<MB_ObjectsManager>(FindObjectsInactive.Include);
            this.Cells.Value = this.GetComponentsInChildren<MB_Cell>(true);
        }

        private void Start() {
            this.Until(() => this.Ready, this.InitializeNodes);
        }

        private void FixedUpdate() {
            this.PlayerNodePosition = GetNodePosition(this.ObjectsManager.Player.transform.position);
        }
        #endregion

        #region Pathfinding
        private void InitializeNodes() {
            foreach (MB_Cell cell in this.Cells.Value) {
                Vector3 position = cell.transform.localPosition;
                if (!this.CellsDictionary.TryAdd(new Vector2Int((int)position.x, (int)position.y), cell)) {
                    Debug.LogError($"Duplicate cell! {cell.name}");
                }
            }

            Vector3 roomPosition = this.transform.position;

            new Thread(() => {
                    int minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;
                    this.GroundNodes = new Dictionary<Vector2Int, C_Node>();
                    this.HoleNodes = new Dictionary<Vector2Int, C_Node>();
                    HashSet<Vector2Int> wallPositions = new();
                    void _AddNode(MB_Cell cell) {
                        Vector2Int nodePosition = GetNodePosition(cell.Node.WorldPosition);
                        Vector2Int chunkPosition = GetChunkPosition(nodePosition);
                        minX = Mathf.Min(minX, nodePosition.x);
                        minY = Mathf.Min(minY, nodePosition.y);
                        maxX = Mathf.Max(maxX, nodePosition.x);
                        maxY = Mathf.Max(maxY, nodePosition.y);

                        if (cell.HasObstacle) {
                            wallPositions.Add(nodePosition);
                        } else if (!cell.IsHole) {
                            if (this.GroundNodes.ContainsKey(nodePosition)) {
                                Debug.Log($"Conflict! {cell.Node.Cell.name}");
                            }

                            this.GroundNodes[nodePosition] = new C_Node {
                                NodePosition = nodePosition,
                                WorldPosition = cell.Node.WorldPosition,
                                Cell = cell.Node.Cell
                            };
                            this.GroundChunks.TryAdd(chunkPosition, new List<C_Node>());
                            this.GroundChunks[chunkPosition].Add(this.GroundNodes[nodePosition]);
                        }
                    }

                    foreach ((Vector2Int _, MB_Cell cell) in this.CellsDictionary) {
                        _AddNode(cell);
                    }

                    for (int x = minX - 2; x <= maxX + 2; x++) {
                        for (int y = minY - 2; y <= maxY + 2; y++) {
                            Vector2Int nodePosition = new(x, y);
                            Vector2Int chunkPosition = GetChunkPosition(nodePosition);
                            if (wallPositions.Contains(nodePosition)) continue;

                            if (this.HoleNodes.ContainsKey(nodePosition)) {
                                Debug.Log($"Conflict! {nodePosition}");
                            }

                            this.HoleNodes[nodePosition] = new C_Node {
                                NodePosition = nodePosition,
                                WorldPosition = roomPosition + new Vector3(x, y),
                                Cell = null
                            };
                            this.HoleChunks.TryAdd(chunkPosition, new List<C_Node>());
                            this.HoleChunks[chunkPosition].Add(this.HoleNodes[nodePosition]);
                        }
                    }

                    Vector2Int[] neighbours = { new(-1, 0), new(1, 0), new(0, -1), new(0, 1) };
                    foreach ((Vector2Int position, C_Node node) in this.GroundNodes) {
                        foreach (Vector2Int neighbourPosition in neighbours) {
                            if (this.GroundNodes.TryGetValue(
                                    new Vector2Int(position.x + neighbourPosition.x, position.y + neighbourPosition.y),
                                    out C_Node otherNode
                                )) {
                                node.AddNeighbour(otherNode);
                                otherNode.AddNeighbour(node);
                            }
                        }
                    }

                    foreach ((Vector2Int position, C_Node node) in this.HoleNodes) {
                        foreach (Vector2Int neighbourPosition in neighbours) {
                            if (this.HoleNodes.TryGetValue(
                                    new Vector2Int(position.x + neighbourPosition.x, position.y + neighbourPosition.y),
                                    out C_Node otherNode
                                )) {
                                node.AddNeighbour(otherNode);
                                otherNode.AddNeighbour(node);
                            }
                        }
                    }
                }
            ).Start();
        }

        private static Vector2Int GetNodePosition(Vector3 worldPosition) {
            return new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.y));
        }

        private static Vector2Int GetChunkPosition(Vector2Int nodePosition) {
            return new Vector2Int(Mathf.RoundToInt(nodePosition.x / CHUNK_SIZE.x), Mathf.RoundToInt(nodePosition.y / CHUNK_SIZE.y));
        }

        public C_Node GetNearestGroundNode(Vector3 worldPosition) => GetNode(worldPosition, this.GroundChunks, this.GroundNodes);

        public C_Node GetNearestHoleNode(Vector3 worldPosition) => GetNode(worldPosition, this.HoleChunks, this.HoleNodes);

        private static C_Node GetNode(
            Vector3 worldPosition,
            Dictionary<Vector2Int, List<C_Node>> chunks,
            Dictionary<Vector2Int, C_Node> allNodes
        ) {
            Vector2Int nodePosition = GetNodePosition(worldPosition);
            if (allNodes.TryGetValue(nodePosition, out C_Node existingNode)) {
                return existingNode;
            }

            Vector2Int chunkPosition = GetChunkPosition(nodePosition);
            if (!chunks.TryGetValue(chunkPosition, out List<C_Node> nodes)) {
                nodes = chunks.Values.SelectMany(n => n).ToList();
            }

            C_Node _FindInChunk(ICollection<C_Node> chunk, float minDistanceFastExit) {
                float distance = float.MaxValue;
                C_Node nearestNode = null;

                foreach (C_Node node in chunk) {
                    float currentDistance = (node.WorldPosition - worldPosition).sqrMagnitude;
                    if (currentDistance <= minDistanceFastExit) return node;
                    if (currentDistance > distance) continue;

                    distance = currentDistance;
                    nearestNode = node;
                }

                return nearestNode;
            }

            return _FindInChunk(nodes, 0) ?? _FindInChunk(allNodes.Values, 1);
        }
        #endregion

        private void ShowMap(MB_Cell originCell) {
            foreach (MB_Cell cell in this.Cells.Value) {
                float distance = (originCell.transform.position - cell.transform.position).magnitude;
                float delay = distance / this.ShowSpeed + Random.Range(-.05f, .05f);
                this.InSeconds(
                    delay,
                    () => {
                        cell.gameObject.SetActive(true);
                        cell.Show(this.ShowDuration);
                        this.InitializedCells++;
                    }
                );
            }
        }

        public void ShowMap() => this.ShowMap(this.SpawnCell);

        public void Register(AMB_Enemy enemy) {
            this.Enemies.Add(enemy);
        }

        public void Register(MB_EnemySpawner enemySpawner) {
            this.EnemySpawners.Add(enemySpawner);
        }

        public void Unregister(AMB_Enemy enemy) {
            this.Enemies.Remove(enemy);
            // Ignore summons for wave counting
            if (enemy.Wave != -1 && this.Enemies.Value.All(e => e.Wave == -1)) this.CurrentWave++;
            this.TryMoveToNextRoom();
        }

        public void Unregister(MB_EnemySpawner enemySpawner) {
            this.EnemySpawners.Remove(enemySpawner);
            this.TryMoveToNextRoom();
        }

        private void TryMoveToNextRoom() {
            if (this.IsEmpty) {
                this.InSeconds(0, this.ObjectsManager.RoomManager.NextRoom);
            }
        }

        #if UNITY_EDITOR
        [ButtonMethod]
        private void EnableCells() {
            Transform cellsTransform = this.transform.Find("Cells");
            for (int i = 0; i < cellsTransform.childCount; i++) {
                cellsTransform.GetChild(i).gameObject.SetActive(true);
            }

            EditorUtility.SetDirty(this.gameObject);
        }

        [ButtonMethod]
        private void DisableCells() {
            Transform cellsTransform = this.transform.Find("Cells");
            for (int i = 0; i < cellsTransform.childCount; i++) {
                cellsTransform.GetChild(i).gameObject.SetActive(false);
            }

            EditorUtility.SetDirty(this.gameObject);
        }

        [ButtonMethod]
        private void SortCells() {
            List<MB_Cell> cells = this.transform.Find("Cells").GetComponentsInChildren<MB_Cell>().ToList();
            foreach (MB_Cell cell in cells) {
                Vector2 position = cell.transform.position.ToVector2();
                cell.name = $"[{position.x},{position.y}] {cell.GetPrefabDefinition().name}";
            }

            cells.Sort((cell1, cell2) => {
                    Vector2 position1 = cell1.transform.position.ToVector2();
                    Vector2 position2 = cell2.transform.position.ToVector2();

                    return Mathf.Approximately(position1.x, position2.x)
                        ? position1.y.CompareTo(position2.y)
                        : position1.x.CompareTo(position2.x);
                }
            );
            for (int i = 0; i < cells.Count; i++) {
                cells[i].transform.SetSiblingIndex(i);

                if (i >= 1 && cells[i].transform.position == cells[i - 1].transform.position) {
                    Debug.LogError($"Duplicate cells! {cells[i].name} / {cells[i - 1].name}");
                }
            }

            EditorUtility.SetDirty(this.gameObject);
        }
        #endif
    }
}
