using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSimulator
{
    public class LoggingCommands : ICommandsDecorator
    {
        public ICommands _inner { get; }

        public LoggingCommands(ICommands innerCommands)
        {
            _inner = innerCommands;
        }

        public void ADDLW(int literal)
        {
            Console.WriteLine($"Method: ADDLW, Parameter: {literal}");
            _inner.ADDLW(literal);
        }

        public void ADDWF(int file)
        {
            Console.WriteLine($"Method: ADDWF, Parameter: {file}");
            _inner.ADDWF(file);
        }

        public void ANDLW(int literal)
        {
            Console.WriteLine($"Method: ANDLW, Parameter: {literal}");
            _inner.ANDLW(literal);
        }

        public void ANDWF(int file)
        {
            Console.WriteLine($"Method: ANDWF, Parameter: {file}");
            _inner.ANDWF(file);
        }

        public void BCF(int arg)
        {
            Console.WriteLine($"Method: BCF, Parameter: {arg}");
            _inner.BCF(arg);
        }

        public void BSF(int arg)
        {
            Console.WriteLine($"Method: BSF, Parameter: {arg}");
            _inner.BSF(arg);
        }

        public void BTFSC(int arg)
        {
            Console.WriteLine($"Method: BTFSC, Parameter: {arg}");
            _inner.BTFSC(arg);
        }

        public void BTFSS(int arg)
        {
            Console.WriteLine($"Method: BTFSS, Parameter: {arg}");
            _inner.BTFSS(arg);
        }

        public void CALL(int literal)
        {
            Console.WriteLine($"Method: CALL, Parameter: {literal}");
            _inner.CALL(literal);
        }

        public void CLRF(int file)
        {
            Console.WriteLine($"Method: CLRF, Parameter: {file}");
            _inner.CLRF(file);
        }

        public void CLRW(int file)
        {
            Console.WriteLine($"Method: CLRW, Parameter: {file}");
            _inner.CLRW(file);
        }

        public void COMF(int file)
        {
            Console.WriteLine($"Method: COMF, Parameter: {file}");
            _inner.COMF(file);
        }

        public void DECF(int file)
        {
            Console.WriteLine($"Method: DECF, Parameter: {file}");
            _inner.DECF(file);
        }

        public void DECFSZ(int file)
        {
            Console.WriteLine($"Method: DECFSZ, Parameter: {file}");
            _inner.DECFSZ(file);
        }

        public void GOTO(int literal)
        {
            Console.WriteLine($"Method: GOTO, Parameter: {literal}");
            _inner.GOTO(literal);
        }

        public void INCF(int file)
        {
            Console.WriteLine($"Method: INCF, Parameter: {file}");
            _inner.INCF(file);
        }

        public void INCFSZ(int file)
        {
            Console.WriteLine($"Method: INCFSZ, Parameter: {file}");
            _inner.INCFSZ(file);
        }

        public void IORLW(int literal)
        {
            Console.WriteLine($"Method: IORLW, Parameter: {literal}");
            _inner.IORLW(literal);
        }

        public void IORWF(int file)
        {
            Console.WriteLine($"Method: IORWF, Parameter: {file}");
            _inner.IORWF(file);
        }

        public void MCLR()
        {
            Console.WriteLine("Method: MCLR");
            _inner.MCLR();
        }

        public void MOVF(int file)
        {
            Console.WriteLine($"Method: MOVF, Parameter: {file}");
            _inner.MOVF(file);
        }

        public void MOVLW(int literal)
        {
            Console.WriteLine($"Method: MOVLW, Parameter: {literal}");
            _inner.MOVLW(literal);
        }

        public void MOVWF(int file)
        {
            Console.WriteLine($"Method: MOVWF, Parameter: {file}");
            _inner.MOVWF(file);
        }

        public void NOP()
        {
            Console.WriteLine("Method: NOP");
            _inner.NOP();
        }

        public void PowerOnReset()
        {
            Console.WriteLine("Method: PowerOnReset");
            _inner.PowerOnReset();
        }

        public void Reset()
        {
            Console.WriteLine("Method: Reset");
            _inner.Reset();
        }

        public void RETFIE()
        {
            Console.WriteLine("Method: RETFIE");
            _inner.RETFIE();
        }

        public void RETLW(int literal)
        {
            Console.WriteLine($"Method: RETLW, Parameter: {literal}");
            _inner.RETLW(literal);
        }

        public void RETURN()
        {
            Console.WriteLine("Method: RETURN");
            _inner.RETURN();
        }

        public void RLF(int file)
        {
            Console.WriteLine($"Method: RLF, Parameter: {file}");
            _inner.RLF(file);
        }

        public void RRF(int file)
        {
            Console.WriteLine($"Method: RRF, Parameter: {file}");
            _inner.RRF(file);
        }

        public void SLEEP()
        {
            Console.WriteLine("Method: SLEEP");
            _inner.SLEEP();
        }

        public void SUBLW(int literal)
        {
            Console.WriteLine($"Method: SUBLW, Parameter: {literal}");
            _inner.SUBLW(literal);
        }

        public void SUBWF(int file)
        {
            Console.WriteLine($"Method: SUBWF, Parameter: {file}");
            _inner.SUBWF(file);
        }

        public void SWAPF(int file)
        {
            Console.WriteLine($"Method: SWAPF, Parameter: {file}");
            _inner.SWAPF(file);
        }

        public void WakeUpFromSleep(bool interrupt)
        {
            Console.WriteLine($"Method: WakeUpFromSleep, Parameter: {interrupt}");
            _inner.WakeUpFromSleep(interrupt);
        }

        public void XORLW(int literal)
        {
            Console.WriteLine($"Method: XORLW, Parameter: {literal}");
            _inner.XORLW(literal);
        }

        public void XORWF(int file)
        {
            Console.WriteLine($"Method: XORWF, Parameter: {file}");
            _inner.XORWF(file);
        }

        public void IncTimer()
        {
            _inner.IncTimer();
        }

        public void InterruptTest()
        {
            _inner.InterruptTest();
        }

        public void PORTBINT()
        {
            _inner.PORTBINT();
        }

        public void RA4()
        {
            _inner.RA4();
        }

        public void RB0()
        {
            _inner.RB0();
        }

        public void WakeUpTest()
        {
            _inner.WakeUpTest();
        }

        public void Watchdog()
        {
            _inner.Watchdog();
        }

        public void writeBit(int value, int bit, int address)
        {
            _inner.writeBit(value, bit, address);
        }

        public void writeByte(int value, int address, bool checkbank = true)
        {
            _inner.writeByte(value, address, checkbank);
        }
    }
}
