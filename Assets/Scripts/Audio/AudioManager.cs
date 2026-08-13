using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Settings")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource pitchSoundSource;
        [SerializeField] private AudioSource normalSoundSource;
        [SerializeField] private AudioMixer audioMixer;
        
        [SerializeField] private AudioClip gameMusic;
        [SerializeField] private AudioClip menuMusic;
        [SerializeField] private AudioClip click;
        [SerializeField] private AudioClip deselect;
        [SerializeField] private AudioClip match;
        [SerializeField] private AudioClip noMatch;
        [SerializeField] private AudioClip whoosh;
        [SerializeField] private AudioClip pop;
        [SerializeField] private AudioClip stopMusic;
        [SerializeField] private AudioClip remove;
        [SerializeField] private AudioClip win;
        [SerializeField] private AudioClip loose;
        [SerializeField] private AudioClip explosion;
        
        private bool _isEnabledSound = true;

        public void StopAllSounds()
        {
            normalSoundSource.Stop();
            pitchSoundSource.Stop();
            musicSource.Stop();
        }
        
        public void PlayClick() => PlayNormalPitch(click);
        public void PlayDeselect() => PlayNormalPitch(deselect);
        public void PlayMatch() => PlayNormalPitch(match);
        public void PlayNoMatch() => PlayNormalPitch(noMatch);
        public void PlayExplosion() => PlayNormalPitch(explosion);
        public void PlayWhoosh() => PlayRandomPitch(whoosh);
        public void PlayPop() => PlayRandomPitch(pop);
        public void PlayStopMusic() => PlayNormalPitch(stopMusic);
        public void PlayRemove() => PlayNormalPitch(remove);
        public void PlayWin() => PlayNormalPitch(win);
        public void PlayLoose() => PlayNormalPitch(loose);
        
        public void SetSoundVolume()
        {
            if (_isEnabledSound)
            {
                audioMixer.SetFloat("Volume", -6f);
                musicSource.Play();
            }
            else 
            {
                audioMixer.SetFloat("Volume", -80f);
                musicSource.Stop();
            }
        }

        public void PlayGameMusic()
        {
            StopMusic();
            musicSource.clip = gameMusic;
            SetSoundVolume();
        }

        public void PlayMenuMusic()
        {
            StopMusic();
            musicSource.clip = menuMusic;
            SetSoundVolume();
        }
        
        public void StopMusic() => musicSource.Stop();

        private void PlayRandomPitch(AudioClip clip)
        {
            pitchSoundSource.pitch = Random.Range(0.8f, 1.2f);
            pitchSoundSource.PlayOneShot(clip);
        }
        
        private void PlayNormalPitch(AudioClip clip) => normalSoundSource.PlayOneShot(clip);

    }
}