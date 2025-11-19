#nullable enable

using Cysharp.Threading.Tasks;

namespace Uft.AdvTools.Commands
{
    public class CmdWait : ICommand
    {
        public CommandCategory CommandCategory { get; } = CommandCategory.Effect;

        protected float WaitSeconds { get; set; }

        public CmdWait(float waitSeconds)
        {
            this.WaitSeconds = waitSeconds;
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            scenarioExecutor.IsWaiting = true;
            UniTask.Void(async () =>
            {
                try
                {
                    await UniTask.Delay((int)(this.WaitSeconds * 1000));
                }
                finally
                {
                    scenarioExecutor.IsWaiting = false;
                }
            });
        }
    }
}
