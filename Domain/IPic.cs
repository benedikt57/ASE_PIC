using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public interface IPic
    {
        int[] Ram { get; set; }
        int[] Stack { get; set; }
        int StackPointer { get; set; }
        int PC { get; set; }
        int PCLATCH { get; set; }
        int WReg { get; set; }
        int CodeTimer { get; set; }
        bool WDTActive { get; set; }
        int WDTTimer { get; set; }
        int WDTPrescaler { get; set; }
        double AusgewaehlteQuarzfrequenz { get; set; }
        bool IsSleeping { get; set; }
    }
}
