#nullable enable

using System.Collections.Generic;
using Uft.AdvTools.Commands;
using Uft.AdvTools.Entities;

namespace Uft.AdvTools
{
    public class LogManager
    {
        readonly List<LogData> _logItemList = new();
        public IReadOnlyList<LogData> LogItemList => this._logItemList;

        public void Add(CmdText.PageCtrlType lastPageCtrl, Character? character, string name, string text)
        {
            var isNewEntry = CmdText.IsNewPage(lastPageCtrl);
            if (isNewEntry || this._logItemList.Count == 0)
            {
                var item = new LogData(character, name, text);
                this._logItemList.Add(item);
            }
            else
            {
                var last = this._logItemList[^1];
                last.AddText(lastPageCtrl, text);
            }
        }
    }
}
