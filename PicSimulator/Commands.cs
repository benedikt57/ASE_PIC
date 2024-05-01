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
            pic.CodeTimer++;
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
            pic.CodeTimer++;
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
            pic.CodeTimer++;
        }
        public static void SUBLW(int literal, Pic pic)
        {
            int tempSUBLW = literal - pic.WReg;
            
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
            pic.WReg &= tempSUBLW;
            pic.CodeTimer++;
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
            pic.CodeTimer++;
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
            pic.CodeTimer++;
        }
        public static void GOTO(int literal, Pic pic)
        {
            pic.PCL &= 0b1111_1000_0000_0000;
            pic.PCL += literal;
            pic.CodeTimer += 2;
        }
        public static void CALL(int literal, Pic pic)
        {
            pic.Stack[pic.StackPointer] = pic.PCL;
            pic.StackPointer++;
            if(pic.StackPointer > 7)
                pic.StackPointer = 0;
            GOTO(literal, pic);
            //Codetimer wird in GOTO() erhöht
        }
        public static void RETURN(Pic pic)
        {
            pic.StackPointer--;
            if (pic.StackPointer < 0)
                pic.StackPointer = 7;
            pic.PCL = pic.Stack[pic.StackPointer];
            pic.CodeTimer += 2;
        }
        public static void RETLW(int literal, Pic pic)
        {
            pic.WReg = literal;
            RETURN(pic);
            //Codetimer wird in RETURN() erhöht
        }
        public static void NOP(Pic pic)
        {
            pic.CodeTimer++;
        }
        public static void MOVWF(int file, Pic pic)
        {
            writeByte(pic.WReg, file, pic);
            pic.CodeTimer++;
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
            pic.CodeTimer++;
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
            pic.CodeTimer++;
        }
        public static void CLRF(int file, Pic pic)
        {
            pic.Ram[file & 0b0111_1111] = 0;
            setZeroFlag(pic);
            pic.CodeTimer++;
        }

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
            pic.CodeTimer++;
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
            pic.CodeTimer++;
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
            pic.CodeTimer++;
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
            pic.CodeTimer++;
        }
        public static void IORWF(int file, Pic pic)
        {
            int tempIORWF = pic.Ram[file & 0b0111_1111] | pic.WReg;
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempIORWF;
            }
            else
            {
                pic.Ram[file & 0b0111_1111] = tempIORWF;
            }
            if (tempIORWF == 0)
            {
                setZeroFlag(pic);
            }
            else
            {
                clearZeroFlag(pic);
            }
            pic.CodeTimer++;
        }
        public static void SUBWF(int file, Pic pic)
        {
            int tempSUBWF = pic.Ram[file & 0b0111_1111] - pic.WReg;
            if (tempSUBWF == 0)
            {
                setZeroFlag(pic);
            }
            else
            {
                clearZeroFlag(pic);
            }
            if (tempSUBWF < 0)
            {
                clearCarryFlag(pic);
            }
            else
            {
                setCarryFlag(pic);
            }
            if ((pic.Ram[file & 0b0111_1111] & 15) - (pic.WReg & 15) < 0)
            {
                clearDigitCarryFlag(pic);
            }
            else
            {
                setDigitCarryFlag(pic);
            }
            tempSUBWF &= 255;
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempSUBWF;
            }
            else
            {
                pic.Ram[file & 0b0111_1111] = tempSUBWF;
            }
            pic.CodeTimer++;
        }
        public static void SWAPF(int file, Pic pic)
        {
            int tempSWAPF = pic.Ram[file & 0b0111_1111];
            tempSWAPF = ((tempSWAPF & 0b0000_1111) << 4) | ((tempSWAPF & 0b1111_0000) >> 4);
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempSWAPF;
            }
            else
            {
                pic.Ram[file & 0b0111_1111] = tempSWAPF;
            }
            if (tempSWAPF == 0)
            {
                setZeroFlag(pic);
            }
            else
            {
                clearZeroFlag(pic);
            }
            pic.CodeTimer++;
        }
        public static void XORWF(int file, Pic pic)
        {
            int tempXORWF = pic.Ram[file & 0b0111_1111] ^ pic.WReg;
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempXORWF;
            }
            else
            {
                pic.Ram[file & 0b0111_1111] = tempXORWF;
            }
            if (tempXORWF == 0)
            {
                setZeroFlag(pic);
            }
            else
            {
                clearZeroFlag(pic);
            }
            pic.CodeTimer++;
        }
        public static void CLRW(int file, Pic pic)
        {
            pic.WReg = 0;
            setZeroFlag(pic);
            pic.CodeTimer++;
        }

        public static void RLF(int file, Pic pic)
        {
            int tempRLF = pic.Ram[file & 0b0111_1111];
            tempRLF = (tempRLF << 1) | ((pic.Ram[3] & 1) == 1 ? 1 : 0);
            if ((tempRLF & 256) == 256)
            {
                setCarryFlag(pic);
            }
            else
            {
                clearCarryFlag(pic);
            }
            tempRLF &= 255;
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempRLF;
            }
            else
            {
                pic.Ram[file & 0b0111_1111] = tempRLF;
            }
            pic.CodeTimer++;
        }

        public static void RRF(int file, Pic pic)
        {
            int tempRRF = pic.Ram[file & 0b0111_1111];
            tempRRF = (tempRRF >> 1) | ((pic.Ram[3] & 1) == 1 ? 128 : 0);
            if ((pic.Ram[file & 0b0111_1111] & 1) == 1)
            {
                setCarryFlag(pic);
            }
            else
            {
                clearCarryFlag(pic);
            }
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempRRF;
            }
            else
            {
                pic.Ram[file & 0b0111_1111] = tempRRF;
            }
            pic.CodeTimer++;
        }

        public static void DECFSZ(int file, Pic pic)
        {
            int tempDECFSZ = pic.Ram[file & 0b0111_1111] - 1;
            if (tempDECFSZ == 0)
            {
                setZeroFlag(pic);
                NOP(pic);
            }
            else
            {
                clearZeroFlag(pic);
            }
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempDECFSZ;
            }
            else
            {
                pic.Ram[file & 0b0111_1111] = tempDECFSZ;
            }
            if (tempDECFSZ == 0)
            {
                pic.PCL++;
            }
            pic.CodeTimer++; //Timer wird nur um 1 erhöht da bei DECFSZ ein NOP() ausgeführt wird
        }

        public static void INCFSZ(int file, Pic pic)
        {
            int tempINCFSZ = (pic.Ram[file & 0b0111_1111] + 1) & 255;
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempINCFSZ;
            }
            else
            {
                pic.Ram[file & 0b0111_1111] = tempINCFSZ;
            }
            if (tempINCFSZ == 0)
            {
                pic.PCL++;
                NOP(pic);
            }
            pic.CodeTimer++; //Timer wird nur um 1 erhöht da bei INCFSZ ein NOP() ausgeführt wird
        }

        public static void BCF(int arg, Pic pic)
        {
            int file = arg & 0b0111_1111;
            int bit = (arg & 0b0000_0011_1000_0000) >> 7;
            clearBit(bit, file, pic);
            pic.CodeTimer++;
        }
        public static void BSF(int arg, Pic pic)
        {
            int file = arg & 0b0111_1111;
            int bit = (arg & 0b0000_0011_1000_0000) >> 7;
            setBit(bit, file, pic);
            pic.CodeTimer++;
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
