using PicSimulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class GotoDecoder : IDecoder
    {
        public bool Decode(int code, ICommands commands)
        {
            int opcode = code & 0b0011_1000_0000_0000;
            if (opcode == 0b0010_1000_0000_0000)
            {
                commands.GOTO(code & 0b0000_0111_1111_1111);
                return true;
            }
            return false;
        }
    }

    public class CallDecoder : IDecoder
    {
        public bool Decode(int code, ICommands commands)
        {
            int opcode = code & 0b0011_1000_0000_0000;
            if (opcode == 0b0010_0000_0000_0000)
            {
                commands.CALL(code & 0b0000_0111_1111_1111);
                return true;
            }
            return false;
        }
    }
}
