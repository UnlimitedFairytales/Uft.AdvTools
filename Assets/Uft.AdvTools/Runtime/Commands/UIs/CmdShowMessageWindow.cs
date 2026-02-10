#nullable enable

using Cysharp.Threading.Tasks;

namespace Uft.AdvTools.Commands
{
    public class CmdShowMessageWindow : ICommand
    {
        public CommandCategory CommandCategory { get; } = CommandCategory.UI;

        /// <summary>独自拡張</summary>
        protected float FadeSeconds { get; set; }

        public CmdShowMessageWindow(float? fadeSeconds)
        {
            this.FadeSeconds = fadeSeconds ?? 0;
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            advRoot.ShowUI(this.FadeSeconds);
            if (0 < this.FadeSeconds)
            {
                scenarioExecutor.IsWaiting = true;
                UniTask.Void(async () =>
                {
                    try
                    {
                        await UniTask.Delay((int)(this.FadeSeconds * 1000));
                    }
                    finally
                    {
                        scenarioExecutor.IsWaiting = false;
                    }
                });
            }
        }
    }
}
