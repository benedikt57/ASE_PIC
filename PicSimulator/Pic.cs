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
        private int aktivLine = 0;

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

        


        public Pic()
        {
        }
        public void LoadFile()
        {
            Code.Clear();
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
                aktivLine++;
            } while (Code[aktivLine].ProgAdrress == -1);
            Decode(Code[aktivLine].HexCode);
        }
        private void Decode(int code)
        {
            //erste 6 Bit Maskieren
            int opcode = code & 0b0011_1111_0000_0000;
            switch (opcode)
            {
                case 0b0011_0000_0000_0000:
                    MessageBox.Show("MOVLW");
                    Commands.MOVLW(code & 0b0000_0000_1111_1111, this);
                    break;
                case 0b0011_1001_0000_0000:
                    MessageBox.Show("ANDLW");
                    Commands.ANDLW(code & 0b0000_0000_1111_1111, this);
                    break;
                case 0b0011_1000_0000_0000:
                    MessageBox.Show("IORLW");
                    Commands.IORLW(code & 0b0000_0000_1111_1111, this);
                    break;
                case 0b0011_1100_0000_0000:
                    MessageBox.Show("SUBLW");
                    Commands.SUBLW(code & 0b0000_0000_1111_1111, this);
                    break;
                case 0b0011_1010_0000_0000:
                    MessageBox.Show("XORLW");
                    Commands.XORLW(code & 0b0000_0000_1111_1111, this);
                    break;
                case 0b0011_1110_0000_0000:
                    MessageBox.Show("ADDLW");
                    Commands.ADDLW(code & 0b0000_0000_1111_1111, this);
                    break;
                default:
                    break;
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
