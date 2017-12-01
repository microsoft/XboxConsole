using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xbox.XTF.Console
{
    public class ConsoleEventArgs : EventArgs
    {
        public readonly XtfConsole console;

        public ConsoleEventArgs(XtfConsole console)
        {
            this.console = console;
        }
    }

    internal class ConsoleManagerCallback : IXtfConsoleManagerCallback
    {
        private object sender;

        public event EventHandler<ConsoleEventArgs> ConsoleAdded;
        public event EventHandler<ConsoleEventArgs> ConsoleRemoved;
        public event EventHandler<ConsoleEventArgs> DefaultConsoleChanged;

        public ConsoleManagerCallback(object sender)
        {
            this.sender = sender;
        }

        void IXtfConsoleManagerCallback.OnAddConsole(ref _XTFCONSOLEDATA consoleData)
        {
            EventHandler<ConsoleEventArgs> handler = ConsoleAdded;

            if(null != handler)
            {
                handler(this.sender, new ConsoleEventArgs(new XtfConsole(consoleData)));
            }
        }

        void IXtfConsoleManagerCallback.OnRemoveConsole(ref _XTFCONSOLEDATA consoleData)
        {
            EventHandler<ConsoleEventArgs> handler = ConsoleRemoved;

            if (null != handler)
            {
                handler(this.sender, new ConsoleEventArgs(new XtfConsole(consoleData)));
            }
        }

        void IXtfConsoleManagerCallback.OnChangedDefaultConsole(ref _XTFCONSOLEDATA consoleData)
        {
            EventHandler<ConsoleEventArgs> handler = DefaultConsoleChanged;

            if (null != handler)
            {   
                handler(this.sender, new ConsoleEventArgs(null == consoleData.bstrAlias ? null : new XtfConsole(consoleData)));
            }
        }
    }
}
