using Code.Characters;
using Code.Utils;
using MyBox;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.UI.Damage {
    public class MB_DamageCanvas : MonoBehaviour {
        #region Members
        [Foldout("MB_DamageCanvas", true)]
        [SerializeField] private protected MB_Damage m_Damage;
        #endregion

        #region Getters / Setters
        private MB_Damage Damage { get => this.m_Damage; }
        #endregion

        #region Unity methods
        #endregion

        public void Initialize() { }

        public void PostInitialize() { }

        public void DamageDealt(AMB_Character character, float value, bool critical) {
            if (value == 0) return;

            MB_Damage damage = Instantiate(this.Damage, this.transform);
            damage.SetDamage(value, critical);

            Vector2 offset = SC_Utils.Rotate(new Vector2(0, Random.Range(1f, 1.25f)), Random.Range(-30, 30));
            damage.transform.position = character.Center.position.ToVector2() + offset;
        }

        public void Dodge(AMB_Character character) {
            MB_Damage damage = Instantiate(this.Damage, this.transform);
            damage.Dodge();

            Vector2 offset = SC_Utils.Rotate(new Vector2(0, Random.Range(1f, 1.25f)), Random.Range(-30, 30));
            damage.transform.position = character.Center.position.ToVector2() + offset;
        }

        public void Heal(AMB_Character character, float value) {
            if (value == 0) return;

            MB_Damage damage = Instantiate(this.Damage, this.transform);
            damage.SetHeal(value);

            Vector2 offset = SC_Utils.Rotate(new Vector2(0, Random.Range(1f, 1.25f)), Random.Range(-30, 30));
            damage.transform.position = character.Center.position.ToVector2() + offset;
        }

        public void DestroyAll() {
            for (int i = this.transform.childCount - 1; i >= 0; i--)
                Destroy(this.transform.GetChild(i).gameObject);
        }
    }
}
