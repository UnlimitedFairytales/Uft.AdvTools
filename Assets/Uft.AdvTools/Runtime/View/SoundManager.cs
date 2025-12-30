using DG.Tweening;
using UnityEngine;

namespace Uft.AdvTools.View
{
    public class SoundManager : MonoBehaviour
    {
        // Parameters

        [SerializeField] AudioSource _audioBgm1;
        [SerializeField] AudioSource _audioBgm2;
        [SerializeField] AudioSource _audioSe;
        [SerializeField] AudioSource _audioVoice1;
        [SerializeField] AudioSource _audioVoice2;

        bool _currentBgmIsBgm1 = false;
        bool _currentVoiceIsVoice1 = false;

        public bool IsAnyVoicePlaying => this._audioVoice1.isPlaying || this._audioVoice2.isPlaying;

        public void ChangeBgm(AudioClip clip, bool isLoop, float volume, float prevFadeOutSeconds, float fadeInSeconds)
        {
            this._audioBgm1.DOComplete();
            this._audioBgm2.DOComplete();

            var ease = Ease.Linear;
            var delay = fadeInSeconds == 0.0f ? 0 : (prevFadeOutSeconds / 2.0f);
            if (this._currentBgmIsBgm1)
            {
                this._audioBgm2.clip = clip;
                this._audioBgm2.loop = isLoop;
                this._audioBgm2.time = 0;
                if (0.0f == fadeInSeconds)
                {
                    this._audioBgm2.volume = volume;
                    this._audioBgm2.Play();
                }
                else
                {
                    this._audioBgm2.volume = 0;
                    this._audioBgm2.Play();
                    this._audioBgm2.DOFade(volume, fadeInSeconds).SetEase(ease).SetDelay(delay);
                }
                this._audioBgm1.DOFade(0, prevFadeOutSeconds).SetEase(ease)
                    .OnComplete(() => this._audioBgm1.Stop());
            }
            else
            {
                this._audioBgm1.clip = clip;
                this._audioBgm1.loop = isLoop;
                this._audioBgm1.time = 0;
                if (0.0f == fadeInSeconds)
                {
                    this._audioBgm1.volume = volume;
                    this._audioBgm1.Play();
                }
                else
                {
                    this._audioBgm1.volume = 0;
                    this._audioBgm1.Play();
                    this._audioBgm1.DOFade(volume, fadeInSeconds).SetEase(ease).SetDelay(delay);
                }
                this._audioBgm2.DOFade(0, prevFadeOutSeconds).SetEase(ease)
                    .OnComplete(() => this._audioBgm2.Stop());
            }
            this._currentBgmIsBgm1 = !this._currentBgmIsBgm1;
        }

        public void StopBgm(float fadeOutSeconds)
        {
            this._audioBgm1.DOComplete();
            this._audioBgm2.DOComplete();

            var ease = Ease.Linear;
            if (this._currentBgmIsBgm1)
            {
                this._audioBgm1.DOFade(0, fadeOutSeconds).SetEase(ease)
                    .OnComplete(() => this._audioBgm1.Stop());
            }
            else
            {
                this._audioBgm2.DOFade(0, fadeOutSeconds).SetEase(ease)
                    .OnComplete(() => this._audioBgm2.Stop());
            }
        }

        public void PlayVoice(AudioClip clip, bool isLoop, float volume)
        {
            float prevFadeOutSeconds = 0.1f;
            float fadeInSeconds = 0f;
            this._audioVoice1.DOComplete();
            this._audioVoice2.DOComplete();

            var ease = Ease.Linear;
            var delay = fadeInSeconds == 0.0f ? 0 : (prevFadeOutSeconds / 2.0f);
            if (this._currentVoiceIsVoice1)
            {
                this._audioVoice2.clip = clip;
                this._audioVoice2.loop = isLoop;
                this._audioVoice2.time = 0;
                if (0.0f == fadeInSeconds)
                {
                    this._audioVoice2.volume = volume;
                    this._audioVoice2.Play();
                }
                else
                {
                    this._audioVoice2.volume = 0;
                    this._audioVoice2.Play();
                    this._audioVoice2.DOFade(volume, fadeInSeconds).SetEase(ease).SetDelay(delay);
                }
                this._audioVoice1.DOFade(0, prevFadeOutSeconds).SetEase(ease)
                    .OnComplete(() => this._audioVoice1.Stop());
            }
            else
            {
                this._audioVoice1.clip = clip;
                this._audioVoice1.loop = isLoop;
                this._audioVoice1.time = 0;
                if (0.0f == fadeInSeconds)
                {
                    this._audioVoice1.volume = volume;
                    this._audioVoice1.Play();
                }
                else
                {
                    this._audioVoice1.volume = 0;
                    this._audioVoice1.Play();
                    this._audioVoice1.DOFade(volume, fadeInSeconds).SetEase(ease).SetDelay(delay);
                }
                this._audioVoice2.DOFade(0, prevFadeOutSeconds).SetEase(ease)
                    .OnComplete(() => this._audioVoice2.Stop());
            }
            this._currentVoiceIsVoice1 = !this._currentVoiceIsVoice1;
        }

        public void StopVoice()
        {
            var fadeOutSeconds = 0.1f;
            this._audioVoice1.DOComplete();
            this._audioVoice2.DOComplete();

            var ease = Ease.Linear;
            if (this._currentVoiceIsVoice1)
            {
                this._audioVoice1.DOFade(0, fadeOutSeconds).SetEase(ease)
                    .OnComplete(() => this._audioVoice1.Stop());
            }
            else
            {
                this._audioVoice2.DOFade(0, fadeOutSeconds).SetEase(ease)
                    .OnComplete(() => this._audioVoice2.Stop());
            }
        }

        public void PlayOneShotSe(AudioClip clip, float volume)
        {
            this._audioSe.PlayOneShot(clip, volume);
        }
    }
}
