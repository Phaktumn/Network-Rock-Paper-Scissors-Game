using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class MenuOption
    {
        public string OptionString;
        public int IndexTriggerCode;
        public string OptionStringTriggerCode;


        public delegate void OnExecute(string read);
        public event OnExecute ExecuteEvent;

        public void OnExecuteEvent(string read)
        {
            ExecuteEvent?.Invoke(read);
        }
    }
}
