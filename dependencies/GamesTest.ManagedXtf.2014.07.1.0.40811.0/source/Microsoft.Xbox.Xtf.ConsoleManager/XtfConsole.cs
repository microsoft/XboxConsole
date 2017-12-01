using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xbox.XTF.Console
{
    public class XtfConsole
    {
        internal _XTFCONSOLEDATA consoleData;

        internal XtfConsole(_XTFCONSOLEDATA consoleData)
        {
            this.consoleData = consoleData;
        }

        public string Alias
        {
            get
            {
                return consoleData.bstrAlias;
            }
        }

        public string Address
        {
            get
            {
                return consoleData.bstrAddress;
            }
        }

        public override bool Equals(object other)
        {
            if (null == other)
            {
                return false;
            }

            if (other.GetType() != this.GetType())
            {
                return false;
            }

            XtfConsole otherConsole = (XtfConsole)other;

            return this.GetHashCode() == otherConsole.GetHashCode();
        }

        public override int GetHashCode()
        {
            return this.Alias.GetHashCode();
        }
    }
}
