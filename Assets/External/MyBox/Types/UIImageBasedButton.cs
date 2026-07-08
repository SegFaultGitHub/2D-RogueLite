using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyBox {
    [RequireComponent(typeof(Button), typeof(Image))]
    public class UIImageBasedButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler {
#pragma warning disable 0649
        [SerializeField, MustBeAssigned] private Sprite _regularSprite;
        [SerializeField, MustBeAssigned] private Sprite _regularSelectedSprite;
        [SerializeField, MustBeAssigned] private Sprite _clickedSprite;
        [SerializeField, MustBeAssigned] private Sprite _clickedSelectedSprite;
#pragma warning restore 0649

        public Action<bool> OnToggled;

        public bool AlternativeSpriteset { get => this._alternative; set => this._alternative = value; }

        private bool _alternative;
        private bool _selected;
        private Image _image;
        private Button _button;


        private void Awake() {
            this._image = this.GetComponent<Image>();
            this._button = this.GetComponent<Button>();
        }

        private void OnEnable() => this._button.onClick.AddListener(this.ToggleSprites);
        private void OnDisable() => this._button.onClick.RemoveListener(this.ToggleSprites);

        private void ToggleSprites() {
            this._alternative = !this._alternative;
            this.OnToggled?.Invoke(this._alternative);

            this.UpdateSprites();
        }


        private void UpdateSprites() {
            this._image.sprite = !this._alternative
                ? !this._selected
                    ? this._regularSprite
                    : this._regularSelectedSprite
                : !this._selected
                    ? this._clickedSprite
                    : this._clickedSelectedSprite;
        }

        private void UpdateSprites(bool selected) {
            this._selected = selected;
            this.UpdateSprites();
        }


        public void OnSelect(BaseEventData eventData) {
            this.UpdateSprites(true);
        }

        public void OnDeselect(BaseEventData eventData) {
            this.UpdateSprites(false);
        }

        public void OnPointerEnter(PointerEventData eventData) {
            this.UpdateSprites(true);
        }

        public void OnPointerExit(PointerEventData eventData) {
            this.UpdateSprites(false);
        }
    }
}
