using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic
{
    interface IDrone    
    {
        string DispatchCommand(string Command);

        string DispatchCommand(string Commmand, int Arg1);

        string DispatchCommand(string Command, int Arg1, int Arg2);
    }
}
