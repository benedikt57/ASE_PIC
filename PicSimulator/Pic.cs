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
        private int[] stack = new int[8];
        public int[] Stack
        {
            get { return stack; }
            set
            {
                stack = value;
                OnPropertyChanged(nameof(Stack));
            }
        }
        public int StackPointer { get; set; }
        private int pcl;
        public int PCL
        {
            get { return pcl; }
            set
            {
                pcl = value;
                Ram[2] = pcl & 255;
                Ram[130] = Ram[2];
                OnPropertyChanged(nameof(Ram));
                OnPropertyChanged(nameof(PCL));
            }
        }
        private int wReg;
        public int WReg
        {
            get { return wReg; }
            set
            {
                wReg = value & 255;
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
        private int codeTimer;

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
        public bool Step()
        {
            do
            {
                Code[activLine].IsHighlighted = false;
                activLine++;
                if(activLine >= Code.Count)
                    activLine = 0;
            } while (Code[activLine].ProgAdrress != PCL);
            PCL++;
            Code[activLine].IsHighlighted = true;
            Decode(Code[activLine].HexCode);
            if (Code[activLine].Breakpoint)
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
                    Commands.GOTO(code & 0b0000_0111_1111_1111, this);
                    return;
                case 0b0010_0000_0000_0000:
                    Commands.CALL(code & 0b0000_0111_1111_1111, this);
                    return;
            }
            //erste 4 Bit Maskieren
            opcode = code & 0b0011_1100_0000_0000;
            switch (opcode)
            {
                case 0b0001_0100_0000_0000:
                    Commands.BSF(code & 0b0000_0011_1111_1111, this);
                    return;
                case 0b0001_0000_0000_0000:
                    Commands.BCF(code & 0b0000_0011_1111_1111, this);
                    return;
                case 0b0001_1100_0000_0000:
                    Commands.BTFSS(code & 0b0000_0011_1111_1111, this);
                    return;
                case 0b0001_1000_0000_0000:
                    Commands.BTFSC(code & 0b0000_0011_1111_1111, this);
                    return;
            }
            //erste 6 Bit Maskieren
            opcode = code & 0b0011_1111_0000_0000;
            switch (opcode)
            {
                case 0b0011_0000_0000_0000:
                    Commands.MOVLW(code & 0b0000_0000_1111_1111, this);
                    return;
                case 0b0011_1001_0000_0000:
                    Commands.ANDLW(code & 0b0000_0000_1111_1111, this);
                    return;
                case 0b0011_1000_0000_0000:
                    Commands.IORLW(code & 0b0000_0000_1111_1111, this);
                    return;
                case 0b0011_1100_0000_0000:
                    Commands.SUBLW(code & 0b0000_0000_1111_1111, this);
                    return;
                case 0b0011_1010_0000_0000:
                    Commands.XORLW(code & 0b0000_0000_1111_1111, this);
                    break;
                case 0b0011_1110_0000_0000:
                    Commands.ADDLW(code & 0b0000_0000_1111_1111, this);
                    break;
                case 0b0011_0100_0000_0000:
                    Commands.RETLW(code & 0b0000_0000_1111_1111, this);
                    return;
                case 0b0000_0111_0000_0000:
                    Commands.ADDWF(code & 0b0000_0000_1111_1111, this);
                    return;
                case 0b0000_0101_0000_0000:
                    Commands.ANDWF(code & 0b0000_0000_1111_1111, this);
                    return;
                case 0b0000_1001_0000_0000:
                    Commands.COMF(code & 0b0000_0000_1111_1111, this);
                    return;
                case 0b0000_0011_0000_0000:
                    Commands.DECF(code & 0b0000_0000_1111_1111, this);
                    return;
                case 0b0000_1010_0000_0000:
                    Commands.INCF(code & 0b0000_0000_1111_1111, this);
                    return;
                case 0b0000_1000_0000_0000:
                    Commands.MOVF(code & 0b0000_0000_1111_1111, this);
                    return;
                case 0b0000_0100_0000_0000:
                    Commands.IORWF(code & 0b0000_00000_1111_1111, this);
                    return;
                case 0b0000_0010_0000_0000:
                    Commands.SUBWF(code & 0b0000_0000_1111_1111, this);
                    return;
                case 0b0000_1110_0000_0000:
                    Commands.SWAPF(code & 0b0000_0000_1111_1111, this);
                    return;
                case 0b0000_0110_0000_0000:
                    Commands.XORWF(code & 0b0000_0000_1111_1111, this);
                    return;
                case 0b0000_1101_0000_0000:
                    Commands.RLF(code & 0b0000_0000_1111_1111, this);
                    return;
                case 0b0000_1100_0000_0000:
                    Commands.RRF(code & 0b0000_0000_1111_1111, this);
                    return;
                case 0b0000_1011_0000_0000:
                    Commands.DECFSZ(code & 0b0000_0000_1111_1111, this);
                    return;
                case 0b0000_1111_0000_0000:
                    Commands.INCFSZ(code & 0b0000_0000_1111_1111, this);
                    return;
            }
            //erste 7 Bit Maskieren
            opcode = code & 0b0011_1111_1000_0000;
            switch (opcode)
            {
                case 0b0000_0000_1000_0000:
                    Commands.MOVWF(code & 0b0000_0000_0111_1111, this);
                    return;
                case 0b0000_0001_1000_0000:
                    Commands.CLRF(code & 0b0000_0000_0111_1111, this);
                    return;
                case 0b0000_0001_0000_0000:
                    Commands.CLRW(code & 0b0000_0000_0111_1111, this);
                    return;
            }
            //erste 14 Bit Maskieren
            opcode = code;
            switch (opcode)
            {
                case 0b0000_0000_0000_1000:
                    Commands.RETURN(this);
                    return;
                case 0b0000_0000_0000_0000:
                    Commands.NOP(this);
                    return;
            }
        }


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
