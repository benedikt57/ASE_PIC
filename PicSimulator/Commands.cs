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
        public static void ANDLW(int literal, Pic pic)
        {
            pic.WReg = pic.WReg & literal;
        }
        public static void IORLW(int literal, Pic pic)
        {
            pic.WReg = pic.WReg | literal;
        }
        public static void SUBLW(int literal, Pic pic)
        {
            pic.WReg = literal - pic.WReg;
        }
        public static void XORLW(int literal, Pic pic)
        {
            pic.WReg = pic.WReg ^ literal;
        }
        public static void ADDLW(int literal, Pic pic)
        {
            pic.WReg = pic.WReg + literal;
        }
    }
}
