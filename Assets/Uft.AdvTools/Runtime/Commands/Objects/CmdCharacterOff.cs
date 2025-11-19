#nullable enable

namespace Uft.AdvTools.Commands
{
    public class CmdCharacterOff : ICommand
    {
        public CommandCategory CommandCategory { get; } = CommandCategory.Object;

        protected string CharacterName { get; set; }
        protected float FadeSeconds { get; set; }

        public CmdCharacterOff(string characterName, float? fadeSeconds)
        {
            this.CharacterName = characterName;
            this.FadeSeconds = fadeSeconds ?? 0.2f;
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            var character = advRoot.CharacterDictionary[this.CharacterName];
            advRoot.SpriteManager.SetCharacterOff(character, this.FadeSeconds);

            character.ResetLastStatus();
        }
    }
}
