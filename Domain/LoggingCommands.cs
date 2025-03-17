using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSimulator
{
    public class LoggingCommands : ICommandsDecorator
    {
        public ICommands _innerCommands { get; }

        public LoggingCommands(ICommands innerCommands)
        {
            _innerCommands = innerCommands;
        }

        public void ADDLW(int literal)
        {
            Console.WriteLine($"Method: ADDLW, Parameter: {literal}");
            _innerCommands.ADDLW(literal);
        }

        public void ADDWF(int file)
        {
            Console.WriteLine($"Method: ADDWF, Parameter: {file}");
            _innerCommands.ADDWF(file);
        }

        public void ANDLW(int literal)
        {
            Console.WriteLine($"Method: ANDLW, Parameter: {literal}");
            _innerCommands.ANDLW(literal);
        }

        public void ANDWF(int file)
        {
            Console.WriteLine($"Method: ANDWF, Parameter: {file}");
            _innerCommands.ANDWF(file);
        }

        public void BCF(int arg)
        {
            Console.WriteLine($"Method: BCF, Parameter: {arg}");
            _innerCommands.BCF(arg);
        }

        public void BSF(int arg)
        {
            Console.WriteLine($"Method: BSF, Parameter: {arg}");
            _innerCommands.BSF(arg);
        }

        public void BTFSC(int arg)
        {
            Console.WriteLine($"Method: BTFSC, Parameter: {arg}");
            _innerCommands.BTFSC(arg);
        }

        public void BTFSS(int arg)
        {
            Console.WriteLine($"Method: BTFSS, Parameter: {arg}");
            _innerCommands.BTFSS(arg);
        }

        public void CALL(int literal)
        {
            Console.WriteLine($"Method: CALL, Parameter: {literal}");
            _innerCommands.CALL(literal);
        }

        public void CLRF(int file)
        {
            Console.WriteLine($"Method: CLRF, Parameter: {file}");
            _innerCommands.CLRF(file);
        }

        public void CLRW(int file)
        {
            Console.WriteLine($"Method: CLRW, Parameter: {file}");
            _innerCommands.CLRW(file);
        }

        public void COMF(int file)
        {
            Console.WriteLine($"Method: COMF, Parameter: {file}");
            _innerCommands.COMF(file);
        }

        public void DECF(int file)
        {
            Console.WriteLine($"Method: DECF, Parameter: {file}");
            _innerCommands.DECF(file);
        }

        public void DECFSZ(int file)
        {
            Console.WriteLine($"Method: DECFSZ, Parameter: {file}");
            _innerCommands.DECFSZ(file);
        }

        public void GOTO(int literal)
        {
            Console.WriteLine($"Method: GOTO, Parameter: {literal}");
            _innerCommands.GOTO(literal);
        }

        public void INCF(int file)
        {
            Console.WriteLine($"Method: INCF, Parameter: {file}");
            _innerCommands.INCF(file);
        }

        public void INCFSZ(int file)
        {
            Console.WriteLine($"Method: INCFSZ, Parameter: {file}");
            _innerCommands.INCFSZ(file);
        }

        public void IORLW(int literal)
        {
            Console.WriteLine($"Method: IORLW, Parameter: {literal}");
            _innerCommands.IORLW(literal);
        }

        public void IORWF(int file)
        {
            Console.WriteLine($"Method: IORWF, Parameter: {file}");
            _innerCommands.IORWF(file);
        }

        public void MCLR()
        {
            Console.WriteLine("Method: MCLR");
            _innerCommands.MCLR();
        }

        public void MOVF(int file)
        {
            Console.WriteLine($"Method: MOVF, Parameter: {file}");
            _innerCommands.MOVF(file);
        }

        public void MOVLW(int literal)
        {
            Console.WriteLine($"Method: MOVLW, Parameter: {literal}");
            _innerCommands.MOVLW(literal);
        }

        public void MOVWF(int file)
        {
            Console.WriteLine($"Method: MOVWF, Parameter: {file}");
            _innerCommands.MOVWF(file);
        }

        public void NOP()
        {
            Console.WriteLine("Method: NOP");
            _innerCommands.NOP();
        }

        public void PowerOnReset()
        {
            Console.WriteLine("Method: PowerOnReset");
            _innerCommands.PowerOnReset();
        }

        public void Reset()
        {
            Console.WriteLine("Method: Reset");
            _innerCommands.Reset();
        }

        public void RETFIE()
        {
            Console.WriteLine("Method: RETFIE");
            _innerCommands.RETFIE();
        }

        public void RETLW(int literal)
        {
            Console.WriteLine($"Method: RETLW, Parameter: {literal}");
            _innerCommands.RETLW(literal);
        }

        public void RETURN()
        {
            Console.WriteLine("Method: RETURN");
            _innerCommands.RETURN();
        }

        public void RLF(int file)
        {
            Console.WriteLine($"Method: RLF, Parameter: {file}");
            _innerCommands.RLF(file);
        }

        public void RRF(int file)
        {
            Console.WriteLine($"Method: RRF, Parameter: {file}");
            _innerCommands.RRF(file);
        }

        public void SLEEP()
        {
            Console.WriteLine("Method: SLEEP");
            _innerCommands.SLEEP();
        }

        public void SUBLW(int literal)
        {
            Console.WriteLine($"Method: SUBLW, Parameter: {literal}");
            _innerCommands.SUBLW(literal);
        }

        public void SUBWF(int file)
        {
            Console.WriteLine($"Method: SUBWF, Parameter: {file}");
            _innerCommands.SUBWF(file);
        }

        public void SWAPF(int file)
        {
            Console.WriteLine($"Method: SWAPF, Parameter: {file}");
            _innerCommands.SWAPF(file);
        }

        public void WakeUpFromSleep(bool interrupt)
        {
            Console.WriteLine($"Method: WakeUpFromSleep, Parameter: {interrupt}");
            _innerCommands.WakeUpFromSleep(interrupt);
        }

        public void XORLW(int literal)
        {
            Console.WriteLine($"Method: XORLW, Parameter: {literal}");
            _innerCommands.XORLW(literal);
        }

        public void XORWF(int file)
        {
            Console.WriteLine($"Method: XORWF, Parameter: {file}");
            _innerCommands.XORWF(file);
        }

        public void IncTimer()
        {
            _innerCommands.IncTimer();
        }

        public void InterruptTest()
        {
            _innerCommands.InterruptTest();
        }

        public void PORTBINT()
        {
            _innerCommands.PORTBINT();
        }

        public void RA4()
        {
            _innerCommands.RA4();
        }

        public void RB0()
        {
            _innerCommands.RB0();
        }

        public void WakeUpTest()
        {
            _innerCommands.WakeUpTest();
        }

        public void Watchdog()
        {
            _innerCommands.Watchdog();
        }

        public void writeBit(int value, int bit, int address)
        {
            _innerCommands.writeBit(value, bit, address);
        }

        public void writeByte(int value, int address, bool checkbank = true)
        {
            _innerCommands.writeByte(value, address, checkbank);
        }
    }
}
