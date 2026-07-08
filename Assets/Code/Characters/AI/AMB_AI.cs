using System;
using System.Collections.Generic;
using System.Linq;
using Code.Characters.Enemies;
using Code.Managers;
using Code.Map;
using Code.Utils;
using MyBox;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Characters.AI {
    public abstract class AMB_AI : MonoBehaviour {
        [Serializable]
        private protected class C_AINode {
            [SerializeField] private protected C_Node m_Node;

            public C_Node Node { get => this.m_Node; set => this.m_Node = value; }
        }

        [Serializable]
        private protected class C_Path {
            [SerializeField] private protected C_AINode m_Destination;
            [SerializeField] public List<C_AINode> m_Nodes;

            public C_AINode Destination { get => this.m_Destination; set => this.m_Destination = value; }
            public List<C_AINode> Nodes { get => this.m_Nodes; set => this.m_Nodes = value; }
        }

        [Serializable]
        public class C_Decision {
            [SerializeField] private protected Vector2 m_MovementDirection;
            [SerializeField] private protected Vector2 m_AimDirection;

            public Vector2 MovementDirection { get => this.m_MovementDirection; set => this.m_MovementDirection = value; }
            public Vector2 AimDirection { get => this.m_AimDirection; set => this.m_AimDirection = value; }
        }

        [Serializable]
        public class C_Avoidance {
            [SerializeField] private protected List<E_AvoidTarget> m_AvoidTargets;
            [SerializeField] private protected float m_Distance;
            [SerializeField] private protected float m_Weight;
            [SerializeField] private protected E_Behaviour[] m_Behaviours;

            [ReadOnly][SerializeField] private protected LayerMask m_AvoidanceLayers;
            [ReadOnly][SerializeField] private protected bool m_IsEnvironment;

            public List<E_AvoidTarget> AvoidTargets { get => this.m_AvoidTargets; }
            public float Distance { get => this.m_Distance; }
            public float Weight { get => this.m_Weight; }
            public E_Behaviour[] Behaviours { get => this.m_Behaviours; }

            public LayerMask AvoidanceLayers { get => this.m_AvoidanceLayers; set => this.m_AvoidanceLayers = value; }
            public bool IsEnvironment { get => this.m_IsEnvironment; set => this.m_IsEnvironment = value; }
        }

        [Serializable]
        public class C_MovementBehaviour {
            [SerializeField] private protected float m_PlayerAttraction;
            [SerializeField] private protected float m_NoiseWeight;
            [SerializeField] private protected float m_ObstaclesRepulsion;

            public float PlayerAttraction { get => this.m_PlayerAttraction; }
            public float NoiseWeight { get => this.m_NoiseWeight; }
            public float ObstaclesRepulsion { get => this.m_ObstaclesRepulsion; }
        }

        #region Members
        [Foldout("AMB_AI", true)]
        [SerializeField] private protected List<C_Avoidance> m_Avoidances;
        [SerializeField] private protected float m_AggressivePropagationRadius;
        [SerializeField][Range(0, 1)] private protected float m_DecisionLerp;

        [SerializeField] private protected bool m_Flying;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;

        [ReadOnly][SerializeField] private protected C_Decision m_Decision;
        [ReadOnly][SerializeField] private protected C_Path m_Path;

        [ReadOnly][SerializeField] private protected MB_Room m_Room;
        [ReadOnly][SerializeField] private protected AMB_Enemy m_Enemy;
        [ReadOnly][SerializeField] private protected LayerMask m_HolesLayer;
        [ReadOnly][SerializeField] private protected LayerMask m_WallsLayer;
        [ReadOnly][SerializeField] private protected E_Behaviour m_Behaviour;
        [ReadOnly][SerializeField] private protected float m_DistanceToPlayer;

        [ReadOnly][SerializeField] private protected List<Vector2> m_Surroundings;
        [ReadOnly][SerializeField] private protected Vector2 m_AvoidanceDirection;

        [ReadOnly][SerializeField] private protected bool m_WallsBlockingSight;
        [ReadOnly][SerializeField] private protected bool m_HolesBlockingSight;

        [ReadOnly][SerializeField] private protected Vector2 m_TrueVectorToPlayer;
        [ReadOnly][SerializeField] private protected Vector2 m_VectorToPlayer;
        [ReadOnly][SerializeField] private protected Vector2Int m_EnemyNodePosition;
        [ReadOnly][SerializeField] private protected Vector2Int m_PlayerNodePosition;

        [ReadOnly][SerializeField] private protected AMB_AI[] m_OtherAIs;

        [ReadOnly][SerializeField] private protected float m_EnabledAt;

        [Separator("Noise")]
        [ReadOnly][SerializeField] private protected int m_MovementSeed;
        [ReadOnly][SerializeField] private protected float m_XOff;
        [ReadOnly][SerializeField] private protected float m_YOff;

        private RaycastHit2D[] RaycastHits { get; } = new RaycastHit2D[15];
        #endregion

        #region Getters / Setters
        private IEnumerable<C_Avoidance> Avoidances { get => this.m_Avoidances; }
        private float AggressivePropagationRadius { get => this.m_AggressivePropagationRadius; }
        private float DecisionLerp { get => this.m_DecisionLerp; }

        private bool Flying { get => this.m_Flying; }

        protected MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; private set => this.m_ObjectsManager = value; }

        public C_Decision Decision { get => this.m_Decision; private set => this.m_Decision = value; }
        private C_Path Path { get => this.m_Path; set => this.m_Path = value; }

        private MB_Room Room { get => this.m_Room; set => this.m_Room = value; }
        protected AMB_Enemy Enemy { get => this.m_Enemy; private set => this.m_Enemy = value; }
        private LayerMask HolesLayer { get => this.m_HolesLayer; set => this.m_HolesLayer = value; }
        private LayerMask WallsLayer { get => this.m_WallsLayer; set => this.m_WallsLayer = value; }
        public E_Behaviour Behaviour { get => this.m_Behaviour; protected set => this.m_Behaviour = value; }
        protected float DistanceToPlayer { get => this.m_DistanceToPlayer; private set => this.m_DistanceToPlayer = value; }

        private List<Vector2> Surroundings { get => this.m_Surroundings; set => this.m_Surroundings = value; }
        private Vector2 AvoidanceDirection { get => this.m_AvoidanceDirection; set => this.m_AvoidanceDirection = value; }

        protected bool WallsBlockingSight { get => this.m_WallsBlockingSight; private set => this.m_WallsBlockingSight = value; }
        protected bool HolesBlockingSight { get => this.m_HolesBlockingSight; private set => this.m_HolesBlockingSight = value; }

        protected Vector2 TrueVectorToPlayer { get => this.m_TrueVectorToPlayer; private set => this.m_TrueVectorToPlayer = value; }
        protected Vector2 VectorToPlayer { get => this.m_VectorToPlayer; private set => this.m_VectorToPlayer = value; }
        private Vector2Int EnemyNodePosition { get => this.m_EnemyNodePosition; set => this.m_EnemyNodePosition = value; }
        private Vector2Int PlayerNodePosition { get => this.m_PlayerNodePosition; set => this.m_PlayerNodePosition = value; }

        private AMB_AI[] OtherAIs { get => this.m_OtherAIs; set => this.m_OtherAIs = value; }

        protected float EnabledAt { get => this.m_EnabledAt; private set => this.m_EnabledAt = value; }

        private int MovementSeed { get => this.m_MovementSeed; set => this.m_MovementSeed = value; }
        private float XOff { get => this.m_XOff; set => this.m_XOff = value; }
        private float YOff { get => this.m_YOff; set => this.m_YOff = value; }
        #endregion

        #region Static / Readonly / Const
        private const float NOISE_SCALE = .2f;
        private const float OFFSET_CYCLING = .1f;
        protected const float AGGRESSIVE_DELAY = 0f;
        private static readonly Dictionary<E_AvoidTarget, string[]> AVOID_TARGETS_MAPPING = new() {
            {
                E_AvoidTarget.Enemies,
                new[] { "Spells/Enemy colliders - Body" }
            }, {
                E_AvoidTarget.Player,
                new[] { "Spells/Character colliders - Body" }
            }, {
                E_AvoidTarget.Walls,
                new[] { "Environment/Walls", "Environment/Map bounds" }
            }, {
                E_AvoidTarget.Holes,
                new[] { "Environment/Holes", "Environment/Map bounds" }
            }, {
                E_AvoidTarget.CollideWithEnemiesBodySpells,
                new[] { "Spells/Collide with enemies - Body", "Spells/Collide with everything - Body" }
            }, {
                E_AvoidTarget.CollideWithCharactersBodySpells,
                new[] { "Spells/Collide with characters - Body", "Spells/Collide with everything - Body" }
            }, {
                E_AvoidTarget.CollideWithEnemiesGroundLowSpells,
                new[] { "Spells/Collide with enemies - Ground (Low)", "Spells/Collide with environment - Ground (Low)" }
            }, {
                E_AvoidTarget.CollideWithCharactersGroundLowSpells,
                new[] { "Spells/Collide with characters - Ground (Low)", "Spells/Collide with everything - Ground (Low)" }
            }, {
                E_AvoidTarget.CollideWithEnemiesGroundHighSpells,
                new[] { "Spells/Collide with enemies - Ground (High)", "Spells/Collide with everything - Ground (High)" }
            }, {
                E_AvoidTarget.CollideWithCharactersGroundHighSpells,
                new[] { "Spells/Collide with characters - Ground (High)", "Spells/Collide with everything - Ground (High)" }
            }
        };
        #endregion

        #region Unity methods
        protected virtual void Awake() {
            this.Room = FindFirstObjectByType<MB_Room>(FindObjectsInactive.Include);
            // this.Room = this.GetComponentInParent<MB_Room>();
            this.Enemy = this.GetComponent<AMB_Enemy>();
            this.OtherAIs = FindObjectsByType<AMB_AI>(FindObjectsSortMode.None).Where(ai => ai != this).ToArray();
            this.Surroundings = new List<Vector2> {
                new Vector2(1, 0).normalized,
                new Vector2(1, 1).normalized,
                new Vector2(0, 1).normalized,
                new Vector2(-1, 1).normalized,
                new Vector2(-1, 0).normalized,
                new Vector2(-1, -1).normalized,
                new Vector2(0, -1).normalized,
                new Vector2(1, -1).normalized
            };
            this.Behaviour = E_Behaviour.Idle;

            this.MovementSeed = (int)Random.Range((float)-1e3, (float)1e3);
            this.XOff = (int)Random.Range((float)-1e3, (float)1e3);
            this.YOff = (int)Random.Range((float)-1e3, (float)1e3);
            this.ObjectsManager = FindFirstObjectByType<MB_ObjectsManager>(FindObjectsInactive.Include);

            foreach (C_Avoidance avoidance in this.Avoidances) {
                avoidance.AvoidanceLayers = new LayerMask();
                avoidance.IsEnvironment = avoidance.AvoidTargets.Any(avoidTarget =>
                    avoidTarget is E_AvoidTarget.Walls or E_AvoidTarget.Holes
                );
                foreach (E_AvoidTarget avoidTarget in avoidance.AvoidTargets) {
                    foreach (string layerName in AVOID_TARGETS_MAPPING[avoidTarget]) {
                        avoidance.AvoidanceLayers |= 1 << LayerMask.NameToLayer(layerName);
                    }
                }
            }

            this.WallsLayer = new LayerMask();
            this.HolesLayer = new LayerMask();
            foreach (string layerName in AVOID_TARGETS_MAPPING[E_AvoidTarget.Walls]) {
                this.WallsLayer |= 1 << LayerMask.NameToLayer(layerName);
                this.HolesLayer |= 1 << LayerMask.NameToLayer(layerName);
            }

            foreach (string layerName in AVOID_TARGETS_MAPPING[E_AvoidTarget.Holes]) {
                this.HolesLayer |= 1 << LayerMask.NameToLayer(layerName);
            }
        }

        protected virtual void FixedUpdate() {
            this.UpdateNoise();
            List<Vector2> directions = this.GetSurroundings();
            this.AvoidanceDirection = directions.Aggregate(Vector2.zero, (acc, direction) => acc - direction);
            this.UpdatePlayerDistance();
            this.UpdateBehaviour();
            Vector2 movementDirection = this.GetMovementDirection();
            Vector2 aimDirection = this.GetAimDirection();

            this.Decision = new C_Decision {
                MovementDirection = Vector2.Lerp(this.Decision.MovementDirection, movementDirection, this.DecisionLerp),
                AimDirection = aimDirection
            };
        }

        protected virtual void OnEnable() {
            this.EnabledAt = Time.time;
        }

        #if UNITY_EDITOR
        protected virtual void OnDrawGizmos() {
            if (!Application.isPlaying) return;
            if (!this.Enemy) return;

            Vector2 position = this.Enemy.transform.position;
            foreach (C_Avoidance avoidance in this.Avoidances) {
                if (avoidance.Behaviours.Contains(this.Behaviour))
                    Handles.DrawWireArc(position, Vector3.forward, position, 360, avoidance.Distance);
            }

            foreach (Vector2 direction in this.GetSurroundings()) {
                Handles.color = Color.red;
                Handles.DrawLine(position, position + direction);
            }

            Handles.color = Color.magenta;
            Handles.DrawLine(position, position + this.AvoidanceDirection);

            // Handles.color = Color.yellow;
            // Handles.DrawLine(position, position + this.GetRandomDirection() * 3, 4);
            //
            Handles.color = Color.green;
            Handles.DrawLine(position, position + this.VectorToPlayer, 1);
        }
        #endif
        #endregion

        protected abstract void UpdateBehaviour();

        public virtual void SetBehaviour(E_Behaviour behaviour, bool propagateAggressive) {
            if (propagateAggressive && behaviour == E_Behaviour.Aggressive) {
                if (this.AggressivePropagationRadius == 0) return;

                foreach (AMB_AI otherAI in this.OtherAIs.Where(otherAI => otherAI != null)) {
                    if (Vector2.Distance(this.transform.position, otherAI.transform.position) > this.AggressivePropagationRadius) continue;

                    otherAI.SetBehaviour(E_Behaviour.Aggressive, false);
                }
            }
        }

        protected abstract Vector2 GetMovementDirection();
        protected abstract Vector2 GetAimDirection();

        protected void OnNewBehaviour(E_Behaviour newBehaviour, E_Behaviour testBehaviour, Action action) {
            if (this.Behaviour != newBehaviour && newBehaviour == testBehaviour) action.Invoke();
        }

        private void UpdatePlayerDistance() {
            this.VectorToPlayer = this.ObjectsManager.Player.transform.position - this.Enemy.transform.position;
            this.TrueVectorToPlayer = this.VectorToPlayer;
            this.DistanceToPlayer = this.VectorToPlayer.magnitude;

            this.WallsBlockingSight = this.AreWallsBlockingSight(this.VectorToPlayer);
            this.HolesBlockingSight = this.AreHolesBlockingSight(this.VectorToPlayer);

            if (this.Flying
                    ? this.WallsBlockingSight
                    : this.HolesBlockingSight) {
                this.Path = this.FindPathToPlayer();
                if (this.Path != null && this.Path.Nodes.Count != 0) {
                    C_Node currentMapNode = this.Flying
                        ? this.Room.GetNearestHoleNode(this.Enemy.transform.position)
                        : this.Room.GetNearestGroundNode(this.Enemy.transform.position);
                    int index = 0;
                    while (this.Path.Nodes[index].Node == currentMapNode) {
                        index++;
                    }

                    this.VectorToPlayer = this.Path.Nodes[index].Node.WorldPosition - this.Enemy.transform.position;
                }
            }
        }

        // Normalized
        protected Vector2 GetDirectionToPlayer(float directionWeight, float noiseWeight, float avoidanceWeight) {
            Vector2 direction = Vector2.zero;
            if (directionWeight != 0) direction += this.VectorToPlayer.normalized * directionWeight;
            if (noiseWeight != 0) direction += this.GetRandomDirection() * noiseWeight;
            if (avoidanceWeight != 0) direction += this.AvoidanceDirection.normalized * avoidanceWeight;

            return direction.normalized;
        }

        private List<Vector2> GetSurroundings() {
            List<Vector2> result = new();
            Vector2 position = this.Enemy.transform.position;
            foreach (Vector2 direction in this.Surroundings) {
                foreach (C_Avoidance avoidance in this.Avoidances) {
                    if (!avoidance.Behaviours.Contains(this.Behaviour)) continue;

                    int hits = Physics2D.RaycastNonAlloc(
                        position,
                        direction,
                        this.RaycastHits,
                        avoidance.Distance,
                        avoidance.AvoidanceLayers
                    );
                    float distance = float.MaxValue;
                    bool hit = false;
                    for (int i = 0; i < hits; i++) {
                        Collider2D col = this.RaycastHits[i].collider;
                        if (col != null && col.gameObject.GetComponentInParent<AMB_Enemy>() != this.Enemy) {
                            hit = true;
                            distance = Mathf.Min(distance, this.RaycastHits[i].distance);
                        }
                    }

                    if (hit) {
                        float distanceRatio = (avoidance.Distance - distance) / avoidance.Distance;
                        result.Add(distanceRatio * avoidance.Weight * direction);
                    }
                }
            }

            return result;
        }

        private void UpdateNoise() {
            Vector2 position = this.Enemy.transform.position;
            Vector2 direction = this.GetRandomDirection();

            foreach (C_Avoidance avoidance in this.Avoidances) {
                if (!avoidance.IsEnvironment) continue;
                int hits = Physics2D.RaycastNonAlloc(position, direction, this.RaycastHits, avoidance.Distance, avoidance.AvoidanceLayers);
                for (int i = 0; i < hits; i++) {
                    Collider2D col = this.RaycastHits[i].collider;
                    if (col != null && col.gameObject.GetComponentInParent<AMB_Enemy>() != this.Enemy) {
                        this.XOff += OFFSET_CYCLING;
                        this.YOff += OFFSET_CYCLING;
                        return;
                    }
                }
            }
        }

        private Vector2 GetRandomDirection() {
            float now = Time.time;
            return new Vector2(
                Mathf.PerlinNoise(now * NOISE_SCALE + this.XOff, this.MovementSeed) * 2 - 1,
                Mathf.PerlinNoise(this.MovementSeed, now * NOISE_SCALE + this.YOff) * 2 - 1
            ).normalized;
        }

        private C_Path FindPathToPlayer() {
            C_Node startingNode = this.Flying
                ? this.Room.GetNearestHoleNode(this.Enemy.transform.position)
                : this.Room.GetNearestGroundNode(this.Enemy.transform.position);

            if (startingNode == null) return null;

            if (this.PlayerNodePosition == this.Room.PlayerNodePosition && this.EnemyNodePosition == startingNode.NodePosition) {
                return this.Path;
            }

            this.PlayerNodePosition = this.Room.PlayerNodePosition;
            this.EnemyNodePosition = startingNode.NodePosition;

            float _H(C_Node node) => (node.NodePosition - this.Room.PlayerNodePosition).sqrMagnitude;

            C_Node destination = this.Flying
                ? this.Room.GetNearestHoleNode(this.ObjectsManager.Player.transform.position)
                : this.Room.GetNearestGroundNode(this.ObjectsManager.Player.transform.position);

            List<C_Node> openSet = new() { startingNode };
            Dictionary<C_Node, C_Node> cameFrom = new();
            Dictionary<C_Node, float> gScore = new() {
                [startingNode] = 0
            };
            Dictionary<C_Node, float> fScore = new() {
                [startingNode] = _H(startingNode)
            };

            C_AINode _CheapestNode(IEnumerable<C_Node> tiles) {
                C_AINode result = null;
                float score = float.MaxValue;
                foreach (C_Node tile in tiles) {
                    if (fScore.GetValueOrDefault(tile, float.MaxValue) >= score) continue;
                    score = fScore[tile];
                    result = new C_AINode { Node = tile };
                }

                return result;
            }
            C_Path _Path(C_AINode to) {
                if (to.Node == startingNode)
                    return new C_Path {
                        Destination = to,
                        Nodes = new List<C_AINode>()
                    };

                List<C_AINode> path = new() {
                    to
                };
                C_Node current = to.Node;
                while (cameFrom.ContainsKey(current)) {
                    current = cameFrom[current];
                    path.Insert(0, new C_AINode { Node = current });
                }

                return new C_Path {
                    Destination = to,
                    Nodes = path
                };
            }

            C_Path path = null;
            while (openSet.Count > 0) {
                C_AINode current = _CheapestNode(openSet);
                if (current.Node == destination) {
                    path = _Path(current);
                    break;
                }

                openSet.Remove(current.Node);
                foreach (C_Node neighbour in current.Node.Neighbours) {
                    float tentativeGScore = gScore[current.Node];
                    if (tentativeGScore >= gScore.GetValueOrDefault(neighbour, float.MaxValue)) continue;
                    cameFrom[neighbour] = current.Node;
                    gScore[neighbour] = tentativeGScore;
                    fScore[neighbour] = tentativeGScore + _H(neighbour);
                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }

            if (path == null) return null;

            while (path.Nodes.Count > 1) {
                Vector2 direction = path.Nodes[1].Node.WorldPosition.ToVector2() - this.Enemy.transform.position.ToVector2();
                bool visible = this.Flying
                    ? !this.AreWallsBlockingSight(direction)
                    : !this.AreHolesBlockingSight(direction);
                if (visible) path.Nodes.RemoveAt(0);
                else break;
            }

            return path;
        }

        private bool AreWallsBlockingSight(Vector2 direction) {
            return Physics2D.RaycastNonAlloc(
                this.Enemy.transform.position,
                direction,
                this.RaycastHits,
                direction.magnitude,
                this.WallsLayer
            )
            != 0;
        }
        private bool AreHolesBlockingSight(Vector2 direction) {
            return Physics2D.RaycastNonAlloc(
                this.Enemy.transform.position,
                direction,
                this.RaycastHits,
                direction.magnitude,
                this.HolesLayer
            )
            != 0;
        }
    }
}
