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
            if(pic.WReg == 0)
            {
                setZeroFlag(pic);
            }
            else
            {
                clearZeroFlag(pic);
            }
        }
        public static void IORLW(int literal, Pic pic)
        {
            pic.WReg = pic.WReg | literal;
            if (pic.WReg == 0)
            {
                setZeroFlag(pic);
            }
            else
            {
                clearZeroFlag(pic);
            }
        }
        public static void SUBLW(int literal, Pic pic)
        {
            pic.WReg = literal - pic.WReg;
            if (pic.WReg == 0)
            {
                setZeroFlag(pic);
            }
            else
            {
                clearZeroFlag(pic);
            }
            if (pic.WReg < 0)
            {
                clearCarryFlag(pic);
            }
            else
            {
                setCarryFlag(pic);
            }
            if ((pic.WReg & 15) + (literal & 15) > 15)
            {
                clearDigitCarryFlag(pic);
            }
            else
            {
                setDigitCarryFlag(pic);
            }
        }
        public static void XORLW(int literal, Pic pic)
        {
            pic.WReg = pic.WReg ^ literal;
            if (pic.WReg == 0)
            {
                setZeroFlag(pic);
            }
            else
            {
                clearZeroFlag(pic);
            }
        }
        public static void ADDLW(int literal, Pic pic)
        {
            pic.WReg = pic.WReg + literal;
            if (pic.WReg == 0)
            {
                setZeroFlag(pic);
            }
            else
            {
                clearZeroFlag(pic);
            }
            if (pic.WReg > 255)
            {
                setCarryFlag(pic);
            }
            else
            {
                clearCarryFlag(pic);
            }
            if ((pic.WReg & 15) + (literal & 15) > 15)
            {
                setDigitCarryFlag(pic);
            }
            else
            {
                clearDigitCarryFlag(pic);
            }
        }
        private static void setZeroFlag(Pic pic)
        {
            pic.Ram[3] |= 4;
            pic.Ram[131] = pic.Ram[3];
        }
        private static void clearZeroFlag(Pic pic)
        {
            pic.Ram[3] &= 251;
            pic.Ram[131] = pic.Ram[3];
        }
        private static void setCarryFlag(Pic pic)
        {
            pic.Ram[3] |= 1;
            pic.Ram[131] = pic.Ram[3];
        }
        private static void clearCarryFlag(Pic pic)
        {
            pic.Ram[3] &= 254;
            pic.Ram[131] = pic.Ram[3];
        }
        private static void setDigitCarryFlag(Pic pic)
        {
            pic.Ram[3] |= 2;
            pic.Ram[131] = pic.Ram[3];
        }
        private static void clearDigitCarryFlag(Pic pic)
        {
            pic.Ram[3] &= 253;
            pic.Ram[131] = pic.Ram[3];
        }
    }
}
