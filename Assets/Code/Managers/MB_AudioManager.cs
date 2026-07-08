using System;
using System.Collections.Generic;
using Code.Utils;
using DG.Tweening;
using MyBox;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Code.Managers {
    public class MB_AudioManager : MonoBehaviour {
        #region Members
        [Foldout("MB_AudioManager", true)]
        [SerializeField] private protected AudioSource[] m_BackgroundMusics;

        [SerializeField] private protected AudioResource m_BGM1;
        [SerializeField] private protected AudioResource m_BGM2;
        [SerializeField] private protected AudioResource m_BGM3;

        [SerializeField] private protected AudioResource[] m_PlayerHurt;
        [SerializeField] private protected AudioResource[] m_SwordSlash;

        [SerializeField] private protected AudioResource[] m_SlimeJump;
        [SerializeField] private protected AudioResource[] m_SlimeLand;
        [SerializeField] private protected AudioResource[] m_SlimeHurt;

        [SerializeField] private protected AudioResource[] m_SkeletonHurt;
        [SerializeField] private protected AudioResource[] m_SkeletonAttack;
        [SerializeField] private protected AudioResource[] m_SkeletonAttackMiss;

        [SerializeField] private protected AudioResource[] m_ZombieHurt;
        [SerializeField] private protected AudioResource[] m_ZombieAttack;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected CollectionWrapperList<AudioSource> m_SoundEffects;

        [ReadOnly][SerializeField][Range(0, 1)] private protected float m_BackgroundMusicVolume = 1;
        [ReadOnly][SerializeField][Range(0, 1)] private protected float m_SoundEffectsVolume = 1;
        [ReadOnly][SerializeField][Range(0, 1)] private protected float m_CustomBackgroundMusicVolume = 1;
        [ReadOnly][SerializeField][Range(0, 1)] private protected float m_CustomSoundEffectsVolume = 1;
        #endregion

        #region Getters / Setters
        private AudioSource[] BackgroundMusics { get => this.m_BackgroundMusics; }

        private AudioResource BGM1 { get => this.m_BGM1; }
        private AudioResource BGM2 { get => this.m_BGM2; }
        private AudioResource BGM3 { get => this.m_BGM3; }

        private AudioResource[] PlayerHurt { get => this.m_PlayerHurt; }
        private AudioResource[] SwordSlash { get => this.m_SwordSlash; }

        private AudioResource[] SlimeJump { get => this.m_SlimeJump; }
        private AudioResource[] SlimeLand { get => this.m_SlimeLand; }
        private AudioResource[] SlimeHurt { get => this.m_SlimeHurt; }

        private AudioResource[] SkeletonHurt { get => this.m_SkeletonHurt; }
        private AudioResource[] SkeletonAttack { get => this.m_SkeletonAttack; }
        private AudioResource[] SkeletonAttackMiss { get => this.m_SkeletonAttackMiss; }

        private AudioResource[] ZombieHurt { get => this.m_ZombieHurt; }
        private AudioResource[] ZombieAttack { get => this.m_ZombieAttack; }

        private CollectionWrapperList<AudioSource> SoundEffects { get => this.m_SoundEffects; }

        private float BackgroundMusicVolume { get => this.m_BackgroundMusicVolume; set => this.m_BackgroundMusicVolume = value; }
        private float SoundEffectsVolume { get => this.m_SoundEffectsVolume; set => this.m_SoundEffectsVolume = value; }
        private float CustomBackgroundMusicVolume {
            get => this.m_CustomBackgroundMusicVolume;
            set => this.m_CustomBackgroundMusicVolume = value;
        }
        private float CustomSoundEffectsVolume { get => this.m_CustomSoundEffectsVolume; set => this.m_CustomSoundEffectsVolume = value; }

        private int BackgroundMusicIndex { get; set; } = 0;
        private float[] BackgroundMusicVolumes { get; } = { 1f, 0f };
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        private void FixedUpdate() {
            this.BackgroundMusics[0].volume =
                this.BackgroundMusicVolumes[0] * this.BackgroundMusicVolume * this.CustomBackgroundMusicVolume;
            this.BackgroundMusics[1].volume =
                this.BackgroundMusicVolumes[1] * this.BackgroundMusicVolume * this.CustomBackgroundMusicVolume;
        }
        #endregion

        public void Initialize() { }

        public void PostInitialize() { }

        public void SetBackgroundMusicVolume(float volume) => this.BackgroundMusicVolume = volume;
        public void SetSoundEffectsVolume(float volume) => this.SoundEffectsVolume = volume;
        public void SetCustomBackgroundMusicVolume(float volume) => this.CustomBackgroundMusicVolume = volume;
        public void SetCustomSoundEffectsVolume(float volume) => this.CustomSoundEffectsVolume = volume;

        private void ChangeBackgroundMusic(AudioResource newBackgroundMusic, float fadeDuration) {
            int currentIndex = this.BackgroundMusicIndex;
            int newIndex = (currentIndex + 1) % 2;

            this.BackgroundMusics[newIndex].Stop();
            this.BackgroundMusics[newIndex].resource = newBackgroundMusic;
            this.BackgroundMusics[newIndex].volume = 0;
            this.BackgroundMusics[newIndex].Play();

            this.BackgroundMusicIndex = (this.BackgroundMusicIndex + 1) % 2;

            DOTween.To( //
                () => 0f,
                volume => this.BackgroundMusicVolumes[newIndex] = volume,
                1f,
                fadeDuration
            );
            DOTween.To( //
                    () => 1f,
                    volume => this.BackgroundMusicVolumes[currentIndex] = volume,
                    0f,
                    fadeDuration
                )
                .OnComplete(this.BackgroundMusics[currentIndex].Stop);
        }

        #region Background Music
        [ButtonMethod]
        public void SetBGM1() => this.ChangeBackgroundMusic(this.BGM1, 3f);

        [ButtonMethod]
        public void SetBGM2() => this.ChangeBackgroundMusic(this.BGM2, 3f);

        [ButtonMethod]
        public void SetBGM3() => this.ChangeBackgroundMusic(this.BGM3, 3f);
        #endregion

        #region Sound Effects
        private void PlayPlayerHurt(float volume, float pitch) => this.PlaySoundEffect(this.PlayerHurt, volume, pitch);
        public void PlayPlayerHurt() => this.PlayPlayerHurt(volume: 1, pitch: 1);
        private void PlaySwordSlash(float volume, float pitch) => this.PlaySoundEffect(this.SwordSlash, volume, pitch);
        public void PlaySwordSlash() => this.PlaySwordSlash(volume: 1, pitch: 1);
        
        private void PlaySlimeJump(float volume, float pitch) => this.PlaySoundEffect(this.SlimeJump, volume, pitch);
        public void PlaySlimeJump() => this.PlaySlimeJump(volume: 1, pitch: 1);
        private void PlaySlimeLand(float volume, float pitch) => this.PlaySoundEffect(this.SlimeLand, volume, pitch);
        public void PlaySlimeLand() => this.PlaySlimeLand(volume: 1, pitch: 1);
        private void PlaySlimeHurt(float volume, float pitch) => this.PlaySoundEffect(this.SlimeHurt, volume, pitch);
        public void PlaySlimeHurt() => this.PlaySlimeHurt(volume: 1, pitch: 1);

        private void PlaySkeletonHurt(float volume, float pitch) => this.PlaySoundEffect(this.SkeletonHurt, volume, pitch);
        public void PlaySkeletonHurt() => this.PlaySkeletonHurt(volume: 1, pitch: 1);
        private void PlaySkeletonAttack(float volume, float pitch) => this.PlaySoundEffect(this.SkeletonAttack, volume, pitch);
        public void PlaySkeletonAttack() => this.PlaySkeletonAttack(volume: 1, pitch: 1);
        private void PlaySkeletonAttackMiss(float volume, float pitch) => this.PlaySoundEffect(this.SkeletonAttackMiss, volume, pitch);
        public void PlaySkeletonAttackMiss() => this.PlaySkeletonAttackMiss(volume: 1, pitch: 1);

        private void PlayZombieHurt(float volume, float pitch) => this.PlaySoundEffect(this.ZombieHurt, volume, pitch);
        public void PlayZombieHurt() => this.PlayZombieHurt(volume: 1, pitch: 1);
        private void PlayZombieAttack(float volume, float pitch) => this.PlaySoundEffect(this.ZombieAttack, volume, pitch);
        public void PlayZombieAttack() => this.PlayZombieAttack(volume: 1, pitch: 1);

        private void PlaySoundEffect(ICollection<AudioResource> resources, float volume, float pitch) {
            if (this.SoundEffectsVolume == 0) return;

            AudioSource[] sources = this.GetComponentsInChildren<AudioSource>();
            if (sources.Length >= 20) {
                Debug.Log("Too many audio sources");
                return;
            }

            AudioResource resource = SC_Utils.Sample(resources);
            AudioSource audioSource = this.AddComponent<AudioSource>();
            this.SoundEffects.Add(audioSource);
            audioSource.resource = resource;
            audioSource.loop = false;
            audioSource.volume = volume * this.SoundEffectsVolume * this.CustomSoundEffectsVolume;
            audioSource.pitch = Random.Range(.95f, 1.05f) * pitch;
            audioSource.Play();
            this.InRealSeconds(
                audioSource.clip.length / Mathf.Abs(audioSource.pitch),
                () => {
                    this.SoundEffects.Remove(audioSource);
                    Destroy(audioSource);
                }
            );
        }
        #endregion
    }
}
