#nullable enable

using Uft.AdvTools.View;

namespace Uft.AdvTools
{
    public class MessageWindowManager
    {
        public MessageWindow? MessageWindow { get; protected set; } // NOTE: Setup() 自動検出

        public void Setup(MessageWindow[] messageWindowList)
        {
            this.MessageWindow = messageWindowList[0];
        }

        public void Cleanup()
        {
            this.MessageWindow = null;
        }
    }
}
