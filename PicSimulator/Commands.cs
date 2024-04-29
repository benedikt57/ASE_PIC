using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        public static void GOTO(int literal, Pic pic)
        {
            pic.PCL &= 0b1111_1000_0000_0000;
            pic.PCL += literal;
        }
        public static void CALL(int literal, Pic pic)
        {
            pic.Stack[pic.StackPointer] = pic.PCL;
            pic.StackPointer++;
            if(pic.StackPointer > 7)
                pic.StackPointer = 0;
            GOTO(literal, pic);
        }
        public static void RETURN(Pic pic)
        {
            pic.StackPointer--;
            if (pic.StackPointer < 0)
                pic.StackPointer = 7;
            pic.PCL = pic.Stack[pic.StackPointer];
        }
        public static void RETLW(int literal, Pic pic)
        {
            pic.WReg = literal;
            RETURN(pic);
        }
        public static void NOP()
        {
        }
        public static void MOVWF(int file, Pic pic)
        {
            writeByte(pic.WReg, file, pic);
        }
        public static void ADDWF(int file, Pic pic)
        {
            int tempADDWF = pic.WReg + pic.Ram[file & 0b0111_1111];
            if (tempADDWF == 0)
            {
                setZeroFlag(pic);
            }
            else
            {
                clearZeroFlag(pic);
            }
            if (tempADDWF > 255)
            {
                setCarryFlag(pic);
            }
            else
            {
                clearCarryFlag(pic);
            }
            if ((pic.WReg & 15) + (pic.Ram[file & 0b0111_1111] & 15) > 15)
            {
                setDigitCarryFlag(pic);
            }
            else
            {
                clearDigitCarryFlag(pic);
            }
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempADDWF;
            }
            else
            {
                pic.Ram[file & 0b0111_1111] = tempADDWF;
            }
        }
        public static void ANDWF(int file, Pic pic)
        {
            int tempANDWF = pic.WReg & pic.Ram[file & 0b0111_1111];
            if (tempANDWF == 0)
            {
                setZeroFlag(pic);
            }
            else
            {
                clearZeroFlag(pic);
            }
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempANDWF;
            }
            else
            {
                pic.Ram[file & 0b0111_1111] = tempANDWF;
            }
        }
        public static void CLRF(int file, Pic pic)
        {
            pic.Ram[file & 0b0111_1111] = 0;
            setZeroFlag(pic);
        }

        //public static void CLRW(Pic pic)
        //{
        //    pic.WReg = 0;
        //    setZeroFlag(pic);
        //}

        public static void COMF(int file, Pic pic)
        {
            int tempCOMF = pic.Ram[file & 0b0111_1111];
            tempCOMF = ~tempCOMF;
            if (tempCOMF == 0)
            {
                setZeroFlag(pic);
            }
            else
            {
                clearZeroFlag(pic);
            }
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempCOMF;
            }
            else
            {
                pic.Ram[file & 0b0111_1111] = tempCOMF;
            }
        }
        public static void DECF(int file, Pic pic)
        {
            int tempDECF = pic.Ram[file & 0b0111_1111] - 1;
            if (tempDECF == 0)
            {
                setZeroFlag(pic);
            }
            else
            {
                clearZeroFlag(pic);
            }
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempDECF;
            }
            else
            {
                pic.Ram[file & 0b0111_1111] = tempDECF;
            }
        }
        public static void INCF(int file, Pic pic)
        {
            int tempINCF = pic.Ram[file & 0b0111_1111] + 1;
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempINCF;
            }
            else
            {
                pic.Ram[file & 0b0111_1111] = tempINCF;
            }
            if (tempINCF == 0)
            {
                setZeroFlag(pic);
            }
            else
            {
                clearZeroFlag(pic);
            }

        }

        public static void MOVF(int file, Pic pic)
        {
            int tempMOVF = pic.Ram[file & 0b0111_1111];
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempMOVF;
            }
            else
            {
                pic.Ram[file & 0b0111_1111] = tempMOVF;
            }

            if (tempMOVF == 0)
            {
                setZeroFlag(pic);
            }
            else
            {
                clearZeroFlag(pic);
            }

        }





        //Hier müssen die ganzen Commands hin
        private static void setBit(int bit, int address, Pic pic)
        {
            if ((pic.Ram[3] & 32) == 32)
                address += 128;
            pic.Ram[address] |= 1 << bit;
        }
        private static void clearBit(int bit, int address, Pic pic)
        {
            if ((pic.Ram[3] & 32) == 32)
                address += 128;
            pic.Ram[address] &= ~(1 << bit);
        }
        private static void writeByte(int value, int address, Pic pic)
        {
            if ((pic.Ram[3] & 32) == 32)
                address += 128;
            pic.Ram[address] = value;
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
