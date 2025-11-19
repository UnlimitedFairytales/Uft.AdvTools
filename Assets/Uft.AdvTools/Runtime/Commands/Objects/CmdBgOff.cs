#nullable enable

using Uft.UnityUtils.UI;

namespace Uft.AdvTools.Commands
{
    public class CmdBgOff : ICommand
    {
        public CommandCategory CommandCategory { get; } = CommandCategory.Object;

        protected float FadeSeconds { get; set; }

        public CmdBgOff(float? fadeSeconds)
        {
            this.FadeSeconds = fadeSeconds ?? 0.2f;
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            advRoot.Bg.ChangeBg(null, 0, 0, AnchorPreset.MiddleCenter, 1.0f, this.FadeSeconds);

            foreach (var kvp in advRoot.BgDictionary)
            {
                kvp.Value.ResetLastStatus();
            }
        }
    }
}
