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
    public class Pic : INotifyPropertyChanged
    {
        private int activLine = 0;

        private ObservableCollection<CodeLine> code = new ObservableCollection<CodeLine>();
        public ObservableCollection<CodeLine> Code
        {
            get { return code; }
            set
            {
                code = value;
                OnPropertyChanged(nameof(Code));
            }
        }
        private int[] ram = new int[256];
        public int[] Ram
        {
            get { return ram; }
            set
            {
                ram = value;
                OnPropertyChanged(nameof(Ram));
            }
        }
        private int wReg;
        public int WReg
        {
            get { return wReg; }
            set
            {
                wReg = value;
                OnPropertyChanged(nameof(WReg));
            }
        }
        private string testString = "blabla";
        public string TestString
        {
            get { return testString; }
            set
            {
                testString = value;
                OnPropertyChanged(nameof(TestString));
            }
        }
        private int codeTimer = 8;

        public int CodeTimer
        {
            get { return codeTimer; }
            set
            {
                codeTimer = value;
                OnPropertyChanged(nameof(CodeTimer));
            }
        }

        


        public Pic()
        {
        }
        public void LoadFile()
        {
            Code.Clear();
            activLine = 0;
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
            }
            Load(filename);
        
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
        




        public void ChangeString()
        {
            TestString = "Hallo";
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
                            Code.Add(line);
                        }
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void Step()
        {
            do
            {
                if (activLine >= Code.Count - 1)
                    return;
                Code[activLine].IsHighlighted = false;
                activLine++;
                Code[activLine].IsHighlighted = true;
                CodeTimer = CodeTimer +1 ;
            } while (Code[activLine].ProgAdrress == -1);
            Decode(Code[activLine].HexCode);
        }
        private void Decode(int code)
        {
            //erste 6 Bit Maskieren
            int opcode = code & 0b0011_1111_0000_0000;
            switch (opcode)
            {
                case 0b0011_0000_0000_0000:
                    Commands.MOVLW(code & 0b0000_0000_1111_1111, this);
                    CodeTimer = CodeTimer + 1;
                    return;
                case 0b0011_1001_0000_0000:
                    Commands.ANDLW(code & 0b0000_0000_1111_1111, this);
                    CodeTimer = CodeTimer + 1;
                    return;
                case 0b0011_1000_0000_0000:
                    Commands.IORLW(code & 0b0000_0000_1111_1111, this);
                    CodeTimer = CodeTimer + 1;
                    return;
                case 0b0011_1100_0000_0000:
                    Commands.SUBLW(code & 0b0000_0000_1111_1111, this);
                    CodeTimer = CodeTimer + 1;
                    return;
                case 0b0011_1010_0000_0000:
                    Commands.XORLW(code & 0b0000_0000_1111_1111, this);
                    CodeTimer = CodeTimer + 1;  
                    break;
                case 0b0011_1110_0000_0000:
                    Commands.ADDLW(code & 0b0000_0000_1111_1111, this);
                    CodeTimer = CodeTimer + 1;
                    return;
                default:
                    return;
            }
            //erste 7 Bit Maskieren
            opcode = code & 0b0011_1111_1000_0000;
            switch (opcode)
            {
                case 0b0010_0000_0000_0000:
                    MessageBox.Show("MOVLB");
                    break;
                default:
                    break;
            }
        }


        //private void Decode(int code)
        //{
        //    //erste 6 Bit Maskieren
        //    int opcode = code & 0b0011_1111_0000_0000;
        //    switch (opcode)
        //    {
        //        case 0b0011_0000_0000_0000:
        //            Commands.MOVLW(code & 0b0000_0000_1111_1111, this);
        //            CodeTimer = CodeTimer + 1;
        //            return;
        //        case 0b00_0111_0000_0000:
        //            Commands.ADDWF(code & 0b0000_0000_1111_1111, this);
        //            CodeTimer = CodeTimer + 1;
        //            return;
        //        case 0b00_0101_0000_0000:
        //            Commands.ANDWF(code & 0b0000_0000_1111_1111, this);
        //            CodeTimer = CodeTimer + 1;
        //            return;
        //        case 0b00_0001_0000_0000:
        //                Commands.CLRF(code & 0b0000_0000_1111_1111, this);
        //            CodeTimer = CodeTimer + 1;
        //            return;
        //        case 0b00_0001_0000_0000:
        //            Commands.CLRW(code & 0b0000_0000_1111_1111, this);
        //            CodeTimer = CodeTimer + 1;
        //            return;
        //        case 0b00_1001_0000_0000:
        //            Commands.COMF(code & 0b0000_0000_1111_1111, this);
        //            CodeTimer = CodeTimer + 1;
        //            return;
        //         case 0b00_0011_0000_0000:
        //             Commands.DECF(code & 0b0000_0000_1111_1111, this);
        //             CodeTimer = CodeTimer + 1;
        //             return;
        //         case 0b00_1011_0000_0000:
        //             Commands.DECFSZ(code & 0b0000_0000_1111_1111, this);
        //             CodeTimer = CodeTimer + 2; //1(2)
        //             return;
        //         case 0b00_1010_0000_0000:
        //             Commands.INCF(code & 0b0000_0000_1111_1111, this);
        //             CodeTimer = CodeTimer + 1;
        //             return;
        //         case 0b00_1111_0000_0000:
        //             Commands.INCFSZ(code & 0b0000_0000_1111_1111, this);
        //             CodeTimer = CodeTimer + 2; //1(2)
        //             return;
        //         case 0b00_0100_0000_0000:
        //             Commands.IORWF(code & 0b0000_0000_1111_1111, this);
        //             CodeTimer = CodeTimer + 1;
        //             return;
        //         case 0b00_1000_0000_0000:
        //             Commands.MOVF(code & 0b0000_0000_1111_1111, this);
        //             CodeTimer = CodeTimer + 1;
        //             return;
        //         case 0b00_0000_1000_0000:
        //             Commands.MOVWF(code & 0b0000_0000_1111_1111, this);
        //             CodeTimer = CodeTimer + 1;
        //             return;
        //         case 0b00_0000_0000_0000:
        //            Commands.NOP(code & 0b0000_0000_1111_1111, this);
        //            CodeTimer = CodeTimer + 1;
        //            return;
        //         case 0b00_1101_0000_0000:
        //            Commands.RLF(code & 0b0000_0000_1111_1111, this);
        //            CodeTimer = CodeTimer + 1;
        //            return;
        //         case 0b00_1100_0000_0000:
        //            Commands.RRF(code & 0b0000_0000_1111_1111, this);
        //            CodeTimer = CodeTimer + 1;
        //            return;
        //         case 0b00_0010_0000_0000:
        //            Commands.SUBWF(code & 0b0000_0000_1111_1111, this);
        //            CodeTimer = CodeTimer + 1;
        //            return;
        //         case 0b00_1110_0000_0000:
        //            Commands.SWAPF(code & 0b0000_0000_1111_1111, this);
        //            CodeTimer = CodeTimer + 1;
        //            return;
        //         case 0b00_0110_0000_0000:
        //            Commands.XORWF(code & 0b0000_0000_1111_1111, this);
        //            CodeTimer = CodeTimer + 1;
        //            return;

        //         default:
        //            return;
        //            }


        //            Alle Befehle die ich gefunden habe:                       Hier Hinten mit den Zeiten
        //    //BYTE-ORIENTED FILE REGISTER OPERATIONS         
        //{"ADDWF", "00 0111 dfff ffff"},                       Zeit:1
        //{"ANDWF", "00 0101 dfff ffff"},                       Zeit: 1
        //{ "CLRF", "00 0001 1fff ffff "},                      Zeit: 1
        //{ "CLRW", "00 0001 oxxx xxxx"},                       Zeit: 1
        //{ "COMF", "00 1001 dfff ffff"},                       Zeit: 1
        //{ "DECF", "00 0011 dfff ffff"},                       Zeit: 1
        //{ "DECFSZ", "00 1011 dfff ffff"},                     Zeit: 1(2)
        //{ "INCF", "00 1010 dfff ffff"},                       Zeit: 1
        //{ "INCFSZ", "00 1111 dfff ffff"},                     Zeit: 1(2)
        //{ "IORWF", "00 0100 dfff ffff"},                      Zeit: 1
        //{ "MOVF", "00 1000 dfff ffff"},                       Zeit: 1
        //{ "MOVWF", "00 000 1fff ffff"},                       Zeit: 1
        //{ "NOP", "00 0000 0xx0 0000"},                        Zeit: 1
        //{ "RLF", "00 1101 dfff ffff"},                        Zeit: 1
        //{ "RRF", "00 1100 dfff ffff"},                        Zeit: 1
        //{ "SUBWF", "00 0010 dfff ffff"},                      Zeit: 1
        //{ "SWAPF", "00 1110 dfff ffff"},                      Zeit: 1
        //{ "XORWF", "00 0110 dfff ffff"},                      Zeit: 1
        //                                                      Zeit: 1
        ////BIT-ORIENTED FILE REGISTER OPERATIONS               Zeit:1
        //{ "BCF", "01 00bb bfff ffff"},                        Zeit: 1
        //{ "BSF", "01 01bb bfff ffff"},                        Zeit: 1
        //{ "BTFSC", "01 10bb bfff ffff"},                      Zeit: 1(2)
        //{ "BTFSS", "01 11bb bfff ffff"},                      Zeit: 1(2)
        //                                                      Zeit: 1
        ////Literal and control operation                       Zeit:1
        //{ "ADDLW", "11 111x kkkk kkkk"},                      Zeit: 1
        //{ "ANDLW", "11 1001 kkkk kkkk"},                      Zeit: 1
        //{ "CALL", "10 0kkk kkkk kkkk"},                       Zeit: 2
        //{ "CLRWDT", "00 0000 0110 0100"},                     Zeit: 1
        //{ "GOTO", "10 1kkkk kkkk kkkk"},                      Zeit: 2
        //{ "IORLW", "11 1000 kkkk kkkk" },                     Zeit: 1
        //{ "MOVLW", "11 00xx kkkk kkkk" },                     Zeit: 1
        //{ "RETFIE", "00 0000 0000 1001" },                    Zeit: 2
        //{ "RETLW", "11 01xx kkkk kkkk" },                     Zeit: 2
        //{ "RETURN", "00 0000 0000 1000" },                    Zeit: 2
        //{ "SLEEP", "00 0000 0110 0011" },                     Zeit: 1
        //{ "SUBLW", "11 110x kkkk kkkk" },                     Zeit: 1
        //{ "XORLW", "11 1010 kkkk kkkk" },                     Zeit: 1




        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //Array
        public void BoolArray()
        {
            int[] array = new int[8];
            array[0] = 1;
            array[1] = 0;
            array[2] = 1;
            array[3] = 0;
            array[5] = 1;
            array[6] = 0;
            array[7] = 1;
        }
    }
}
