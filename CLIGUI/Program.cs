using PicSimulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLIGUI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PicViewModel picViewModel = new PicViewModel();

            picViewModel.LoadFileButton();

            while (true)
            {
                Console.WriteLine("S: Step\nE: Edit Ram StatusRegister\nX: Close");
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
                else if (input.ToLower() == "e")
                {
                    picViewModel.RamEdit("Ram4");
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
