using PicSimulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLIGUI
{
    public class Program
    {
        public static void StartCLIGUI()
        {
            PicViewModel picViewModel = new PicViewModel();

            picViewModel.LoadFileButton();

            while (true)
            {
                Console.WriteLine("S: Step\nX: Close");
                string input = Console.ReadLine();
                if (input.ToLower() == "x")
                {
                    break;
                }
                else if (input.ToLower() == "s")
                {
                    picViewModel.StepCommand.Execute(null);
                    Console.WriteLine(picViewModel.CodeTimerString);
                    Console.WriteLine("Wreg: " + picViewModel.WReg);
                    Console.WriteLine("Carry: " + picViewModel.CarryBit);
                    Console.WriteLine("Digit Carry: " + picViewModel.DCBit);
                    Console.WriteLine("Zero: " + picViewModel.ZeroBit);
                }
            }
        }
    }
}
