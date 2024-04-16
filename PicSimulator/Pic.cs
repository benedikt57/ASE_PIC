using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private List<int> hexCode = new List<int>();
        public List<int> HexCode
        {
            get { return hexCode; }
            set
            {
                hexCode = value;
                OnPropertyChanged(nameof(hexCode));
            }
        }
        private List<string> code = new List<string>();
        public List<string> Code
        {
            get { return code; }
            set
            {
                code = value;
                OnPropertyChanged(nameof(Code));
            }
        }



        public Pic()
        {
            LoadFile();
        }
        private void LoadFile()
        {
            try
            {
                using (var sr = new StreamReader("C:\\Users\\jonas\\Downloads\\TestProg_PicSim_20230413\\TPicSim1.LST", Encoding.Default))
                {
                    while (!sr.EndOfStream)
                    {
                        Match match = Regex.Match(sr.ReadLine(), @"^ *(([0-9A-F]{4}) ([0-9A-F]{4}) *)?([0-9]{5}) *(.*)$");
                        if(match.Success)
                        {

                            Code.Add(match.Groups[4].Value + "    " + match.Groups[5].Value);
                            if (match.Groups[1].Success)
                            {
                                HexCode.Add(Convert.ToInt32(match.Groups[2].Value + match.Groups[3].Value, 16));
                            }
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
    }
}
