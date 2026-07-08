using System;
using System.Collections.Generic;
using Code.Cameras;
using Code.Characters.Enemies;
using Code.UI.HUD;
using Code.Utils;
using MyBox;
using UnityEngine;

namespace Code.Managers {
    public class MB_EnemyIndicatorsManager : MonoBehaviour {
        #region Members
        [Foldout("MB_EnemyIndicatorsManager", true)]
        [SerializeField] private protected MB_EnemyIndicator m_EnemyIndicatorPrefab;
        [SerializeField] private protected Transform m_EnemyIndicatorsParent;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;
        #endregion

        #region Getters / Setters
        private MB_EnemyIndicator EnemyIndicatorPrefab { get => this.m_EnemyIndicatorPrefab; }
        private Transform EnemyIndicatorsParent { get => this.m_EnemyIndicatorsParent; }

        public MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; set => this.m_ObjectsManager = value; }

        private Dictionary<AMB_Enemy, MB_EnemyIndicator> EnemyIndicators { get; } = new();
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        private void Update() {
            Vector2 cameraPosition = this.ObjectsManager.MainCamera.transform.position.ToVector2();
            RectTransform rect = this.EnemyIndicatorsParent.GetComponent<RectTransform>();

            foreach ((AMB_Enemy enemy, MB_EnemyIndicator enemyIndicator) in this.EnemyIndicators) {
                if (enemy == null) continue;

                Vector2 enemyPosition = enemy.transform.position.ToVector2();
                Vector2 shaderPosition = this.ObjectsManager.MainCamera.GetShaderPosition(enemyPosition);
                enemyIndicator.gameObject.SetActive(
                    (shaderPosition.x < 0 || shaderPosition.x > 1 || shaderPosition.y < 0 || shaderPosition.y > 1)
                    && !this.ObjectsManager.Player.VisualEffects.IsBlind
                );
                (Vector2 position, float angle) = GetCoordinates(
                    rect.rect.width,
                    rect.rect.height,
                    enemyPosition - cameraPosition
                );
                enemyIndicator.transform.localPosition = position;
                enemyIndicator.SetAngle(angle);
                // enemyIndicator.SetDirection(enemyPosition - playerPosition);
                // enemyIndicator.SetDistance(this.ObjectsManager.MainCamera.Camera.orthographicSize * .85f);
            }
        }
        #endregion

        public void Initialize() { }

        public void PostInitialize() { }

        public void Register(AMB_Enemy enemy) {
            MB_EnemyIndicator enemyIndicator = Instantiate(this.EnemyIndicatorPrefab, this.EnemyIndicatorsParent);
            this.EnemyIndicators.TryAdd(enemy, enemyIndicator);
        }

        public void Unregister(AMB_Enemy enemy) {
            if (this.EnemyIndicators.TryGetValue(enemy, out MB_EnemyIndicator enemyIndicator)) {
                Destroy(enemyIndicator.gameObject);
            }
        }

        private static (Vector2, float) GetCoordinates(float width, float height, Vector2 direction) {
            float angleDeg = Vector2.SignedAngle(Vector2.right, direction);
            float angleRad = angleDeg * Mathf.Deg2Rad;
            float dx = Mathf.Cos(angleRad);
            float dy = Mathf.Sin(angleRad);

            float hw = width / 2f;
            float hh = height / 2f;

            float tx = hw / Mathf.Abs(dx);
            float ty = hh / Mathf.Abs(dy);

            float t = Mathf.Min(tx, ty);

            return (new Vector2(dx * t, dy * t), angleDeg);
        }
    }
}
