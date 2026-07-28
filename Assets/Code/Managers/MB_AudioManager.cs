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
        public enum BackgroundMusic {
            Unset,
            Battle1,
            Battle2,
            Battle3,
            Battle4,
            Boss1
        }

        #region Members
        [Foldout("MB_AudioManager", true)]
        [SerializeField] private protected AudioSource[] m_BackgroundMusicAudioSourceAudioSources;

        [Header("BGMs")]
        [SerializeField] private protected AudioResource m_BattleBGM1;
        [SerializeField] private protected AudioResource m_BattleBGM2;
        [SerializeField] private protected AudioResource m_BattleBGM3;
        [SerializeField] private protected AudioResource m_BattleBGM4;
        [SerializeField] private protected AudioResource m_BossBGM1;

        [Header("Player")]
        [SerializeField] private protected AudioResource[] m_PlayerHurt;
        [SerializeField] private protected AudioResource[] m_PlayerHurtFromDamageOverTime;
        [SerializeField] private protected AudioResource[] m_PlayerDash;
        [SerializeField] private protected AudioResource[] m_SwordSwing;

        [Header("Slime")]
        [SerializeField] private protected AudioResource[] m_SlimeJump;
        [SerializeField] private protected AudioResource[] m_SlimeLand;
        [SerializeField] private protected AudioResource[] m_SlimeHurt;

        [Header("Skeleton")]
        [SerializeField] private protected AudioResource[] m_SkeletonHurt;
        [SerializeField] private protected AudioResource[] m_SkeletonAttack;
        [SerializeField] private protected AudioResource[] m_SkeletonAttackMiss;
        [SerializeField] private protected AudioResource[] m_SkeletonFocusing;

        [Header("Zombie")]
        [SerializeField] private protected AudioResource[] m_ZombieHurt;
        [SerializeField] private protected AudioResource[] m_ZombieAttack;

        [Header("Ghost")]
        [SerializeField] private protected AudioResource[] m_GhostHurt;
        [SerializeField] private protected AudioResource[] m_GhostAttack;

        [Header("Bat")]
        [SerializeField] private protected AudioResource[] m_BatHurt;
        [SerializeField] private protected AudioResource[] m_BatScream;

        [Header("Spider")]
        [SerializeField] private protected AudioResource[] m_SpiderHurt;
        [SerializeField] private protected AudioResource[] m_SpiderWeb;

        [Header("Necromancer")]
        [SerializeField] private protected AudioResource[] m_NecromancerHurt;
        [SerializeField] private protected AudioResource[] m_NecromancerSummon;
        [SerializeField] private protected AudioResource[] m_NecromancerScream;

        [Header("Spawner")]
        [SerializeField] private protected AudioResource[] m_SpawnerStart;
        [SerializeField] private protected AudioResource[] m_SpawnerSpawn;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected CollectionWrapperList<AudioSource> m_SoundEffects;
        [ReadOnly][SerializeField] private protected BackgroundMusic m_CurrentBackgroundMusic;

        [ReadOnly][SerializeField][Range(0, 1)] private protected float m_BackgroundMusicVolume = 1;
        [ReadOnly][SerializeField][Range(0, 1)] private protected float m_SoundEffectsVolume = 1;
        [SerializeField][Range(0, 1)] private protected float m_CustomBackgroundMusicVolume = 1;
        [SerializeField][Range(0, 1)] private protected float m_CustomSoundEffectsVolume = 1;
        [SerializeField][Range(0, 1)] private protected float m_BaseBackgroundMusicVolume = 1;
        [SerializeField][Range(0, 1)] private protected float m_BaseSoundEffectsVolume = 1;
        #endregion

        #region Getters / Setters
        private AudioSource[] BackgroundMusicAudioSources { get => this.m_BackgroundMusicAudioSourceAudioSources; }

        private AudioResource BattleBGM1 { get => this.m_BattleBGM1; }
        private AudioResource BattleBGM2 { get => this.m_BattleBGM2; }
        private AudioResource BattleBGM3 { get => this.m_BattleBGM3; }
        private AudioResource BattleBGM4 { get => this.m_BattleBGM4; }
        private AudioResource BossBGM1 { get => this.m_BossBGM1; }

        private AudioResource[] PlayerHurt { get => this.m_PlayerHurt; }
        private AudioResource[] PlayerHurtFromDamageOverTime { get => this.m_PlayerHurtFromDamageOverTime; }
        private AudioResource[] PlayerDash { get => this.m_PlayerDash; }
        private AudioResource[] SwordSwing { get => this.m_SwordSwing; }

        private AudioResource[] SlimeJump { get => this.m_SlimeJump; }
        private AudioResource[] SlimeLand { get => this.m_SlimeLand; }
        private AudioResource[] SlimeHurt { get => this.m_SlimeHurt; }

        private AudioResource[] SkeletonHurt { get => this.m_SkeletonHurt; }
        private AudioResource[] SkeletonAttack { get => this.m_SkeletonAttack; }
        private AudioResource[] SkeletonAttackMiss { get => this.m_SkeletonAttackMiss; }
        private AudioResource[] SkeletonFocusing { get => this.m_SkeletonFocusing; }

        private AudioResource[] ZombieHurt { get => this.m_ZombieHurt; }
        private AudioResource[] ZombieAttack { get => this.m_ZombieAttack; }

        private AudioResource[] GhostHurt { get => this.m_GhostHurt; }
        private AudioResource[] GhostAttack { get => this.m_GhostAttack; }

        private AudioResource[] BatHurt { get => this.m_BatHurt; }
        private AudioResource[] BatScream { get => this.m_BatScream; }

        private AudioResource[] SpiderHurt { get => this.m_SpiderHurt; }
        private AudioResource[] SpiderWeb { get => this.m_SpiderWeb; }

        private AudioResource[] NecromancerHurt { get => this.m_NecromancerHurt; }
        private AudioResource[] NecromancerSummon { get => this.m_NecromancerSummon; }
        private AudioResource[] NecromancerScream { get => this.m_NecromancerScream; }

        private AudioResource[] SpawnerStart { get => this.m_SpawnerStart; }
        private AudioResource[] SpawnerSpawn { get => this.m_SpawnerSpawn; }

        private CollectionWrapperList<AudioSource> SoundEffects { get => this.m_SoundEffects; }
        private BackgroundMusic CurrentBackgroundMusic {
            get => this.m_CurrentBackgroundMusic;
            set => this.m_CurrentBackgroundMusic = value;
        }

        private float BackgroundMusicVolume { get => this.m_BackgroundMusicVolume; set => this.m_BackgroundMusicVolume = value; }
        private float SoundEffectsVolume { get => this.m_SoundEffectsVolume; set => this.m_SoundEffectsVolume = value; }
        private float CustomBackgroundMusicVolume {
            get => this.m_CustomBackgroundMusicVolume;
            set => this.m_CustomBackgroundMusicVolume = value;
        }
        private float CustomSoundEffectsVolume { get => this.m_CustomSoundEffectsVolume; set => this.m_CustomSoundEffectsVolume = value; }
        private float BaseBackgroundMusicVolume { get => this.m_BaseBackgroundMusicVolume; }
        private float BaseSoundEffectsVolume { get => this.m_BaseSoundEffectsVolume; }

        private int BackgroundMusicIndex { get; set; } = 0;
        private float[] BackgroundMusicVolumes { get; } = { 1f, 0f };
        private readonly Dictionary<ICollection<AudioResource>, float> LastPlayedAt = new();
        #endregion

        #region Static / Readonly / Const
        private Dictionary<BackgroundMusic, AudioResource> BackgroundMusicsCatalog { get; } = new();
        #endregion

        #region Unity methods
        private void FixedUpdate() {
            this.BackgroundMusicAudioSources[0].volume = this.BackgroundMusicVolumes[0]
                                                         * this.BackgroundMusicVolume
                                                         * this.CustomBackgroundMusicVolume
                                                         * this.BaseBackgroundMusicVolume;
            this.BackgroundMusicAudioSources[1].volume = this.BackgroundMusicVolumes[1]
                                                         * this.BackgroundMusicVolume
                                                         * this.CustomBackgroundMusicVolume
                                                         * this.BaseBackgroundMusicVolume;
        }
        #endregion

        public void Initialize() {
            this.BackgroundMusicsCatalog[BackgroundMusic.Battle1] = this.BattleBGM1;
            this.BackgroundMusicsCatalog[BackgroundMusic.Battle2] = this.BattleBGM2;
            this.BackgroundMusicsCatalog[BackgroundMusic.Battle3] = this.BattleBGM3;
            this.BackgroundMusicsCatalog[BackgroundMusic.Battle4] = this.BattleBGM4;
            this.BackgroundMusicsCatalog[BackgroundMusic.Boss1] = this.BossBGM1;

            this.CurrentBackgroundMusic = BackgroundMusic.Unset;
            this.ChangeBackgroundMusic(BackgroundMusic.Battle1, 0);
        }

        public void PostInitialize() { }

        public void SetBackgroundMusicVolume(float volume) => this.BackgroundMusicVolume = volume;
        public void SetSoundEffectsVolume(float volume) => this.SoundEffectsVolume = volume;
        public void SetCustomBackgroundMusicVolume(float volume) => this.CustomBackgroundMusicVolume = volume;
        public void SetCustomSoundEffectsVolume(float volume) => this.CustomSoundEffectsVolume = volume;

        public void ChangeBackgroundMusic(BackgroundMusic newBackgroundMusic, float fadeDuration) {
            if (newBackgroundMusic == this.CurrentBackgroundMusic) return;

            this.CurrentBackgroundMusic = newBackgroundMusic;
            this.ChangeBackgroundMusic(this.BackgroundMusicsCatalog[newBackgroundMusic], fadeDuration);
        }

        private void ChangeBackgroundMusic(AudioResource newBackgroundMusic, float fadeDuration) {
            int currentIndex = this.BackgroundMusicIndex;
            int newIndex = (currentIndex + 1) % 2;

            this.BackgroundMusicAudioSources[newIndex].Stop();
            this.BackgroundMusicAudioSources[newIndex].resource = newBackgroundMusic;
            this.BackgroundMusicAudioSources[newIndex].volume = 0;
            this.BackgroundMusicAudioSources[newIndex].Play();

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
                .OnComplete(this.BackgroundMusicAudioSources[currentIndex].Stop);
        }

        #region Background Music
        [ButtonMethod]
        public void SetBattleBGM1() => this.ChangeBackgroundMusic(BackgroundMusic.Battle1, 3f);

        [ButtonMethod]
        public void SetBattleBGM2() => this.ChangeBackgroundMusic(BackgroundMusic.Battle2, 3f);

        [ButtonMethod]
        public void SetBattleBGM3() => this.ChangeBackgroundMusic(BackgroundMusic.Battle3, 3f);

        [ButtonMethod]
        public void SetBattleBGM4() => this.ChangeBackgroundMusic(BackgroundMusic.Battle4, 3f);

        [ButtonMethod]
        public void SetBossBGM1() => this.ChangeBackgroundMusic(BackgroundMusic.Boss1, 3f);
        #endregion

        #region Sound Effects
        private void PlayPlayerHurt(float volume, float pitch) => this.PlaySoundEffect(this.PlayerHurt, volume, pitch);
        public void PlayPlayerHurt() => this.PlayPlayerHurt(volume: 1, pitch: 1);
        private void PlayPlayerHurtFromDamageOverTime(float volume, float pitch) =>
            this.PlaySoundEffect(this.PlayerHurtFromDamageOverTime, volume, pitch, .15f);
        public void PlayPlayerHurtFromDamageOverTime() => this.PlayPlayerHurtFromDamageOverTime(volume: 1, pitch: 1);
        private void PlayPlayerDash(float volume, float pitch) => this.PlaySoundEffect(this.PlayerDash, volume, pitch);
        public void PlayPlayerDash() => this.PlayPlayerDash(volume: 1, pitch: 1);
        private void PlaySwordSwing(float volume, float pitch) => this.PlaySoundEffect(this.SwordSwing, volume, pitch);
        public void PlaySwordSwing() => this.PlaySwordSwing(volume: 1, pitch: 1);

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
        private void PlaySkeletonFocusing(float volume, float pitch) => this.PlaySoundEffect(this.SkeletonFocusing, volume, pitch);
        public void PlaySkeletonFocusing() => this.PlaySkeletonFocusing(volume: 1, pitch: 1f);

        private void PlayZombieHurt(float volume, float pitch) => this.PlaySoundEffect(this.ZombieHurt, volume, pitch);
        public void PlayZombieHurt() => this.PlayZombieHurt(volume: 1, pitch: 1f);
        private void PlayZombieAttack(float volume, float pitch) => this.PlaySoundEffect(this.ZombieAttack, volume, pitch);
        public void PlayZombieAttack() => this.PlayZombieAttack(volume: 1, pitch: 1f);

        private void PlayGhostHurt(float volume, float pitch) => this.PlaySoundEffect(this.GhostHurt, volume, pitch);
        public void PlayGhostHurt() => this.PlayGhostHurt(volume: 1, pitch: 1f);
        private void PlayGhostAttack(float volume, float pitch) => this.PlaySoundEffect(this.GhostAttack, volume, pitch);
        public void PlayGhostAttack() => this.PlayGhostAttack(volume: 1, pitch: 1f);

        private void PlayBatHurt(float volume, float pitch) => this.PlaySoundEffect(this.BatHurt, volume, pitch);
        public void PlayBatHurt() => this.PlayBatHurt(volume: 1, pitch: 1f);
        private void PlayBatScream(float volume, float pitch) => this.PlaySoundEffect(this.BatScream, volume, pitch);
        public void PlayBatScream() => this.PlayBatScream(volume: 1, pitch: 1f);

        private void PlaySpiderHurt(float volume, float pitch) => this.PlaySoundEffect(this.SpiderHurt, volume, pitch);
        public void PlaySpiderHurt() => this.PlaySpiderHurt(volume: 1, pitch: 1f);
        private void PlaySpiderWeb(float volume, float pitch) => this.PlaySoundEffect(this.SpiderWeb, volume, pitch);
        public void PlaySpiderWeb() => this.PlaySpiderWeb(volume: 1, pitch: 1f);

        private void PlayNecromancerHurt(float volume, float pitch) => this.PlaySoundEffect(this.NecromancerHurt, volume, pitch);
        public void PlayNecromancerHurt() => this.PlayNecromancerHurt(volume: 1, pitch: 1f);
        private void PlayNecromancerSummon(float volume, float pitch) => this.PlaySoundEffect(this.NecromancerSummon, volume, pitch);
        public void PlayNecromancerSummon() => this.PlayNecromancerSummon(volume: 1, pitch: 1f);
        private void PlayNecromancerScream(float volume, float pitch) => this.PlaySoundEffect(this.NecromancerScream, volume, pitch);
        public void PlayNecromancerScream() => this.PlayNecromancerScream(volume: 1, pitch: 1f);

        private void PlaySpawnerStart(float volume, float pitch) => this.PlaySoundEffect(this.SpawnerStart, volume, pitch);
        public void PlaySpawnerStart() => this.PlaySpawnerStart(volume: 1, pitch: 1f);
        private void PlaySpawnerSpawn(float volume, float pitch) => this.PlaySoundEffect(this.SpawnerSpawn, volume, pitch);
        public void PlaySpawnerSpawn() => this.PlaySpawnerSpawn(volume: 1, pitch: 1f);

        private void PlaySoundEffect(ICollection<AudioResource> resources, float volume, float pitch, float maxLastPlayedInterval = .1f) {
            if (this.SoundEffectsVolume == 0 || resources.Count == 0) return;

            if (this.LastPlayedAt.TryGetValue(resources, out float lastPlayedAt) && Time.time - lastPlayedAt < maxLastPlayedInterval)
                return;

            this.LastPlayedAt[resources] = Time.time;

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
            audioSource.volume = volume * this.SoundEffectsVolume * this.CustomSoundEffectsVolume * this.BaseSoundEffectsVolume;
            audioSource.pitch = Random.Range(.925f, 1.075f) * pitch;
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
