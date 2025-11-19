#nullable enable

using UnityEngine;

namespace Uft.AdvTools.Commands
{
    public class CmdSe : ICommand
    {
        public CommandCategory CommandCategory { get; } = CommandCategory.Sound;

        protected string Label { get; set; }
        protected float Volume { get; set; }

        public CmdSe(string soundLabel, float? volume)
        {
            this.Label = soundLabel;
            this.Volume = Mathf.Clamp(volume ?? 1.0f, 0.0f, 1.0f);
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            var clip = advRoot.SeDictionary[this.Label];
            advRoot.SoundManager.PlayOneShotSe(clip, this.Volume);
        }
    }
}
