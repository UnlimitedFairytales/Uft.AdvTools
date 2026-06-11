#nullable enable

using UnityEngine;

namespace Uft.AdvTools.Commands
{
    public class CmdSe : ICommand
    {
        public CommandCategory CommandCategory { get; } = CommandCategory.Sound;

        protected string Label { get; set; }
        protected bool IsLoop { get; set; }
        protected float Volume { get; set; }

        public CmdSe(string soundLabel, bool? isLoop, float? volume)
        {
            this.Label = soundLabel;
            this.IsLoop = isLoop ?? false;
            this.Volume = Mathf.Clamp(volume ?? 1.0f, 0.0f, 1.0f);
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            if (!advRoot.SeDictionary.TryGetValue(this.Label, out var clip))
            {
                clip = advRoot.AssetLoadProxy.Load<AudioClip>(advRoot.SePathDictionary[this.Label]);
                advRoot.SeDictionary[this.Label] = clip;
            }
            advRoot.SoundManager.PlaySe(clip, this.IsLoop, this.Volume);
        }
    }
}
