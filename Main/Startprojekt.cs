using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLIGUI;
using PicSimulator;

namespace Main
{
    internal class StartCLIGUI
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Process.Start("C:\\Users\\jonas\\Source\\Repos\\ASE_PIC\\PicSimulator\\bin\\Debug\\PicSimulator.exe");
            }
            else
            {
                if (args[0].ToLower() == "cli")
                    Program.StartCLIGUI();
            }
        }
    }
}
