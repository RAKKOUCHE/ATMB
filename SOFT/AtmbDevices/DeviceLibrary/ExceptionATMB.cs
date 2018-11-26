using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceLibrary
{
    [Serializable]public class ExceptionATMB : Exception
    {
        public ExceptionATMB()
        {

        }

        public ExceptionATMB(string Message) : base(Message)
        {

        }
    }
}
