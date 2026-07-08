using System;
using System.Collections.Generic;
using Code.Characters;
using Code.Characters.Effects;
using Code.Managers;
using Code.Utils;
using DG.Tweening;
using MyBox;
using UnityEngine;

namespace Code.Spells.Enemies {
    public class MB_NecromancerScream : AMB_PositionalSpell {
        private enum E_Effect {
            Confused,
            Poison,
            Blind,
            ShortSighted,
            Burning
        }

        #region Members
        [Foldout("MB_NecromancerScream", true)]
        [SerializeField] private protected float m_Size;
        [SerializeField] private protected CircleCollider2D m_InnerCircleCollider;
        [SerializeField] private protected CircleCollider2D m_OuterCircleCollider;

        [SerializeField] private protected List<C_WeightedObject<int>> m_EffectsCount;
        [SerializeField] private protected float m_EffectsDuration;

        [Separator("Confused")]
        [SerializeField] private protected MB_Confused m_ConfusedPrefab;

        [Separator("Poison")]
        [SerializeField] private protected MB_Poison m_PoisonPrefab;
        [SerializeField] private protected float m_PoisonDamagePerTick;
        [SerializeField] private protected int m_PoisonTickCount;

        [Separator("Blind")]
        [SerializeField] private protected MB_Blind m_BlindPrefab;
        [SerializeField] private protected float m_BlindFOVSize;

        [Separator("Short sighted")]
        [SerializeField] private protected MB_ShortSighted m_ShortSightedPrefab;
        [SerializeField] private protected int m_ShortSightedSightRatio;

        [Separator("Burning")]
        [SerializeField] private protected MB_Burning m_BurningPrefab;
        [SerializeField] private protected float m_BurningDamagePerTick;
        [SerializeField] private protected int m_BurningTickCount;
        #endregion

        #region Getters / Setters
        private float Size { get => this.m_Size; }
        private CircleCollider2D InnerCircleCollider { get => this.m_InnerCircleCollider; }
        private CircleCollider2D OuterCircleCollider { get => this.m_OuterCircleCollider; }

        private List<C_WeightedObject<int>> EffectsCount { get => this.m_EffectsCount; }
        private float EffectsDuration { get => this.m_EffectsDuration; }

        private MB_Confused ConfusedPrefab { get => this.m_ConfusedPrefab; }

        private MB_Poison PoisonPrefab { get => this.m_PoisonPrefab; }
        private float PoisonDamagePerTick { get => this.m_PoisonDamagePerTick; }
        private int PoisonTickCount { get => this.m_PoisonTickCount; }

        private MB_Blind BlindPrefab { get => this.m_BlindPrefab; }
        private float BlindFOVSize { get => this.m_BlindFOVSize; }

        private MB_ShortSighted ShortSightedPrefab { get => this.m_ShortSightedPrefab; }
        private int ShortSightedSightRatio { get => this.m_ShortSightedSightRatio; }

        private MB_Burning BurningPrefab { get => this.m_BurningPrefab; }
        private float BurningDamagePerTick { get => this.m_BurningDamagePerTick; }
        private int BurningTickCount { get => this.m_BurningTickCount; }
        #endregion

        #region Static / Readonly / Const
        private const float SHOCKWAVE_DURATION = 5f;
        #endregion

        #region Unity methods
        protected override void Start() {
            base.Start();

            // Wait for next update because initialization is done after instantiating
            this.InUpdates(
                1,
                () => {
                    this.ObjectsManager.ShockWavesManager.PerformShockWave(
                        worldPosition: this.To,
                        duration: SHOCKWAVE_DURATION,
                        strength: 5f,
                        maxSize: this.Size
                    );
                    DOTween.To( //
                            () => 0,
                            size => this.OuterCircleCollider.radius = size,
                            this.Size,
                            SHOCKWAVE_DURATION
                        )
                        .OnComplete(() => this.OuterCircleCollider.radius = 0)
                        .SetEase(MB_ShockWavesManager.EASE);
                    DOTween.To( //
                            () => 0f,
                            ratio => this.InnerCircleCollider.radius = ratio * this.OuterCircleCollider.radius,
                            1f,
                            SHOCKWAVE_DURATION
                        )
                        .OnComplete(() => this.InnerCircleCollider.radius = 0)
                        .SetEase(MB_ShockWavesManager.EASE);
                    this.InSeconds(SHOCKWAVE_DURATION * .8f, () => this.InnerCircleCollider.enabled = false);
                    this.InSeconds(SHOCKWAVE_DURATION * .8f, () => this.OuterCircleCollider.enabled = false);
                    this.InSeconds(SHOCKWAVE_DURATION, () => this.Destroyer.Destroy());
                }
            );
        }
        #endregion

        public override void TriggerEnter(AMB_Character character) {
            E_SpellCollisionFlag flag = base.Collide(character);
            if (flag is not E_SpellCollisionFlag.Character and not E_SpellCollisionFlag.CharacterInvulnerable) return;

            character.Knockback(character.transform.position.ToVector2() - this.To, this.HitMetadata.KnockbackForce);

            int effectsCount = SC_Utils.Sample(this.EffectsCount).Obj;
            List<E_Effect> selectedEffects = SC_Utils.Sample(
                new List<E_Effect> {
                    E_Effect.Confused,
                    E_Effect.Poison,
                    E_Effect.Blind,
                    E_Effect.ShortSighted,
                    E_Effect.Burning
                },
                effectsCount
            );

            foreach (E_Effect selectedEffect in selectedEffects) {
                switch (selectedEffect) {
                    case E_Effect.Confused:
                        character.AddEffect(this.ConfusedPrefab, this.Character, this.EffectsDuration);
                        break;
                    case E_Effect.Poison:
                        MB_Poison poison = Instantiate(this.PoisonPrefab);
                        poison.SetDamage(this.PoisonDamagePerTick, this.PoisonTickCount);
                        character.AddEffect(poison, this.Character);
                        break;
                    case E_Effect.Blind:
                        MB_Blind blind = Instantiate(this.BlindPrefab);
                        blind.SetFOVSize(this.BlindFOVSize);
                        character.AddEffect(blind, this.Character, this.EffectsDuration);
                        break;
                    case E_Effect.ShortSighted:
                        MB_ShortSighted shortSighted = Instantiate(this.ShortSightedPrefab);
                        shortSighted.SetSightRatio(this.ShortSightedSightRatio);
                        character.AddEffect(shortSighted, this.Character, this.EffectsDuration);
                        break;
                    case E_Effect.Burning:
                        MB_Burning burning = Instantiate(this.BurningPrefab);
                        burning.SetDamage(this.BurningDamagePerTick, this.BurningTickCount);
                        character.AddEffect(burning, this.Character);
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override void TriggerStay(AMB_Character character) { }

        public override void TriggerExit(AMB_Character character) { }
    }
}
