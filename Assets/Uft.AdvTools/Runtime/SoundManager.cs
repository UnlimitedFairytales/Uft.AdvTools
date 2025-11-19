using DG.Tweening;
using UnityEngine;

namespace Uft.AdvTools
{
    public class SoundManager : MonoBehaviour
    {
        // Parameters

        [SerializeField] AudioSource _audioBgm1;
        [SerializeField] AudioSource _audioBgm2;
        [SerializeField] AudioSource _audioSe;
        [SerializeField] AudioSource _audioVoice;

        bool _currentBgmIsBgm1 = false;

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

        public void PlayOneShotSe(AudioClip clip, float volume)
        {
            this._audioSe.PlayOneShot(clip, volume);
        }
    }
}
