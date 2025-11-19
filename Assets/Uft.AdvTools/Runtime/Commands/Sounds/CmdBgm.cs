#nullable enable

using UnityEngine;

namespace Uft.AdvTools.Commands
{
    public class CmdBgm : ICommand
    {
        public CommandCategory CommandCategory { get; } = CommandCategory.Sound;

        protected string Label { get; set; }
        protected bool IsLoop { get; set; }
        protected float Volume { get; set; }
        protected float PrevFadeOutSeconds { get; set; }
        protected float FadeInSeconds { get; set; }

        public CmdBgm(string soundLabel, bool? isLoop, float? volume, float? prevFadeOutSeconds, float? fadeInSeconds)
        {
            this.Label = soundLabel;
            this.IsLoop = isLoop ?? true;
            this.Volume = Mathf.Clamp(volume ?? 1.0f, 0.0f, 1.0f);
            this.PrevFadeOutSeconds = prevFadeOutSeconds ?? 0.2f;
            this.FadeInSeconds = fadeInSeconds ?? 0;
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            var clip = advRoot.BgmDictionary[this.Label];
            advRoot.SoundManager.ChangeBgm(clip, this.IsLoop, this.Volume, this.PrevFadeOutSeconds, this.FadeInSeconds);
        }
    }
}
