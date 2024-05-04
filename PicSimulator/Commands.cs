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
            IncTimer(pic);
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
            IncTimer(pic);
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
            IncTimer(pic);
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
            IncTimer(pic);
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
            IncTimer(pic);
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
            IncTimer(pic);
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
            IncTimer(pic);
        }
        public static void MOVWF(int file, Pic pic)
        {
            writeByte(pic.WReg, file, pic);
            IncTimer(pic);
        }
        public static void ADDWF(int file, Pic pic)
        {
            int tempADDWF = pic.WReg + readByte(file & 0b0111_1111, pic);
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
            if ((pic.WReg & 15) + (readByte(file & 0b0111_1111, pic) & 15) > 15)
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
                writeByte(tempADDWF, file & 0b0111_1111, pic);
            }
            IncTimer(pic);
        }
        public static void ANDWF(int file, Pic pic)
        {
            int tempANDWF = pic.WReg & readByte(file & 0b0111_1111, pic);
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
                writeByte(tempANDWF, file & 0b0111_1111, pic);
            }
            IncTimer(pic);
        }
        public static void CLRF(int file, Pic pic)
        {
            writeByte(0, file & 0b0111_1111, pic);
            setZeroFlag(pic);
            IncTimer(pic);
        }

        public static void COMF(int file, Pic pic)
        {
            int tempCOMF = readByte(file & 0b0111_1111, pic);                                                                                     
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
                writeByte(tempCOMF, file & 0b0111_1111, pic);
            }
            IncTimer(pic);
        }
        public static void DECF(int file, Pic pic)
        {
            int tempDECF = readByte(file & 0b0111_1111, pic) - 1;
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
                writeByte(tempDECF, file & 0b0111_1111, pic);
            }
            IncTimer(pic);
        }
        public static void INCF(int file, Pic pic)
        {
            int tempINCF = readByte(file & 0b0111_1111, pic) + 1;
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempINCF;
            }
            else
            {
                writeByte(tempINCF, file & 0b0111_1111, pic);                      
            }
            if (tempINCF == 0)
            {
                setZeroFlag(pic);
            }
            else
            {
                clearZeroFlag(pic);
            }
            IncTimer(pic);
        }

        public static void MOVF(int file, Pic pic)
        {
            int tempMOVF = readByte(file & 0b0111_1111, pic);
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempMOVF;
            }
            else
            {
                writeByte(tempMOVF, file & 0b0111_1111, pic);
            }

            if (tempMOVF == 0)
            {
                setZeroFlag(pic);
            }
            else
            {
                clearZeroFlag(pic);
            }
            IncTimer(pic);
        }
        public static void IORWF(int file, Pic pic)
        {
            int tempIORWF = readByte(file & 0b0111_1111, pic) | pic.WReg;
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempIORWF;
            }
            else
            {
                writeByte(tempIORWF, file & 0b0111_1111, pic);
            }
            if (tempIORWF == 0)
            {
                setZeroFlag(pic);
            }
            else
            {
                clearZeroFlag(pic);
            }
            IncTimer(pic);
        }
        public static void SUBWF(int file, Pic pic)
        {
            int tempSUBWF = readByte(file & 0b0111_1111, pic) - pic.WReg;
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
            if ((readByte(file & 0b0111_1111, pic) & 15) - (pic.WReg & 15) < 0)
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
                writeByte(tempSUBWF, file & 0b0111_1111, pic);
            }
            IncTimer(pic);
        }
        public static void SWAPF(int file, Pic pic)
        {
            int tempSWAPF = readByte(file & 0b0111_1111, pic);
            tempSWAPF = ((tempSWAPF & 0b0000_1111) << 4) | ((tempSWAPF & 0b1111_0000) >> 4);
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempSWAPF;
            }
            else
            {
                writeByte(tempSWAPF, file & 0b0111_1111, pic);
            }
            if (tempSWAPF == 0)
            {
                setZeroFlag(pic);
            }
            else
            {
                clearZeroFlag(pic);
            }
            IncTimer(pic);
        }
        public static void XORWF(int file, Pic pic)
        {
            int tempXORWF = readByte(file & 0b0111_1111, pic) ^ pic.WReg;
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempXORWF;
            }
            else
            {
                writeByte(tempXORWF, file & 0b0111_1111, pic);
            }
            if (tempXORWF == 0)
            {
                setZeroFlag(pic);
            }
            else
            {
                clearZeroFlag(pic);
            }
            IncTimer(pic);
        }
        public static void CLRW(int file, Pic pic)
        {
            pic.WReg = 0;
            setZeroFlag(pic);
            IncTimer(pic);
        }

        public static void RLF(int file, Pic pic)
        {
            int tempRLF = readByte(file & 0b0111_1111, pic);
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
                writeByte(tempRLF, file & 0b0111_1111, pic);
            }
            IncTimer(pic);
        }

        public static void RRF(int file, Pic pic)
        {
            int tempRRF = readByte(file & 0b0111_1111, pic);
            tempRRF = (tempRRF >> 1) | ((pic.Ram[3] & 1) == 1 ? 128 : 0);
            if ((readByte(file & 0b0111_1111, pic) & 1) == 1)
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
                writeByte(tempRRF, file & 0b0111_1111, pic);
            }
            IncTimer(pic);
        }

        public static void DECFSZ(int file, Pic pic)
        {
            int tempDECFSZ = readByte(file & 0b0111_1111, pic) - 1;
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
                writeByte(tempDECFSZ, file & 0b0111_1111, pic);
            }
            if (tempDECFSZ == 0)
            {
                pic.PCL++;
            }
            IncTimer(pic); //Timer wird nur um 1 erhöht da bei DECFSZ ein NOP() ausgeführt wird
        }

        public static void INCFSZ(int file, Pic pic)
        {
            int tempINCFSZ = (readByte(file & 0b0111_1111, pic) + 1) & 255;
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempINCFSZ;
            }
            else
            {
                writeByte(tempINCFSZ, file & 0b0111_1111, pic);
            }
            if (tempINCFSZ == 0)
            {
                pic.PCL++;
                NOP(pic);
            }
            IncTimer(pic); //Timer wird nur um 1 erhöht da bei INCFSZ ein NOP() ausgeführt wird
        }

        public static void BCF(int arg, Pic pic)
        {
            int file = arg & 0b0111_1111;
            int bit = (arg & 0b0000_0011_1000_0000) >> 7;
            clearBit(bit, file, pic);
            IncTimer(pic);
        }
        public static void BSF(int arg, Pic pic)
        {
            int file = arg & 0b0111_1111;
            int bit = (arg & 0b0000_0011_1000_0000) >> 7;
            setBit(bit, file, pic);
            IncTimer(pic);
        }
        public static void BTFSC(int arg, Pic pic)
        {
            int file = arg & 0b0111_1111;
            int bit = (arg & 0b0000_0011_1000_0000) >> 7;
            if ((readByte(file, pic) & (1 << bit)) == 0)
            {
                pic.PCL++;
                NOP(pic);
            }
            IncTimer(pic); //Timer wird nur um 1 erhöht da bei BTFSC ein NOP() ausgeführt wird
        }
        public static void BTFSS(int arg, Pic pic)
        {
            int file = arg & 0b0111_1111;
            int bit = (arg & 0b0000_0011_1000_0000) >> 7;
            if ((readByte(file, pic) & (1 << bit)) != 0)
            {
                pic.PCL++;
                NOP(pic);
            }
            IncTimer(pic); //Timer wird nur um 1 erhöht da bei BTFSS ein NOP() ausgeführt wird
        }





        //Hier müssen die ganzen Commands hin
        private static void setBit(int bit, int address, Pic pic)
        {
            if(address == 0)
            {
                address = pic.Ram[4];
            }
            else if ((pic.Ram[3] & 32) == 32)
                address += 128;
            switch (address)
            {
                case 0:
                case 2:
                case 3:
                case 4:
                case 10:
                case 11:
                    pic.Ram[address] |= 1 << bit;
                    pic.Ram[address + 128] = pic.Ram[address];
                    break;
                case 0x80:
                case 0x82:
                case 0x83:
                case 0x84:
                case 0x8A:
                case 0x8B:
                    pic.Ram[address] |= 1 << bit;
                    pic.Ram[address - 128] = pic.Ram[address];
                    break;
                case 1:
                    pic.Ram[address] |= 1 << (bit - 1);   //Timer wird um 1 verringert, weil er durch IncTimer() wieder um 1 erhöht wird
                    break;
                default:
                    pic.Ram[address] |= 1 << bit;
                    break;
            }
        }
        private static void clearBit(int bit, int address, Pic pic)
        {
            if (address == 0)
            {
                address = pic.Ram[4];
            }
            else if ((pic.Ram[3] & 32) == 32)
                address += 128;
            switch (address)
            {
                case 0:
                case 2:
                case 3:
                case 4:
                case 10:
                case 11:
                    pic.Ram[address] &= ~(1 << bit);
                    pic.Ram[address + 128] = pic.Ram[address];
                    break;
                case 0x80:
                case 0x82:
                case 0x83:
                case 0x84:
                case 0x8A:
                case 0x8B:
                    pic.Ram[address] &= ~(1 << bit);
                    pic.Ram[address - 128] = pic.Ram[address];
                    break;
                case 1:
                    pic.Ram[address] &= ~(1 << (bit - 1));   //Timer wird um 1 verringert, weil er durch IncTimer() wieder um 1 erhöht wird
                    break;
                default:
                    pic.Ram[address] &= ~(1 << bit);
                    break;
            }
        }
        private static void writeByte(int value, int address, Pic pic)
        {
            if(address == 0)
            {
                address = pic.Ram[4];
            }
            else if ((pic.Ram[3] & 32) == 32)
                address += 128;
            switch (address)
            {
                case 0:
                case 2:
                case 3:
                case 4:
                case 10:
                case 11:
                    pic.Ram[address] = value;
                    pic.Ram[address + 128] = pic.Ram[address];
                    break;
                case 0x80:
                case 0x82:
                case 0x83:
                case 0x84:
                case 0x8A:
                case 0x8B:
                    pic.Ram[address] = value;
                    pic.Ram[address - 128] = pic.Ram[address];
                    break;
                case 1:
                    pic.Ram[address] = value - 1;   //Timer wird um 1 verringert, weil er durch IncTimer() wieder um 1 erhöht wird
                    break;
                default:
                    pic.Ram[address] = value;
                    break;
            }
        }
        private static int readByte(int address, Pic pic)
        {
            if (address == 0)
            {
                address = pic.Ram[4];
            }
            else if ((pic.Ram[3] & 32) == 32)
                address += 128;
            return pic.Ram[address];
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
        private static void IncTimer(Pic pic)
        {
            pic.CodeTimer++;
            pic.Ram[1] = (pic.Ram[1] + 1) & 255;
        }

    }
}
