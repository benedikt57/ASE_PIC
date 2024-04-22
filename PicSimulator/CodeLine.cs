using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSimulator
{
    public class CodeLine
    {
        public bool Breakpoint { get; set; }
        public int ProgAdrress { get; set; }
        public int HexCode { get; set; }
        public string Code { get; set; }
        public CodeLine()
        {
            Breakpoint = false;
            HexCode = 0;
            Code = string.Empty;
        }
        public CodeLine(int hexCode, string code)
        {
            Breakpoint = false;
            HexCode = hexCode;
            Code = code;
        }
        public CodeLine(bool breakpoint, int hexCode, string code)
        {
            Breakpoint = breakpoint;
            HexCode = hexCode;
            Code = code;
        }
    }
}
