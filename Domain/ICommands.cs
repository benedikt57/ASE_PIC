﻿namespace PicSimulator
{
    public interface ICommands
    {
        void ADDLW(int literal);
        void ADDWF(int file);
        void ANDLW(int literal);
        void ANDWF(int file);
        void BCF(int arg);
        void BSF(int arg);
        void BTFSC(int arg);
        void BTFSS(int arg);
        void CALL(int literal);
        void CLRF(int file);
        void CLRW(int file);
        void COMF(int file);
        void DECF(int file);
        void DECFSZ(int file);
        void GOTO(int literal);
        void INCF(int file);
        void INCFSZ(int file);
        void IncTimer();
        void InterruptTest();
        void IORLW(int literal);
        void IORWF(int file);
        void MCLR();
        void MOVF(int file);
        void MOVLW(int literal);
        void MOVWF(int file);
        void NOP();
        void PORTBINT();
        void PowerOnReset();
        void RA4();
        void RB0();
        void Reset();
        void RETFIE();
        void RETLW(int literal);
        void RETURN();
        void RLF(int file);
        void RRF(int file);
        void SLEEP();
        void SUBLW(int literal);
        void SUBWF(int file);
        void SWAPF(int file);
        void WakeUpFromSleep(bool interrupt);
        void WakeUpTest();
        void Watchdog();
        void writeBit(int value, int bit, int address);
        void writeByte(int value, int address, bool checkbank = true);
        void XORLW(int literal);
        void XORWF(int file);
    }
}