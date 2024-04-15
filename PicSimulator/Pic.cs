using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PicSimulator
{
    public class Pic
    {
        public Pic()
        {
            LoadFile();
        }
        public List<string> Code { get; set; } = new List<string>();
        private async void LoadFile()
        {
            try
            {
                using (var sr = new StreamReader("C:\\Users\\jonas\\Downloads\\TestProg_PicSim_20230413\\TPicSim1.LST", Encoding.Default))
                {
                    while (!sr.EndOfStream)
                    {
                        Code.Add(await sr.ReadLineAsync());
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                Code = Code.DefaultIfEmpty("File not found").ToList();
            }
        }
    }
}
