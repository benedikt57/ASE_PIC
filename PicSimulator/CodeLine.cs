using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSimulator
{
    public class CodeLine : INotifyPropertyChanged
    {
        private bool breakpoint;
        private bool isHighlighted;
        private int progAdrress;
        private int hexCode;
        private string code;
        private int CodeTimer;

        public bool Breakpoint
        {
            get { return breakpoint; }
            set
            {
                if (breakpoint != value)
                {
                    breakpoint = value;
                    OnPropertyChanged(nameof(Breakpoint));
                }
            }
        }

        public bool IsHighlighted
        {
            get { return isHighlighted; }
            set
            {
                if (isHighlighted != value)
                {
                    isHighlighted = value;
                    OnPropertyChanged(nameof(IsHighlighted));
                }
            }
        }

        public int ProgAdrress
        {
            get { return progAdrress; }
            set
            {
                if (progAdrress != value)
                {
                    progAdrress = value;
                    OnPropertyChanged(nameof(ProgAdrress));
                }
            }
        }

        public int HexCode
        {
            get { return hexCode; }
            set
            {
                if (hexCode != value)
                {
                    hexCode = value;
                    OnPropertyChanged(nameof(HexCode));
                }
            }
        }

        public string Code
        {
            get { return code; }
            set
            {
                if (code != value)
                {
                    code = value;
                    OnPropertyChanged(nameof(Code));
                }
            }
        }

        public int GetCodeTimer()
        { return GetCodeTimer(); }
        public void SetCodeTimer(int value)
        {
            if (GetCodeTimer() != value)
            {
                SetCodeTimer(value);
                OnPropertyChanged(nameof(CodeTimer));
            }
        }

        public CodeLine()
        {
            Breakpoint = false;
            IsHighlighted = false;
            HexCode = 0;
            Code = string.Empty;
        }

        public CodeLine(int hexCode, string code)
        {
            Breakpoint = false;
            IsHighlighted = false;
            HexCode = hexCode;
            Code = code;
        }

        public CodeLine(bool breakpoint, int hexCode, string code)
        {
            Breakpoint = breakpoint;
            IsHighlighted = false;
            HexCode = hexCode;
            Code = code;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
