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
            //Load("C:\\Users\\jonas\\Downloads\\TestProg_PicSim_20230413\\TPicSim1.LST");
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
                                line.HexCode = Convert.ToInt32(match.Groups[2].Value + match.Groups[3].Value, 16);
                            }else
                            {
                                line.HexCode = 0;
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
