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
    }
}
