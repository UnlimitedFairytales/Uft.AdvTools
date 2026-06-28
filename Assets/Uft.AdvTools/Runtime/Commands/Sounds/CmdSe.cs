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
            advRoot.SoundManager.PlaySe(advRoot.SePathDictionary[this.Label], this.IsLoop, this.Volume);
        }
    }
}
