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
            
            if (tempSUBLW == 0)
            {
                setZeroFlag(pic);
            }
            else
            {
                clearZeroFlag(pic);
            }
            if (tempSUBLW < 0)
            {
                clearCarryFlag(pic);
            }
            else
            {
                setCarryFlag(pic);
            }
            if ((tempSUBLW & 15) - (literal & 15) > 15)
            {
                clearDigitCarryFlag(pic);
            }
            else
            {
                setDigitCarryFlag(pic);
            }
            pic.WReg = tempSUBLW;
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
            IncTimer(pic);
            IncTimer(pic);
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
            IncTimer(pic);
            IncTimer(pic);
        }
        public static void RETLW(int literal, Pic pic)
        {
            pic.WReg = literal;
            RETURN(pic);
            //Codetimer wird in RETURN() erhöht
        }
        public static void RETFIE(Pic pic)
        {
            setBit(7, 0x0B, pic);
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



        public static void PowerOnReset (Pic pic)
        {
            pic.WReg = 0;
            pic.Ram[0x00] = 0;
            pic.Ram[0x01] = 0;
            pic.Ram[0x02] = 0x00;
            pic.PCL = 0;
            pic.Ram[0x03] = 0b0001_1000;
            pic.Ram[0x04] = 0;
            pic.Ram[0x05] = 0;
            pic.Ram[0x06] = 0;
            pic.Ram[0x07] = 0; //7 gibt es nicht
            pic.Ram[0x08] = 0;
            pic.Ram[0x09] = 0;
            pic.Ram[0x0A] = 0;
            pic.Ram[0x0B] = 0;
            pic.Ram[0x80] = 0;
            pic.Ram[0x81] = 0b1111_1111;
            pic.Ram[0x82] = 0;
            pic.Ram[0x83] = 0b0001_1000;
            pic.Ram[0x84] = 0;
            pic.Ram[0x85] = 0b0001_1111;
            pic.Ram[0x86] = 0b1111_1111;
            pic.Ram[0x87] = 0; //87 gibt es nicht
            pic.Ram[0x88] = 0;
            pic.Ram[0x89] = 0;
            pic.Ram[0x8A] = 0;
            pic.Ram[0x8B] = 0;
        }

        public static void MCLR (Pic pic)
        {
            pic.WReg = pic.WReg;
            pic.Ram[0x00] = 0;
            pic.Ram[0x01] = pic.Ram[0x01];
            pic.Ram[0x02] = 0x00;
            pic.PCL = 0;
            pic.Ram[0x03] &= 0b0001_1111;
            if (pic.isSleeping)
            {
                setBit(4, 0x03, pic);
                clearBit(3, 0x03, pic);
                pic.isSleeping = false;
            }
            pic.Ram[0x04] = pic.Ram[0x04];
            pic.Ram[0x05] &= 0b0001_1111;
            pic.Ram[0x06] = pic.Ram[0x06];
            pic.Ram[0x07] = 0; //7 gibt es nicht
            pic.Ram[0x08] = pic.Ram[0x08];
            pic.Ram[0x09] = pic.Ram[0x09];
            pic.Ram[0x0A] = 0b0000_0000;
            pic.Ram[0x0B] &= 0b0000_0001;
            pic.Ram[0x80] = 0;
            pic.Ram[0x81] = 0b1111_1111;
            pic.Ram[0x82] = 0x00;
            pic.Ram[0x83] = pic.Ram[0x03];
            pic.Ram[0x84] = pic.Ram[0x84];
            pic.Ram[0x85] = 0b0001_1111;
            pic.Ram[0x86] = 0b1111_1111;
            pic.Ram[0x87] = 0; //87 gibt es nicht
            pic.Ram[0x88] &= 0b0000_1000; // q wird zu unchanged, weil EEPROM nicht im Simulator
            pic.Ram[0x89] = 0b0000_0000;
            pic.Ram[0x8A] = 0;
            pic.Ram[0x8B] &= 0b0000_0001;
        }

        public static void WakeUpFromSleep (bool interrupt, Pic pic)
        {
            pic.isSleeping = false;
            pic.WReg = pic.WReg;
            pic.Ram[0x00] = 0;
            pic.Ram[0x01] = pic.Ram[0x01];
            //pic.Ram[0x02] = PC + 1;
            //pic.Ram[0x03] = uuuq_quuu; //Jonas
            if (interrupt)
            {
                setBit(4, 0x03, pic);
                clearBit(3, 0x03, pic);
            }
            else
            {
                clearBit(4, 0x03, pic);
                clearBit(3, 0x03, pic);
            }
            pic.Ram[0x04] = pic.Ram[0x04];
            pic.Ram[0x05] &= 0b0001_1111;
            pic.Ram[0x06] = pic.Ram[0x06];
            pic.Ram[0x07] = 0; //7 gibt es nicht
            pic.Ram[0x08] = pic.Ram[0x08];
            pic.Ram[0x09] = pic.Ram[0x09];
            pic.Ram[0x0A] &= 0b0001_1111;
            pic.Ram[0x0B] = pic.Ram[0x0B];
            pic.Ram[0x80] = 0;
            pic.Ram[0x81] = pic.Ram[0x81];
            //pic.Ram[0x82] = PC + 1;
            //pic.Ram[0x83] = uuuq_quuu; //Jonas
            pic.Ram[0x83] = pic.Ram[0x03];
            pic.Ram[0x84] = pic.Ram[0x84];
            pic.Ram[0x85] &= 0b0001_1111;
            pic.Ram[0x86] = pic.Ram[0x86];
            pic.Ram[0x87] = 0; //87 gibt es nicht
            pic.Ram[0x88] &= 0b0000_1111;
            pic.Ram[0x89] = pic.Ram[0x89];
            pic.Ram[0x8A] &= 0b0001_1111;
            pic.Ram[0x8B] = pic.Ram[0x8B];
        }
        public static void SLEEP (Pic pic)
        {
            if (((pic.Ram[0x0B] & 16) == 16 && (pic.Ram[0x0B] & 2) == 2)
                || ((pic.Ram[0x0B] & 8) == 8 && (pic.Ram[0x0B] & 1) == 1))
            {
                NOP(pic);
            }
            else
            {
                clearBit(3, 0x03, pic);
                setBit(4, 0x03, pic);
                if (pic.WDTActive)
                {
                    pic.WDTTimer = 0;
                    pic.WDTPrescaler = 0;
                }
                pic.isSleeping = true;
            }
        }
        public static void Watchdog(Pic pic)
        {
            var tmp = ((4 / (pic.AusgewaehlteQuarzfrequenzInt * 1e6)) * pic.WDTTimer);
            setWDTprescaler(pic);
            if(tmp >= (0.018 * pic.WDTPrescaler))
            {
                MessageBox.Show("Watchdog Timer Reset");
                pic.WDTTimer = 0;
                if (pic.isSleeping)
                {
                    WakeUpFromSleep(false, pic);
                }
                else
                {
                    MCLR(pic);
                    clearBit(4, 0x03, pic);
                    setBit(3, 0x03, pic);
                }
            }
        }

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
                    pic.Ram[address] |= 1 << bit;
                    PicTimer = 0;
                    break;
                case 5:
                    if ((pic.Ram[0x85] & (1 << bit)) == 0)
                    {
                        pic.Ram[address] |= 1 << bit;
                    }
                    break;
                case 6:
                    if ((pic.Ram[0x86] & (1 << bit)) == 0)
                    {
                        pic.Ram[address] |= 1 << bit;
                    }
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
                    pic.Ram[address] &= ~(1 << bit);
                    PicTimer = 0;
                    break;
                case 5:
                    if ((pic.Ram[0x85] & (1 << bit)) == 0)
                    {
                        pic.Ram[address] &= ~(1 << bit);
                    }
                    break;
                case 6:
                    if ((pic.Ram[0x86] & (1 << bit)) == 0)
                    {
                        pic.Ram[address] &= ~(1 << bit);
                    }
                    break;
                default:
                    pic.Ram[address] &= ~(1 << bit);
                    break;
            }
        }
        private static void writeBit(int value, int bit, int address, Pic pic)
        {
            if (value == 0)
                clearBit(bit, address, pic);
            else
                setBit(bit, address, pic);
        }
        public static void writeByte(int value, int address, Pic pic, bool checkbank = true)
        {
            value &= 255;
            if (checkbank)
            {
                if (address == 0)
                {
                    address = pic.Ram[4];
                }
                else if ((pic.Ram[3] & 32) == 32)
                    address += 128; 
            }
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
                    pic.Ram[address] = value;
                    PicTimer = 0;
                    break;
                case 5:
                    writeBit(value & 1,        0, 5, pic);
                    writeBit((value & 2) >> 1, 1, 5, pic);
                    writeBit((value & 4) >> 2, 2, 5, pic);
                    writeBit((value & 8) >> 3, 3, 5, pic);
                    writeBit((value & 16) >> 4, 4, 5, pic);
                    writeBit((value & 32) >> 5, 5, 5, pic);
                    writeBit((value & 64) >> 6, 6, 5, pic);
                    writeBit((value & 128) >> 7, 7, 5, pic);
                    break;
                case 6:
                    writeBit(value & 1,        0, 6, pic);
                    writeBit((value & 2) >> 1, 1, 6, pic);
                    writeBit((value & 4) >> 2, 2, 6, pic);
                    writeBit((value & 8) >> 3, 3, 6, pic);
                    writeBit((value & 16) >> 4, 4, 6, pic);
                    writeBit((value & 32) >> 5, 5, 6, pic);
                    writeBit((value & 64) >> 6, 6, 6, pic);
                    writeBit((value & 128) >> 7, 7, 6, pic);
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
        public static void Reset(Pic pic)
        {
            pic.CodeTimer = 0;
            PicTimer = 0;
        }
        private static int PicTimer;
        public static void IncTimer(Pic pic)
        {
            pic.CodeTimer++;
            if ((pic.Ram[0x81] & 32) == 0)
            {
                PicTimer++;
                setTimer(pic);
            }
            if (pic.WDTActive)
            {
                pic.WDTTimer++;
            }
        }
        private static void setTimer(Pic pic)
        {
            if ((pic.Ram[0x81] & 8) == 0)
            {
                switch(pic.Ram[0x81] & 7)
                {
                    case 0:
                        if(PicTimer % 2 == 0)
                            pic.Ram[1]++;
                        break;
                    case 1:
                        if (PicTimer % 4 == 0)
                            pic.Ram[1]++;
                        break;
                    case 2:
                        if (PicTimer % 8 == 0)
                            pic.Ram[1]++;
                        break;
                    case 3:
                        if (PicTimer % 16 == 0)
                            pic.Ram[1]++;
                        break;
                    case 4:
                        if (PicTimer % 32 == 0)
                            pic.Ram[1]++;
                        break;
                    case 5:
                        if (PicTimer % 64 == 0)
                            pic.Ram[1]++;
                        break;
                    case 6:
                        if (PicTimer % 128 == 0)
                            pic.Ram[1]++;
                        break;
                    case 7:
                        if (PicTimer % 256 == 0)
                            pic.Ram[1]++;
                        break;
                }
            }
            else
            {
                pic.Ram[1]++;
            }
            if (pic.Ram[1] > 255)
                setBit(2, 0x0B, pic);
            pic.Ram[1] &= 255;
        }
        private static int lastRA4;
        public static void RA4(Pic pic)
        {
            if ((pic.Ram[0x81] & 32) == 32)
            {
                if ((pic.Ram[5] & 16) == 0 && lastRA4 == 1)
                {
                PicTimer++;
                setTimer(pic);
                }
            }
            lastRA4 = (pic.Ram[5] & 16) >> 4;
        }
        private static int lastRB0;
        public static void RB0(Pic pic)
        {
            if ((pic.Ram[6] & 1) != lastRB0)
            {
                lastRB0 = pic.Ram[6] & 1;
                if ((pic.Ram[0x81] & 64) == 64 && lastRB0 == 1
                    || (pic.Ram[0x81] & 64) == 0 && lastRB0 == 0)
                {
                    setBit(1, 0x0B, pic);
                }
            }
        }
        private static int lastRB7_4;
        public static void PORTBINT(Pic pic)
        {
            if ((pic.Ram[6] & 0xF0) != lastRB7_4)
            {
                lastRB7_4 = pic.Ram[6] & 0xF0;
                if ((pic.Ram[0x81] & 8) == 8)
                {
                    setBit(0, 0x0B, pic);
                }
            }
        }
        public static void InterruptTest(Pic pic)
        {
            if ((pic.Ram[0x8B] & 128) == 128) // GIE prüfen
            {
                if ((pic.Ram[0x8B] & 32) == 32) // TOIE prüfen
                {
                    if ((pic.Ram[0x8B] & 4) == 4) // TOIF prüfen
                    {
                        clearBit(7, 0x0B, pic); // GIE löschen
                        pic.Stack[pic.StackPointer] = pic.PCL;
                        pic.StackPointer++;
                        if (pic.StackPointer > 7)
                            pic.StackPointer = 0;
                        pic.PCL = 4;
                    }
                }
                if ((pic.Ram[0x8B] & 16) == 16) // INTE prüfen
                {
                    if ((pic.Ram[0x8B] & 2) == 2) // INTF prüfen
                    {
                        clearBit(7, 0x0B, pic); // GIE löschen
                        pic.Stack[pic.StackPointer] = pic.PCL;
                        pic.StackPointer++;
                        if (pic.StackPointer > 7)
                            pic.StackPointer = 0;
                        pic.PCL = 4;
                    }
                }
                if ((pic.Ram[0x8B] & 8) == 8) // RBIE prüfen
                {
                    if ((pic.Ram[0x8B] & 1) == 1) // RBIF prüfen
                    {
                        clearBit(7, 0x0B, pic); // GIE löschen
                        pic.Stack[pic.StackPointer] = pic.PCL;
                        pic.StackPointer++;
                        if (pic.StackPointer > 7)
                            pic.StackPointer = 0;
                        pic.PCL = 4;
                    }
                }
            }
        }
        public static void WakeUpTest(Pic pic)
        {
            if ((pic.Ram[0x8B] & 16) == 16) // INTE prüfen
            {
                if ((pic.Ram[0x8B] & 2) == 2) // INTF prüfen
                {
                    if ((pic.Ram[0x8B] & 128) == 128) // GIE prüfen
                    {
                        WakeUpFromSleep(true, pic);
                    }
                    else
                    {
                        WakeUpFromSleep(true, pic);
                    }
                }
            }
            if ((pic.Ram[0x8B] & 8) == 8) // RBIE prüfen
            {
                if ((pic.Ram[0x8B] & 1) == 1) // RBIF prüfen
                {
                    if ((pic.Ram[0x8B] & 128) == 128) // GIE prüfen
                    {
                        WakeUpFromSleep(true, pic);
                    }
                    else
                    {
                        WakeUpFromSleep(true, pic);
                    }
                }
            }
        }
        private static void setWDTprescaler(Pic pic)
        {
            if ((pic.Ram[0x81] & 8) == 8)
            {
                switch (pic.Ram[0x81] & 7)
                {
                    case 0:
                        pic.WDTPrescaler = 1;
                        break;
                    case 1:
                        pic.WDTPrescaler = 2;
                        break;
                    case 2:
                        pic.WDTPrescaler = 4;
                        break;
                    case 3:
                        pic.WDTPrescaler = 8;
                        break;
                    case 4:
                        pic.WDTPrescaler = 16;
                        break;
                    case 5:
                        pic.WDTPrescaler = 32;
                        break;
                    case 6:
                        pic.WDTPrescaler = 64;
                        break;
                    case 7:
                        pic.WDTPrescaler = 128;
                        break;
                }
            }
            else
            {
                pic.WDTPrescaler = 1;
            }
        }
    }
}
