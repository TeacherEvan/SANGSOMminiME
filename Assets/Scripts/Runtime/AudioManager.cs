using UnityEngine;

namespace SangsomMiniMe.Core
{
    /// <summary>
    /// Centralized audio management system for sound effects and music.
    /// Provides volume control, sound effect playback, and audio state persistence.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource musicSource;

        [Header("Sound Effects")]
        [SerializeField] private AudioClip loginBonusChime;
        [SerializeField] private AudioClip milestoneSparkle;
        [SerializeField] private AudioClip coinSound;
        [SerializeField] private AudioClip happinessChime;
        [SerializeField] private AudioClip feedSound;
        [SerializeField] private AudioClip restSound;
        [SerializeField] private AudioClip playSound;
        [SerializeField] private AudioClip buttonClick;
        [SerializeField] private AudioClip gentleReminder;

        [Header("Volume Settings")]
        [SerializeField][Range(0f, 1f)] private float sfxVolume = 0.7f;
        [SerializeField][Range(0f, 1f)] private float musicVolume = 0.5f;

        // Singleton instance
        private static AudioManager instance;
        public static AudioManager Instance => instance;

        private void Awake()
        {
            // Singleton pattern
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudio();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeAudio()
        {
            // Create AudioSources if not assigned
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }

            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.playOnAwake = false;
                musicSource.loop = true;
            }

            // Load saved volume settings
            LoadVolumeSettings();
        }

        /// <summary>
        /// Play login bonus chime sound
        /// </summary>
        public void PlayLoginBonus()
        {
            PlaySFX(loginBonusChime);
        }

        /// <summary>
        /// Play milestone celebration sparkle sound
        /// </summary>
        public void PlayMilestone()
        {
            PlaySFX(milestoneSparkle);
        }

        /// <summary>
        /// Play coin reward sound
        /// </summary>
        public void PlayCoin()
        {
            PlaySFX(coinSound);
        }

        /// <summary>
        /// Play happiness increase sound
        /// </summary>
        public void PlayHappiness()
        {
            PlaySFX(happinessChime);
        }

        /// <summary>
        /// Play feed action sound
        /// </summary>
        public void PlayFeed()
        {
            PlaySFX(feedSound);
        }

        /// <summary>
        /// Play rest action sound
        /// </summary>
        public void PlayRest()
        {
            PlaySFX(restSound);
        }

        /// <summary>
        /// Play play action sound
        /// </summary>
        public void PlayPlay()
        {
            PlaySFX(playSound);
        }

        /// <summary>
        /// Play button click sound
        /// </summary>
        public void PlayButtonClick()
        {
            PlaySFX(buttonClick);
        }

        /// <summary>
        /// Play gentle reminder sound
        /// </summary>
        public void PlayGentleReminder()
        {
            PlaySFX(gentleReminder);
        }

        /// <summary>
        /// Play a sound effect with volume control
        /// </summary>
        private void PlaySFX(AudioClip clip)
        {
            if (clip != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(clip, sfxVolume);
            }
        }

        /// <summary>
        /// Set SFX volume and persist to PlayerPrefs
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            if (sfxSource != null)
            {
                sfxSource.volume = sfxVolume;
            }
            PlayerPrefs.SetFloat(GameConstants.PlayerPrefsKeys.SfxVolume, sfxVolume);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Set music volume and persist to PlayerPrefs
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            if (musicSource != null)
            {
                musicSource.volume = musicVolume;
            }
            PlayerPrefs.SetFloat(GameConstants.PlayerPrefsKeys.MusicVolume, musicVolume);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Get current SFX volume
        /// </summary>
        public float GetSFXVolume()
        {
            return sfxVolume;
        }

        /// <summary>
        /// Get current music volume
        /// </summary>
        public float GetMusicVolume()
        {
            return musicVolume;
        }

        /// <summary>
        /// Load volume settings from PlayerPrefs
        /// </summary>
        private void LoadVolumeSettings()
        {
            if (PlayerPrefs.HasKey(GameConstants.PlayerPrefsKeys.SfxVolume))
            {
                sfxVolume = PlayerPrefs.GetFloat(GameConstants.PlayerPrefsKeys.SfxVolume);
            }

            if (PlayerPrefs.HasKey(GameConstants.PlayerPrefsKeys.MusicVolume))
            {
                musicVolume = PlayerPrefs.GetFloat(GameConstants.PlayerPrefsKeys.MusicVolume);
            }

            if (sfxSource != null)
            {
                sfxSource.volume = sfxVolume;
            }

            if (musicSource != null)
            {
                musicSource.volume = musicVolume;
            }
        }

        /// <summary>
        /// Play background music
        /// </summary>
        public void PlayMusic(AudioClip clip)
        {
            if (clip != null && musicSource != null)
            {
                musicSource.clip = clip;
                musicSource.Play();
            }
        }

        /// <summary>
        /// Stop background music
        /// </summary>
        public void StopMusic()
        {
            if (musicSource != null)
            {
                musicSource.Stop();
            }
        }

        /// <summary>
        /// Pause background music
        /// </summary>
        public void PauseMusic()
        {
            if (musicSource != null)
            {
                musicSource.Pause();
            }
        }

        /// <summary>
        /// Resume background music
        /// </summary>
        public void ResumeMusic()
        {
            if (musicSource != null)
            {
                musicSource.UnPause();
            }
        }
    }
}
