using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSimulator
{
    public class Pic
    {
        public Pic()
        {
        }
        public string TestString { get; set; } = "Test";
        public void Click()
        {
            TestString = "Hallo";
        }
        public void Click2()
        {
            TestString = "Tschau";
        }
        public void Click3()
        {
            TestString = "Passt";
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
