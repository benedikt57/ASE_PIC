using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSimulator
{
    public class Commands
    {
        public static void MOVLW(int literal, Pic pic)
        {
            pic.WReg = literal;
        }
    }
}
