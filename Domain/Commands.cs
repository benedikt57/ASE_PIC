using Domain;
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
        private IPic pic;
        public Commands(IPic _pic)
        {
            pic = _pic;
        }

        public void MOVLW(int literal)
        {
            pic.WReg = literal;
            IncTimer();
        }
        public void ANDLW(int literal)
        {
            pic.WReg = pic.WReg & literal;
            if (pic.WReg == 0)
            {
                setZeroFlag();
            }
            else
            {
                clearZeroFlag();
            }
            IncTimer();
        }
        public void IORLW(int literal)
        {
            pic.WReg = pic.WReg | literal;
            if (pic.WReg == 0)
            {
                setZeroFlag();
            }
            else
            {
                clearZeroFlag();
            }
            IncTimer();
        }

        public void SUBLW(int literal)
        {
            int tempSUBLW = literal - pic.WReg;

            if (tempSUBLW == 0)
            {
                setZeroFlag();
            }
            else
            {
                clearZeroFlag();
            }
            if (tempSUBLW < 0)
            {
                clearCarryFlag();
            }
            else
            {
                setCarryFlag();
            }
            if ((tempSUBLW & 15) - (literal & 15) > 15)
            {
                clearDigitCarryFlag();
            }
            else
            {
                setDigitCarryFlag();
            }
            pic.WReg = tempSUBLW;
            IncTimer();
        }
        public void XORLW(int literal)
        {
            pic.WReg = pic.WReg ^ literal;
            if (pic.WReg == 0)
            {
                setZeroFlag();
            }
            else
            {
                clearZeroFlag();
            }
            IncTimer();
        }
        public void ADDLW(int literal)
        {
            pic.WReg = pic.WReg + literal;
            if (pic.WReg == 0)
            {
                setZeroFlag();
            }
            else
            {
                clearZeroFlag();
            }
            if (pic.WReg > 255)
            {
                setCarryFlag();
            }
            else
            {
                clearCarryFlag();
            }
            if ((pic.WReg & 15) + (literal & 15) > 15)
            {
                setDigitCarryFlag();
            }
            else
            {
                clearDigitCarryFlag();
            }
            IncTimer();
        }
        public void GOTO(int literal)
        {
            pic.PC &= 0b1111_1000_0000_0000;
            pic.PC += literal;
            pic.PC += ((pic.PCLATCH & 0b0001_1000) << 8);
            IncTimer();
            IncTimer();
        }
        public void CALL(int literal)
        {
            pic.Stack[pic.StackPointer] = pic.PC;
            pic.StackPointer++;
            if (pic.StackPointer > 7)
                pic.StackPointer = 0;
            GOTO(literal);
            //Codetimer wird in GOTO() erhöht
        }
        public void RETURN()
        {
            pic.StackPointer--;
            if (pic.StackPointer < 0)
                pic.StackPointer = 7;
            pic.PC = pic.Stack[pic.StackPointer];
            IncTimer();
            IncTimer();
        }
        public void RETLW(int literal)
        {
            pic.WReg = literal;
            RETURN();
            //Codetimer wird in RETURN() erhöht
        }
        public void RETFIE()
        {
            setBit(7, 0x0B);
            RETURN();
            //Codetimer wird in RETURN() erhöht
        }
        public void NOP()
        {
            IncTimer();
        }
        public void MOVWF(int file)
        {
            writeByte(pic.WReg, file);
            IncTimer();
        }
        public void ADDWF(int file)
        {
            int tempADDWF = pic.WReg + readByte(file & 0b0111_1111);
            if (tempADDWF == 0)
            {
                setZeroFlag();
            }
            else
            {
                clearZeroFlag();
            }
            if (tempADDWF > 255)
            {
                setCarryFlag();
            }
            else
            {
                clearCarryFlag();
            }
            if ((pic.WReg & 15) + (readByte(file & 0b0111_1111) & 15) > 15)
            {
                setDigitCarryFlag();
            }
            else
            {
                clearDigitCarryFlag();
            }
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempADDWF;
            }
            else
            {
                writeByte(tempADDWF, file & 0b0111_1111);
            }
            IncTimer();
        }
        public void ANDWF(int file)
        {
            int tempANDWF = pic.WReg & readByte(file & 0b0111_1111);
            if (tempANDWF == 0)
            {
                setZeroFlag();
            }
            else
            {
                clearZeroFlag();
            }
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempANDWF;
            }
            else
            {
                writeByte(tempANDWF, file & 0b0111_1111);
            }
            IncTimer();
        }
        public void CLRF(int file)
        {
            writeByte(0, file & 0b0111_1111);
            setZeroFlag();
            IncTimer();
        }

        public void COMF(int file)
        {
            int tempCOMF = readByte(file & 0b0111_1111);
            tempCOMF = ~tempCOMF;
            if (tempCOMF == 0)
            {
                setZeroFlag();
            }
            else
            {
                clearZeroFlag();
            }
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempCOMF;
            }
            else
            {
                writeByte(tempCOMF, file & 0b0111_1111);
            }
            IncTimer();
        }
        public void DECF(int file)
        {
            int tempDECF = readByte(file & 0b0111_1111) - 1;
            if (tempDECF == 0)
            {
                setZeroFlag();
            }
            else
            {
                clearZeroFlag();
            }
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempDECF;
            }
            else
            {
                writeByte(tempDECF, file & 0b0111_1111);
            }
            IncTimer();
        }
        public void INCF(int file)
        {
            int tempINCF = readByte(file & 0b0111_1111) + 1;
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempINCF;
            }
            else
            {
                writeByte(tempINCF, file & 0b0111_1111);
            }
            if (tempINCF == 0)
            {
                setZeroFlag();
            }
            else
            {
                clearZeroFlag();
            }
            IncTimer();
        }

        public void MOVF(int file)
        {
            int tempMOVF = readByte(file & 0b0111_1111);
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempMOVF;
            }
            else
            {
                writeByte(tempMOVF, file & 0b0111_1111);
            }

            if (tempMOVF == 0)
            {
                setZeroFlag();
            }
            else
            {
                clearZeroFlag();
            }
            IncTimer();
        }
        public void IORWF(int file)
        {
            int tempIORWF = readByte(file & 0b0111_1111) | pic.WReg;
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempIORWF;
            }
            else
            {
                writeByte(tempIORWF, file & 0b0111_1111);
            }
            if (tempIORWF == 0)
            {
                setZeroFlag();
            }
            else
            {
                clearZeroFlag();
            }
            IncTimer();
        }
        public void SUBWF(int file)
        {
            int tempSUBWF = readByte(file & 0b0111_1111) - pic.WReg;
            if (tempSUBWF == 0)
            {
                setZeroFlag();
            }
            else
            {
                clearZeroFlag();
            }
            if (tempSUBWF < 0)
            {
                clearCarryFlag();
            }
            else
            {
                setCarryFlag();
            }
            if ((readByte(file & 0b0111_1111) & 15) - (pic.WReg & 15) < 0)
            {
                clearDigitCarryFlag();
            }
            else
            {
                setDigitCarryFlag();
            }
            tempSUBWF &= 255;
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempSUBWF;
            }
            else
            {
                writeByte(tempSUBWF, file & 0b0111_1111);
            }
            IncTimer();
        }
        public void SWAPF(int file)
        {
            int tempSWAPF = readByte(file & 0b0111_1111);
            tempSWAPF = ((tempSWAPF & 0b0000_1111) << 4) | ((tempSWAPF & 0b1111_0000) >> 4);
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempSWAPF;
            }
            else
            {
                writeByte(tempSWAPF, file & 0b0111_1111);
            }
            if (tempSWAPF == 0)
            {
                setZeroFlag();
            }
            else
            {
                clearZeroFlag();
            }
            IncTimer();
        }
        public void XORWF(int file)
        {
            int tempXORWF = readByte(file & 0b0111_1111) ^ pic.WReg;
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempXORWF;
            }
            else
            {
                writeByte(tempXORWF, file & 0b0111_1111);
            }
            if (tempXORWF == 0)
            {
                setZeroFlag();
            }
            else
            {
                clearZeroFlag();
            }
            IncTimer();
        }
        public void CLRW(int file)
        {
            pic.WReg = 0;
            setZeroFlag();
            IncTimer();
        }

        public void RLF(int file)
        {
            int tempRLF = readByte(file & 0b0111_1111);
            tempRLF = (tempRLF << 1) | ((pic.Ram[3] & 1) == 1 ? 1 : 0);
            if ((tempRLF & 256) == 256)
            {
                setCarryFlag();
            }
            else
            {
                clearCarryFlag();
            }
            tempRLF &= 255;
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempRLF;
            }
            else
            {
                writeByte(tempRLF, file & 0b0111_1111);
            }
            IncTimer();
        }

        public void RRF(int file)
        {
            int tempRRF = readByte(file & 0b0111_1111);
            tempRRF = (tempRRF >> 1) | ((pic.Ram[3] & 1) == 1 ? 128 : 0);
            if ((readByte(file & 0b0111_1111) & 1) == 1)
            {
                setCarryFlag();
            }
            else
            {
                clearCarryFlag();
            }
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempRRF;
            }
            else
            {
                writeByte(tempRRF, file & 0b0111_1111);
            }
            IncTimer();
        }

        public void DECFSZ(int file)
        {
            int tempDECFSZ = readByte(file & 0b0111_1111) - 1;
            if (tempDECFSZ == 0)
            {
                setZeroFlag();
                NOP();
            }
            else
            {
                clearZeroFlag();
            }
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempDECFSZ;
            }
            else
            {
                writeByte(tempDECFSZ, file & 0b0111_1111);
            }
            if (tempDECFSZ == 0)
            {
                pic.PC++;
            }
            IncTimer(); //Timer wird nur um 1 erhöht da bei DECFSZ ein NOP() ausgeführt wird
        }

        public void INCFSZ(int file)
        {
            int tempINCFSZ = (readByte(file & 0b0111_1111) + 1) & 255;
            if ((file & 0b1000_0000) == 0)
            {
                pic.WReg = tempINCFSZ;
            }
            else
            {
                writeByte(tempINCFSZ, file & 0b0111_1111);
            }
            if (tempINCFSZ == 0)
            {
                pic.PC++;
                NOP();
            }
            IncTimer(); //Timer wird nur um 1 erhöht da bei INCFSZ ein NOP() ausgeführt wird
        }

        public void BCF(int arg)
        {
            int file = arg & 0b0111_1111;
            int bit = (arg & 0b0000_0011_1000_0000) >> 7;
            clearBit(bit, file);
            IncTimer();
        }
        public void BSF(int arg)
        {
            int file = arg & 0b0111_1111;
            int bit = (arg & 0b0000_0011_1000_0000) >> 7;
            setBit(bit, file);
            IncTimer();
        }
        public void BTFSC(int arg)
        {
            int file = arg & 0b0111_1111;
            int bit = (arg & 0b0000_0011_1000_0000) >> 7;
            if ((readByte(file) & (1 << bit)) == 0)
            {
                pic.PC++;
                NOP();
            }
            IncTimer(); //Timer wird nur um 1 erhöht da bei BTFSC ein NOP() ausgeführt wird
        }
        public void BTFSS(int arg)
        {
            int file = arg & 0b0111_1111;
            int bit = (arg & 0b0000_0011_1000_0000) >> 7;
            if ((readByte(file) & (1 << bit)) != 0)
            {
                pic.PC++;
                NOP();
            }
            IncTimer(); //Timer wird nur um 1 erhöht da bei BTFSS ein NOP() ausgeführt wird
        }



        public void PowerOnReset()
        {
            pic.WReg = 0;
            pic.Ram[0x00] = 0;
            pic.Ram[0x01] = 0;
            pic.Ram[0x02] = 0x00;
            pic.PC = 0;
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
            pic.Ram[0x85] = 0b1111_1111;
            pic.Ram[0x86] = 0b1111_1111;
            pic.Ram[0x87] = 0; //87 gibt es nicht
            pic.Ram[0x88] = 0;
            pic.Ram[0x89] = 0;
            pic.Ram[0x8A] = 0;
            pic.Ram[0x8B] = 0;
        }

        public void MCLR()
        {
            pic.WReg = pic.WReg;
            pic.Ram[0x00] = 0;
            pic.Ram[0x01] = pic.Ram[0x01];
            pic.Ram[0x02] = 0x00;
            pic.PC = 0;
            pic.Ram[0x03] &= 0b0001_1111;
            if (pic.IsSleeping)
            {
                setBit(4, 0x03);
                clearBit(3, 0x03);
                pic.IsSleeping = false;
            }
            pic.Ram[0x04] = pic.Ram[0x04];
            pic.Ram[0x05] = pic.Ram[0x05];
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
            pic.Ram[0x85] = 0b1111_1111;
            pic.Ram[0x86] = 0b1111_1111;
            pic.Ram[0x87] = 0; //87 gibt es nicht
            pic.Ram[0x88] &= 0b0000_1000; // q wird zu unchanged, weil EEPROM nicht im Simulator
            pic.Ram[0x89] = 0b0000_0000;
            pic.Ram[0x8A] = 0;
            pic.Ram[0x8B] &= 0b0000_0001;
        }

        public void WakeUpFromSleep(bool interrupt)
        {
            pic.IsSleeping = false;
            pic.WReg = pic.WReg;
            pic.Ram[0x00] = 0;
            pic.Ram[0x01] = pic.Ram[0x01];
            //pic.Ram[0x02] = PC + 1;
            //pic.Ram[0x03] = uuuq_quuu; //Jonas
            if (interrupt)
            {
                setBit(4, 0x03);
                clearBit(3, 0x03);
            }
            else
            {
                clearBit(4, 0x03);
                clearBit(3, 0x03);
            }
            pic.Ram[0x04] = pic.Ram[0x04];
            pic.Ram[0x05] = pic.Ram[0x05];
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
            pic.Ram[0x85] = pic.Ram[0x85];
            pic.Ram[0x86] = pic.Ram[0x86];
            pic.Ram[0x87] = 0; //87 gibt es nicht
            pic.Ram[0x88] &= 0b0000_1111;
            pic.Ram[0x89] = pic.Ram[0x89];
            pic.Ram[0x8A] &= 0b0001_1111;
            pic.Ram[0x8B] = pic.Ram[0x8B];
        }
        public void SLEEP()
        {
            if (((pic.Ram[0x0B] & 16) == 16 && (pic.Ram[0x0B] & 2) == 2)
                || ((pic.Ram[0x0B] & 8) == 8 && (pic.Ram[0x0B] & 1) == 1))
            {
                NOP();
            }
            else
            {
                clearBit(3, 0x03);
                setBit(4, 0x03);
                if (pic.WDTActive)
                {
                    pic.WDTTimer = 0;
                    pic.WDTPrescaler = 0;
                }
                pic.IsSleeping = true;
            }
        }
        public void Watchdog()
        {
            var tmp = ((4 / (pic.AusgewaehlteQuarzfrequenz * 1e6)) * pic.WDTTimer);
            setWDTprescaler();
            if (tmp >= (0.018 * pic.WDTPrescaler))
            {
                MessageBox.Show("Watchdog Timer Reset");
                pic.WDTTimer = 0;
                if (pic.IsSleeping)
                {
                    WakeUpFromSleep(false);
                }
                else
                {
                    MCLR();
                    clearBit(4, 0x03);
                    setBit(3, 0x03);
                }
            }
        }

        private void setBit(int bit, int address)
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
                case 3:
                case 4:
                case 10:
                case 11:
                    pic.Ram[address] |= 1 << bit;
                    pic.Ram[address + 128] = pic.Ram[address];
                    break;
                case 0x80:
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
                case 2:
                case 0x82:
                    pic.Ram[2] |= 1 << bit;
                    pic.Ram[2] &= 255;
                    pic.Ram[0x82] = pic.Ram[2];
                    pic.PC = pic.Ram[2] + (pic.PCLATCH << 8);
                    break;
                default:
                    pic.Ram[address] |= 1 << bit;
                    break;
            }
        }
        private void clearBit(int bit, int address)
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
                case 3:
                case 4:
                case 10:
                case 11:
                    pic.Ram[address] &= ~(1 << bit);
                    pic.Ram[address + 128] = pic.Ram[address];
                    break;
                case 0x80:
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
                case 2:
                case 0x82:
                    pic.Ram[2] &= ~(1 << bit);
                    pic.Ram[2] &= 255;
                    pic.Ram[0x82] = pic.Ram[2];
                    pic.PC = pic.Ram[2] + (pic.PCLATCH << 8);
                    break;
                default:
                    pic.Ram[address] &= ~(1 << bit);
                    break;
            }
        }
        public void writeBit(int value, int bit, int address)
        {
            if (value == 0)
                clearBit(bit, address);
            else
                setBit(bit, address);
        }
        public void writeByte(int value, int address, bool checkbank = true)
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
                case 3:
                case 4:
                case 10:
                case 11:
                    pic.Ram[address] = value;
                    pic.Ram[address + 128] = pic.Ram[address];
                    break;
                case 0x80:
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
                case 2:
                case 0x82:
                    pic.Ram[2] = value;
                    pic.Ram[2] &= 255;
                    pic.Ram[0x82] = pic.Ram[2];
                    pic.PC = pic.Ram[2] + (pic.PCLATCH << 8);
                    break;
                default:
                    pic.Ram[address] = value;
                    break;
            }
        }
        private int readByte(int address)
        {
            if (address == 0)
            {
                address = pic.Ram[4];
            }
            else if ((pic.Ram[3] & 32) == 32)
                address += 128;
            return pic.Ram[address];
        }
        private void setZeroFlag()
        {
            pic.Ram[3] |= 4;
            pic.Ram[131] = pic.Ram[3];
        }
        private void clearZeroFlag()
        {
            pic.Ram[3] &= 251;
            pic.Ram[131] = pic.Ram[3];
        }
        private void setCarryFlag()
        {
            pic.Ram[3] |= 1;
            pic.Ram[131] = pic.Ram[3];
        }
        private void clearCarryFlag()
        {
            pic.Ram[3] &= 254;
            pic.Ram[131] = pic.Ram[3];
        }
        private void setDigitCarryFlag()
        {
            pic.Ram[3] |= 2;
            pic.Ram[131] = pic.Ram[3];
        }
        private void clearDigitCarryFlag()
        {
            pic.Ram[3] &= 253;
            pic.Ram[131] = pic.Ram[3];
        }
        public void Reset()
        {
            pic.CodeTimer = 0;
            PicTimer = 0;
        }
        private int PicTimer;
        public void IncTimer()
        {
            pic.CodeTimer++;
            if ((pic.Ram[0x81] & 32) == 0)
            {
                PicTimer++;
                setTimer();
            }
            if (pic.WDTActive)
            {
                pic.WDTTimer++;
            }
        }
        private void setTimer()
        {
            if ((pic.Ram[0x81] & 8) == 0)
            {
                switch (pic.Ram[0x81] & 7)
                {
                    case 0:
                        if (PicTimer % 2 == 0)
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
                setBit(2, 0x0B);
            pic.Ram[1] &= 255;
        }
        private int lastRA4;
        public void RA4()
        {
            if ((pic.Ram[0x81] & 32) == 32)
            {
                if ((pic.Ram[5] & 16) == 0 && lastRA4 == 1)
                {
                    PicTimer++;
                    setTimer();
                }
            }
            lastRA4 = (pic.Ram[5] & 16) >> 4;
        }
        private int lastRB0;
        public void RB0()
        {
            if ((pic.Ram[6] & 1) != lastRB0)
            {
                lastRB0 = pic.Ram[6] & 1;
                if ((pic.Ram[0x81] & 64) == 64 && lastRB0 == 1
                    || (pic.Ram[0x81] & 64) == 0 && lastRB0 == 0)
                {
                    setBit(1, 0x0B);
                }
            }
        }
        private int lastRB7_4;
        public void PORTBINT()
        {
            if ((pic.Ram[6] & 0xF0) != lastRB7_4)
            {
                lastRB7_4 = pic.Ram[6] & 0xF0;
                if ((pic.Ram[0x81] & 8) == 8)
                {
                    setBit(0, 0x0B);
                }
            }
        }
        public void InterruptTest()
        {
            if ((pic.Ram[0x8B] & 128) == 128) // GIE prüfen
            {
                if ((pic.Ram[0x8B] & 32) == 32) // TOIE prüfen
                {
                    if ((pic.Ram[0x8B] & 4) == 4) // TOIF prüfen
                    {
                        clearBit(7, 0x0B); // GIE löschen
                        pic.Stack[pic.StackPointer] = pic.PC;
                        pic.StackPointer++;
                        if (pic.StackPointer > 7)
                            pic.StackPointer = 0;
                        pic.PC = 4;
                    }
                }
                if ((pic.Ram[0x8B] & 16) == 16) // INTE prüfen
                {
                    if ((pic.Ram[0x8B] & 2) == 2) // INTF prüfen
                    {
                        clearBit(7, 0x0B); // GIE löschen
                        pic.Stack[pic.StackPointer] = pic.PC;
                        pic.StackPointer++;
                        if (pic.StackPointer > 7)
                            pic.StackPointer = 0;
                        pic.PC = 4;
                    }
                }
                if ((pic.Ram[0x8B] & 8) == 8) // RBIE prüfen
                {
                    if ((pic.Ram[0x8B] & 1) == 1) // RBIF prüfen
                    {
                        clearBit(7, 0x0B); // GIE löschen
                        pic.Stack[pic.StackPointer] = pic.PC;
                        pic.StackPointer++;
                        if (pic.StackPointer > 7)
                            pic.StackPointer = 0;
                        pic.PC = 4;
                    }
                }
            }
        }
        public void WakeUpTest()
        {
            if ((pic.Ram[0x8B] & 16) == 16) // INTE prüfen
            {
                if ((pic.Ram[0x8B] & 2) == 2) // INTF prüfen
                {
                    if ((pic.Ram[0x8B] & 128) == 128) // GIE prüfen
                    {
                        WakeUpFromSleep(true);
                    }
                    else
                    {
                        WakeUpFromSleep(true);
                    }
                }
            }
            if ((pic.Ram[0x8B] & 8) == 8) // RBIE prüfen
            {
                if ((pic.Ram[0x8B] & 1) == 1) // RBIF prüfen
                {
                    if ((pic.Ram[0x8B] & 128) == 128) // GIE prüfen
                    {
                        WakeUpFromSleep(true);
                    }
                    else
                    {
                        WakeUpFromSleep(true);
                    }
                }
            }
        }
        private void setWDTprescaler()
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
