#nullable enable

using Uft.AdvTools.Commands;
using Uft.AdvTools.Entities;

namespace Uft.AdvTools
{
    public class LogData
    {
        public Character? Character { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }

        public LogData(Character? character, string name, string text)
        {
            this.Character = character;
            this.Name = name;
            this.Text = text;
        }

        public void AddText(CmdText.PageCtrlType lastPageCtrl, string additionalText)
        {
            if (lastPageCtrl == CmdText.PageCtrlType.InputBr)
            {
                additionalText = "\n" + additionalText;
            }
            this.Text += additionalText;
        }
    }
}
