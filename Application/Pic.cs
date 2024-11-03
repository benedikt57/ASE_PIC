using Domain;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace PicSimulator
{
    public class Pic : INotifyPropertyChanged, IPic
    {
        public IPicViewModel picViewModel;
        public int[] Ram {get => picViewModel.Ram;set => picViewModel.Ram = value; }
        public int[] Stack { get => picViewModel.Stack; set => picViewModel.Stack = value; }
        public int StackPointer { get => picViewModel.StackPointer; set => picViewModel.StackPointer = value; }
        public int PC { get => picViewModel.PC; set => picViewModel.PC = value; }
        public int PCLATCH { get => picViewModel.PCLATCH; set => picViewModel.PCLATCH = value; }
        public int WReg { get => picViewModel.WReg; set => picViewModel.WReg = value; }
        public int CodeTimer { get => picViewModel.CodeTimer; set => picViewModel.CodeTimer = value; }
        public bool WDTActive { get => picViewModel.WDTActive; set => picViewModel.WDTActive = value; }
        public int WDTTimer { get => picViewModel.WDTTimer; set => picViewModel.WDTTimer = value; }
        public int WDTPrescaler { get => picViewModel.WDTPrescaler; set => picViewModel.WDTPrescaler = value; }
        public double AusgewaehlteQuarzfrequenz { get => picViewModel.AusgewaehlteQuarzfrequenz; set => picViewModel.AusgewaehlteQuarzfrequenz = value; }
        public bool IsSleeping { get => picViewModel.IsSleeping; set => picViewModel.IsSleeping = value; }

        public Commands commands;
        private int activLine = 0;

        public Pic(IPicViewModel viewModel)
        {
            //Ram[0x81] = 0xFF;
            //Ram[0x85] = 0xFF;
            //Ram[0x86] = 0xFF;
            picViewModel = viewModel;
            commands = new Commands(picViewModel);
            commands.PowerOnReset();
        }
        public void LoadFile()
        {
            picViewModel.Code.Clear();
            activLine = 0;
            commands.Reset();
            string filename = string.Empty;
            // Konfiguriere das Dialogfeld "Datei öffnen"
            var dialog = new OpenFileDialog();
            dialog.FileName = "Document"; // Standarddateiname
            dialog.DefaultExt = ".LST"; // Standarddateierweiterung
            dialog.Filter = "Textdokumente (.LST)|*.LST"; // Filter für Dateierweiterungen

            // Zeige das Dialogfeld "Datei öffnen" an
            bool? result = dialog.ShowDialog();

            // Verarbeite die Ergebnisse des Dialogfelds
            if (result == true)
            {
                // Der ausgewählte Dateipfad
                filename = dialog.FileName;
                // Verwende den Dateipfad in deiner Anwendung
                SourceFilePath = filename;
                Load(filename);
            }
        
        }
        
        private string _sourceFilePath;
        public string SourceFilePath
        {
            get { return _sourceFilePath; }
        set
        {
            _sourceFilePath = value;
            OnPropertyChanged(nameof(SourceFilePath));
                }
        }

        private void Load(string path)
        {
            try
            {
                using (var sr = new StreamReader(path, Encoding.Default))
                {
                    while (!sr.EndOfStream)
                    {
                        Match match = Regex.Match(sr.ReadLine(), @"^ *(([0-9A-F]{4}) ([0-9A-F]{4}) *)?([0-9]{5}) *(.*)$");
                        if(match.Success)
                        {
                            CodeLine line = new CodeLine();
                            line.Code = (match.Groups[2].Value.PadRight(8) +
                                match.Groups[3].Value.PadRight(8) +
                                match.Groups[4].Value.PadRight(9) +
                                match.Groups[5].Value);
                            if (match.Groups[1].Success)
                            {
                                line.ProgAdrress = Convert.ToInt32(match.Groups[2].Value, 16);
                                line.HexCode = Convert.ToInt32(match.Groups[3].Value, 16);
                            }else
                            {
                                line.ProgAdrress = -1;
                                line.HexCode = -1;
                            }
                            picViewModel.Code.Add(line);
                        }
                    }
                }
                CodeTimer = 0;
                WDTTimer = 0;
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public bool Step()
        {
            if (!IsSleeping)
            {
                do
                {
                    picViewModel.Code[activLine].IsHighlighted = false;
                    activLine++;
                    if(activLine >= picViewModel.Code.Count)
                        activLine = 0;
                } while (picViewModel.Code[activLine].ProgAdrress != PC);
                PC++;
                picViewModel.Code[activLine].IsHighlighted = true;
                Decode(picViewModel.Code[activLine].HexCode);
                commands.InterruptTest(); //Interrupt prüfen
            }
            else
            {
                commands.IncTimer();
            }

            commands.RA4(); //RA4 prüfen um Timer zu zählen
            commands.RB0(); //RB0 Flag setzten
            commands.PORTBINT(); //PORTBINT Flag setzten
            commands.Watchdog(); //Watchdog prüfen
            commands.WakeUpTest();
            if (picViewModel.Code[activLine].Breakpoint)
            {
                return false;
            }
            return true;
        }
        private void Decode(int code)
        {
            //erste 3 Bit Maskieren
            int opcode = code & 0b0011_1000_0000_0000;
            switch (opcode)
            {
                case 0b0010_1000_0000_0000:
                    commands.GOTO(code & 0b0000_0111_1111_1111);
                    return;
                case 0b0010_0000_0000_0000:
                    commands.CALL(code & 0b0000_0111_1111_1111);
                    return;
            }
            //erste 4 Bit Maskieren
            opcode = code & 0b0011_1100_0000_0000;
            switch (opcode)
            {
                case 0b0001_0100_0000_0000:
                    commands.BSF(code & 0b0000_0011_1111_1111);
                    return;
                case 0b0001_0000_0000_0000:
                    commands.BCF(code & 0b0000_0011_1111_1111);
                    return;
                case 0b0001_1100_0000_0000:
                    commands.BTFSS(code & 0b0000_0011_1111_1111);
                    return;
                case 0b0001_1000_0000_0000:
                    commands.BTFSC(code & 0b0000_0011_1111_1111);
                    return;
            }
            //erste 6 Bit Maskieren
            opcode = code & 0b0011_1111_0000_0000;
            switch (opcode)
            {
                case 0b0011_0000_0000_0000:
                    commands.MOVLW(code & 0b0000_0000_1111_1111);
                    return;
                case 0b0011_1001_0000_0000:
                    commands.ANDLW(code & 0b0000_0000_1111_1111);
                    return;
                case 0b0011_1000_0000_0000:
                    commands.IORLW(code & 0b0000_0000_1111_1111);
                    return;
                case 0b0011_1100_0000_0000:
                    commands.SUBLW(code & 0b0000_0000_1111_1111);
                    return;
                case 0b0011_1010_0000_0000:
                    commands.XORLW(code & 0b0000_0000_1111_1111);
                    break;
                case 0b0011_1110_0000_0000:
                    commands.ADDLW(code & 0b0000_0000_1111_1111);
                    break;
                case 0b0011_0100_0000_0000:
                    commands.RETLW(code & 0b0000_0000_1111_1111);
                    return;
                case 0b0000_0111_0000_0000:
                    commands.ADDWF(code & 0b0000_0000_1111_1111);
                    return;
                case 0b0000_0101_0000_0000:
                    commands.ANDWF(code & 0b0000_0000_1111_1111);
                    return;
                case 0b0000_1001_0000_0000:
                    commands.COMF(code & 0b0000_0000_1111_1111);
                    return;
                case 0b0000_0011_0000_0000:
                    commands.DECF(code & 0b0000_0000_1111_1111);
                    return;
                case 0b0000_1010_0000_0000:
                    commands.INCF(code & 0b0000_0000_1111_1111);
                    return;
                case 0b0000_1000_0000_0000:
                    commands.MOVF(code & 0b0000_0000_1111_1111);
                    return;
                case 0b0000_0100_0000_0000:
                    commands.IORWF(code & 0b0000_00000_1111_1111);
                    return;
                case 0b0000_0010_0000_0000:
                    commands.SUBWF(code & 0b0000_0000_1111_1111);
                    return;
                case 0b0000_1110_0000_0000:
                    commands.SWAPF(code & 0b0000_0000_1111_1111);
                    return;
                case 0b0000_0110_0000_0000:
                    commands.XORWF(code & 0b0000_0000_1111_1111);
                    return;
                case 0b0000_1101_0000_0000:
                    commands.RLF(code & 0b0000_0000_1111_1111);
                    return;
                case 0b0000_1100_0000_0000:
                    commands.RRF(code & 0b0000_0000_1111_1111);
                    return;
                case 0b0000_1011_0000_0000:
                    commands.DECFSZ(code & 0b0000_0000_1111_1111);
                    return;
                case 0b0000_1111_0000_0000:
                    commands.INCFSZ(code & 0b0000_0000_1111_1111);
                    return;
            }
            //erste 7 Bit Maskieren
            opcode = code & 0b0011_1111_1000_0000;
            switch (opcode)
            {
                case 0b0000_0000_1000_0000:
                    commands.MOVWF(code & 0b0000_0000_0111_1111);
                    return;
                case 0b0000_0001_1000_0000:
                    commands.CLRF(code & 0b0000_0000_0111_1111);
                    return;
                case 0b0000_0001_0000_0000:
                    commands.CLRW(code & 0b0000_0000_0111_1111);
                    return;
            }
            //erste 14 Bit Maskieren
            opcode = code;
            switch (opcode)
            {
                case 0b0000_0000_0000_1000:
                    commands.RETURN();
                    return;
                case 0b0000_0000_0000_1001:
                    commands.RETFIE();
                    return;
                case 0b0000_0000_0110_0011:
                    commands.SLEEP();
                    return;
                case 0b0000_0000_0000_0000:
                    commands.NOP();
                    return;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //Array
        //public void BoolArray()
        //{
        //    int[] array = new int[8];
        //    array[0] = 1;
        //    array[1] = 0;
        //    array[2] = 1;
        //    array[3] = 0;
        //    array[5] = 1;
        //    array[6] = 0;
        //    array[7] = 1;
        //}
    }
}
