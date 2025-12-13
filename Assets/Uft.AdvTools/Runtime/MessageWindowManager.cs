#nullable enable

using Uft.AdvTools.View;

namespace Uft.AdvTools
{
    /// <summary>
    /// 1. 非表示Windowへフォーカスする場合、直前のWindowは非表示される<br/>
    /// 2. 表示Window間へフォーカスする場合、直前のWindowはそのまま
    /// </summary>
    public class MessageWindowManager
    {
        public MessageWindow? CurrentMessageWindow { get; protected set; }

        public void Setup(MessageWindow[] messageWindowList)
        {
            this.CurrentMessageWindow = messageWindowList[0];
        }

        public void Cleanup()
        {
            this.CurrentMessageWindow = null;
        }
    }
}
