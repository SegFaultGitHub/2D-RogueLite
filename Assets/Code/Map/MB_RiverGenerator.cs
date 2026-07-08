using System;
using System.Collections.Generic;
using Code.Utils;
using MyBox;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Map {
    public class MB_RiverGenerator : MonoBehaviour {
        [Serializable]
        private protected class C_RiverPoint {
            [SerializeField] private Vector2 m_Position;
            [SerializeField] private Color m_Color;

            public Vector2 Position { get => this.m_Position; set => this.m_Position = value; }
            public Color Color { get => this.m_Color; set => this.m_Color = value; }
        }

        [Serializable]
        private protected class C_RiverTile {
            [SerializeField] private Vector2Int m_Chunk;
            [SerializeField] private Vector2Int m_Position;
            [SerializeField] private Vector2 m_RealPosition;

            public Vector2Int Chunk { get => this.m_Chunk; set => this.m_Chunk = value; }
            public Vector2Int Position { get => this.m_Position; set => this.m_Position = value; }
            public Vector2 RealPosition { get => this.m_RealPosition; set => this.m_RealPosition = value; }
        }

        #region Members
        [Foldout("MB_RiverGenerator", true)]
        [SerializeField] private protected Vector2 m_RiverStart;
        [SerializeField] private protected Vector2 m_RiverEnd;

        [SerializeField] private protected float m_SectionSize;
        [SerializeField][Range(0, .5f)] private protected float m_SectionDelta;
        [SerializeField] private protected float m_SectionDrag;

        [SerializeField][MinMaxRange(0, 10)] private protected RangedInt m_SubSectionCount;
        [SerializeField] private protected float m_SubSectionDrag;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected List<C_RiverPoint> m_Points;
        [ReadOnly][SerializeField] private protected List<C_RiverTile> m_RiverTiles;
        #endregion

        #region Getters / Setters
        private Vector2 RiverStart { get => this.m_RiverStart; set => this.m_RiverStart = value; }
        private Vector2 RiverEnd { get => this.m_RiverEnd; set => this.m_RiverEnd = value; }

        private float SectionSize { get => this.m_SectionSize; }
        private float SectionDelta { get => this.m_SectionDelta; }
        private float SectionDrag { get => this.m_SectionDrag; }

        private RangedInt SubSectionCount { get => this.m_SubSectionCount; }
        private float SubSectionDrag { get => this.m_SubSectionDrag; }

        private List<C_RiverPoint> Points { get => this.m_Points; }
        private List<C_RiverTile> RiverTiles { get => this.m_RiverTiles; set => this.m_RiverTiles = value; }

        private Dictionary<Vector2Int, List<C_RiverTile>> RiverTileMap { get; set; } = new();
        #endregion

        #region Static / Readonly / Const
        private const int MAP_HEIGHT = 22;
        private const int MAP_WIDTH = 36;
        #endregion

        #region Unity methods
        private void Start() {
            this.Generate();
        }

        [Serializable]
        public class BezierPoints {
            public Vector2 Start, Mid, End;

            public override string ToString() {
                return $"Start: {this.Start}, Mid: {this.Mid}, End: {this.End}";
            }
        }
        [Separator("TEMP")]
        public int BezierDivisions;
        [Range(0, 1)] public float T;

        public bool AutoUpdate;

        private void OnDrawGizmos() {
            if (this.AutoUpdate) this.Generate();

            foreach (C_RiverPoint point in this.Points) {
                Gizmos.color = point.Color;
                Gizmos.DrawSphere(point.Position, 0.2f);
            }

            List<BezierPoints> bezierPoints = this.GetBezierPoints();
            foreach (BezierPoints points in bezierPoints) {
                Gizmos.color = Color.blue;
                for (int i = 0; i <= this.BezierDivisions; i++) {
                    Gizmos.DrawSphere(this.GetBezierPoint(points.Start, points.Mid, points.End, (float)i / this.BezierDivisions), 0.3f);
                }

                // Gizmos.DrawSphere(this.GetBezierPoint(points.Start, points.Mid, points.End, (float)i / this.BezierDivisions), 0.5f);

                // Gizmos.color = Color.yellow;
                // Gizmos.DrawSphere(points.Start, 0.1f);
                // Gizmos.color = Color.red;
                // Gizmos.DrawSphere(points.Mid, 0.1f);
                // Gizmos.color = Color.yellow;
                // Gizmos.DrawSphere(points.End, 0.1f);
            }

            float t = Mathf.Min(this.T, .99999f) * bezierPoints.Count;
            int intT = Mathf.FloorToInt(t);
            BezierPoints current = bezierPoints[intT];
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(this.GetBezierPoint(current.Start, current.Mid, current.End, t - intT), 0.5f);
        }

        // [SerializeField] public List<BezierPoints> Sections;
        // public int BezierDivisions;
        // [Range(0,1)] public float BezierT;
        // private void OnDrawGizmos() {
        //     Gizmos.color = Color.green;
        //     foreach (Vector2 point in this.Points) {
        //         Gizmos.DrawSphere(point, 0.1f);
        //     }
        //
        //     foreach (BezierPoints points in this.Sections) {
        //         Gizmos.color = Color.green;
        //         for (int i = 0; i <= this.BezierDivisions; i++) {
        //             Gizmos.DrawSphere(this.GetBeziersPoint(points.Start, points.Mid, points.End, (float)i / this.BezierDivisions), 0.1f);
        //         }
        //         Gizmos.color = Color.blue;
        //         Gizmos.DrawSphere(points.Start, 0.1f);
        //         Gizmos.DrawSphere(points.Mid, 0.1f);
        //         Gizmos.DrawSphere(points.End, 0.1f);
        //         Gizmos.color = Color.red;
        //         Gizmos.DrawSphere(this.GetBeziersPoint(points.Start, points.Mid, points.End, this.BezierT), 0.1f);
        //     }
        // }
        #endregion

        private List<BezierPoints> GetBezierPoints() {
            List<Vector2> points = new() {
                this.Points[0].Position,
                this.Points[1].Position
            };

            for (int i = 2; i < this.Points.Count - 1; i++) {
                points.Add((this.Points[i].Position + this.Points[i - 1].Position) / 2);
                points.Add(this.Points[i].Position);
            }

            points.Add(this.Points[^1].Position);

            List<BezierPoints> result = new();
            for (int i = 2; i < points.Count; i += 2) {
                result.Add(
                    new BezierPoints {
                        Start = points[i - 2],
                        Mid = points[i - 1],
                        End = points[i],
                    }
                );
            }

            // string s = result.Aggregate("Bezier points: ", (current, p) => current + $"\n{p}");
            // Debug.Log(s);

            return result;
        }

        public Vector2 NoiseOffset;
        public MB_RiverGenerator RiverGenerator1;
        public MB_RiverGenerator RiverGenerator2;
        public MB_RiverGenerator RiverGenerator3;

        [ButtonMethod]
        public void Generate() {
            this.Points.Clear();
            List<Vector2> points = new() { this.RiverStart };

            int sectionCount = (int)((this.RiverEnd - this.RiverStart).magnitude / this.SectionSize);
            float sectionMaxDelta = this.SectionDelta / sectionCount;
            Vector2 direction = this.RiverEnd - this.RiverStart;
            Vector2 normal = new Vector2(direction.y, -direction.x).normalized;
            Vector2 noiseOffset = new(Mathf.Sqrt(this.NoiseOffset.x), Mathf.Sqrt(this.NoiseOffset.y));

            // Vector2 previousPoint = this.RiverStart;
            Vector2 noise = new(Random.Range(-1000, 1000), Random.Range(-1000, 1000));
            for (int i = 1; i < sectionCount; i++) {
                float ratio = i / (float)sectionCount;
                // ratio += Random.Range(-sectionMaxDelta, sectionMaxDelta);

                Vector2 point = this.RiverStart + direction * ratio;
                Vector2 offset = this.GetOffset(noise.x, noise.y, normal, this.SectionDrag);
                const float adjustRatio = .5f;
                offset = ratio switch {
                    < adjustRatio => Vector2.Lerp(offset, Vector2.zero, SC_Utils.MapFrom(0, adjustRatio, 1, 0, ratio)),
                    > 1 - adjustRatio => Vector2.Lerp(offset, Vector2.zero, SC_Utils.MapFrom(1 - adjustRatio, 1, 0, 1, ratio)),
                    _ => offset
                };
                points.Add(point + offset);
                //Vector2 midPoint = this.GetOffsettedMidPoint(previousPoint, point, normal, .5f, this.SectionDrag);

                // points.Add(this.GetOffsettedMidPoint(previousPoint, point, normal, .5f, this.SectionDrag));
                // points.Add(this.GetOffsettedMidPoint(previousPoint, point, normal, 1, this.SectionDrag));
                // previousPoint = point;

                noise += noiseOffset;
            }

            // points.Add(this.GetOffsettedMidPoint(previousPoint, this.RiverEnd, normal, .5f, this.SectionDrag));
            points.Add(this.RiverEnd);

            this.Points.Add(
                new C_RiverPoint {
                    Position = points[0],
                    Color = Color.green
                }
            );

            int[] forceDirections = new int[points.Count];
            for (int i = 1; i < points.Count - 1; i++) {
                int dot1 = (int)Mathf.Sign(Vector2.Dot(points[i + 1] - points[i], normal));
                int dot2 = (int)Mathf.Sign(Vector2.Dot(points[i - 1] - points[i], normal));
                forceDirections[i] = dot1 == dot2
                    ? -dot1
                    : 0;
            }

            for (int i = 1; i < points.Count; i++) {
                int subSectionCount = Random.Range(this.SubSectionCount.Min, this.SubSectionCount.Max);
                Vector2 sectionNormal = (points[i] - points[i - 1]).normalized;
                sectionNormal = new Vector2(sectionNormal.y, -sectionNormal.x);

                for (int j = 1; j < subSectionCount; j++) {
                    int forceDirection = 0;
                    if (j == 1) {
                        forceDirection = forceDirections[i - 1];
                    } else if (j == subSectionCount - 1) {
                        forceDirection = forceDirections[i];
                    }

                    Vector2 midPoint = this.GetOffsettedMidPoint(
                        points[i - 1],
                        points[i],
                        sectionNormal,
                        (float)j / subSectionCount,
                        this.SubSectionDrag,
                        forceDirection
                    );

                    this.Points.Add(
                        new C_RiverPoint {
                            Position = midPoint,
                            Color = Color.orange
                        }
                    );
                }

                this.Points.Add(
                    new C_RiverPoint {
                        Position = points[i],
                        Color = Color.green
                    }
                );
            }

            // this.Points.AddRange(newPoints);

            if (this.RiverGenerator1 != null) {
                this.RiverGenerator1.RiverStart = this.Points[this.Points.Count * 1 / 3].Position;
                this.RiverGenerator1.Generate();
            }

            if (this.RiverGenerator2 != null) {
                this.RiverGenerator2.RiverStart = this.Points[this.Points.Count * 2 / 3].Position;
                this.RiverGenerator2.Generate();
            }

            if (this.RiverGenerator3 != null) {
                this.RiverGenerator3.RiverStart = this.Points[this.Points.Count * 1 / 4].Position;
                this.RiverGenerator3.RiverEnd = this.RiverGenerator2.Points[this.RiverGenerator2.Points.Count * 3 / 4].Position;
                this.RiverGenerator3.Generate();
            }

            this.RiverTileMap.Clear();
            this.RiverTiles = new List<C_RiverTile>();
            foreach (BezierPoints bezierPoints in this.GetBezierPoints()) {
                for (int i = 0; i <= this.BezierDivisions; i++) {
                    Vector2 position = this.GetBezierPoint(
                        bezierPoints.Start,
                        bezierPoints.Mid,
                        bezierPoints.End,
                        (float)i / this.BezierDivisions
                    );
                    Vector2Int chunkPosition = new(Mathf.FloorToInt(position.x / MAP_WIDTH), Mathf.FloorToInt(position.y / MAP_HEIGHT));
                    Vector2Int intPosition = new(
                        Mathf.FloorToInt(position.x - chunkPosition.x * MAP_WIDTH),
                        Mathf.FloorToInt(position.y - chunkPosition.y * MAP_HEIGHT)
                    );
                    C_RiverTile riverTile = new() {
                        RealPosition = position,
                        Position = intPosition,
                        Chunk = chunkPosition,
                    };

                    this.RiverTiles.Add(riverTile);
                    this.RiverTileMap.TryAdd(chunkPosition, new List<C_RiverTile>());
                    this.RiverTileMap[chunkPosition].Add(riverTile);
                }
            }
        }

        private Vector2 GetOffsettedMidPoint(
            Vector2 previousPoint,
            Vector2 point,
            Vector2 normal,
            float ratio,
            float drag,
            int forceDirection = 0
        ) {
            Vector2 midPoint = previousPoint + (point - previousPoint) * ratio;
            int direction = forceDirection == 0
                ? SC_Utils.Rate(.5f)
                    ? 1
                    : -1
                : forceDirection;
            Mathf.PerlinNoise(1, 0);
            Vector2 offset = normal * Random.Range(0, drag) * direction;
            return midPoint + offset;
        }

        private Vector2 GetOffset(float noiseX, float noiseY, Vector2 normal, float size) {
            float noise = 2 * (Mathf.PerlinNoise(noiseX, noiseY) - .5f);
            return noise * size * normal;
        }

        private Vector2 GetBezierPoint(Vector2 p1, Vector2 p2, Vector2 p3, float t) {
            Vector2 controlP2 = p2; //2 * p2 - (p1 + p3) / 2;
            Vector2 interpolation1 = Vector2.Lerp(p1, controlP2, t);
            Vector2 interpolation2 = Vector2.Lerp(controlP2, p3, t);
            return Vector2.Lerp(interpolation1, interpolation2, t);
        }
    }
}
