using System.Collections.Generic;
using Code.Enhancements;
using Code.Utils;
using DG.Tweening;
using MyBox;
using UnityEngine;

namespace Code.UI.Notifications {
    public class MB_NotificationsContainer : MonoBehaviour {
        #region Members
        [Foldout("MB_NotificationsContainer", true)]
        [SerializeField] private protected MB_EnhancementNotification m_EnhancementNotification;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected CollectionWrapperList<AMB_Notification> m_VisibleNotifications = new();
        #endregion

        #region Getters / Setters
        private MB_EnhancementNotification EnhancementNotification { get => this.m_EnhancementNotification; }

        private CollectionWrapperList<AMB_Notification> VisibleNotifications { get => this.m_VisibleNotifications; }

        private readonly Dictionary<AMB_Notification, Tweener> YTweeners = new();
        #endregion

        #region Static / Readonly / Const
        private const float NOTIFICATION_MOVE_DURATION = .5f;
        private const float NOTIFICATION_SPACING = 4;
        private const float NOTIFICATION_DURATION = 3.5f;
        #endregion

        #region Unity methods
        #endregion

        public void Initialize() { }
        public void PostInitialize() { }

        public void CreateNotification(AMB_Enhancement enhancement) {
            MB_EnhancementNotification notification = Instantiate(this.EnhancementNotification, this.transform);
            notification.SetEnhancement(enhancement);
            notification.transform.localPosition = new Vector3(0, -50, 0);

            this.VisibleNotifications.Insert(0, notification);
            this.MoveNotifications();
            this.InSeconds(NOTIFICATION_DURATION, () => this.HideNotification(notification));
        }

        private void MoveNotifications() {
            float height = 0;
            foreach (AMB_Notification notification in this.VisibleNotifications.Value) {
                Debug.Log(height);
                this.MoveNotification(notification, height);
                height += NOTIFICATION_SPACING + notification.Height;
            }
        }

        private void MoveNotification(AMB_Notification notification, float height) {
            if (this.YTweeners.ContainsKey(notification) && this.YTweeners[notification] is { active: true })
                DOTween.Kill(this.YTweeners[notification]);

            this.YTweeners[notification] = DOTween.To( //
                    () => notification.transform.localPosition.y,
                    y => notification.transform.localPosition = new Vector3(
                        notification.transform.localPosition.x,
                        y,
                        notification.transform.localPosition.z
                    ),
                    height,
                    NOTIFICATION_MOVE_DURATION
                )
                .OnComplete(() => this.YTweeners[notification] = null);
        }

        private void HideNotification(AMB_Notification notification) {
            this.VisibleNotifications.Remove(notification);
            DOTween.To( //
                    () => notification.transform.localPosition.x,
                    x => notification.transform.localPosition = new Vector3(
                        x,
                        notification.transform.localPosition.y,
                        notification.transform.localPosition.z
                    ),
                    notification.Width + 20,
                    NOTIFICATION_MOVE_DURATION
                )
                .OnComplete(() => {
                        Destroy(notification.gameObject);
                    }
                );
        }
    }
}
