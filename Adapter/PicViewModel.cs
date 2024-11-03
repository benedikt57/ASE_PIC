using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PicSimulator
{
    public class PicViewModel : INotifyPropertyChanged, IPicViewModel
    {
        private Pic pic;
        public PicViewModel()
        {
            Code = new ObservableCollection<CodeLine>();
            pic = new Pic(this);
            pic.PropertyChanged += Pic_PropertyChanged;
            //Commands
            LoadFileCommand = new RelayCommand(_ => LoadFileButton());
            StepCommand = new RelayCommand(_ => StepButton());
            StartCommand = new RelayCommand(_ => StartButton());
            ResetCommand = new RelayCommand(_ => ResetButton());
            InputCommand = new RelayCommand(parameter => InputButton(parameter));
            RamEditCommand = new RelayCommand(parameter => RamEdit(parameter));
            SFRCommand = new RelayCommand(parameter => SFRButton(parameter));

            // Set default value to 4 MHz
            Is4MHzChecked = true;
            IsPortBChecked = true;
            AusgewaehlteQuarzfrequenz = 4;

            // Start View aktualisieren
        }
        private void Pic_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }
        private bool started = false;
        public bool Started
        {
            get { return started; }
            set
            {
                started = value;
                OnPropertyChanged(nameof(Started));
            }
        }
        private bool isSleeping = false;
        public bool IsSleeping
        {
            get { return isSleeping; }
            set
            {
                isSleeping = value;
                OnPropertyChanged(nameof(IsSleeping));
            }
        }

        //Variablen
        private ObservableCollection<CodeLine> code;
        public ObservableCollection<CodeLine> Code
        {
            get { return code; }
            set
            {
                code = value;
                OnPropertyChanged(nameof(Code));
            }
        }
        private int[] ram = new int[256];
        public int[] Ram
        {
            get { return ram; }
            set
            {
                ram = value;
                OnPropertyChanged(nameof(Ram));
                OnPropertyChanged(nameof(InOutA));
                OnPropertyChanged(nameof(WertA));
                OnPropertyChanged(nameof(InOutB));
                OnPropertyChanged(nameof(WertB));
                OnPropertyChanged(nameof(CarryBit));
                OnPropertyChanged(nameof(DCBit));
                OnPropertyChanged(nameof(ZeroBit));
                OnPropertyChanged(nameof(LED));
            }
        }
        private int[] stack = new int[8];
        public int[] Stack
        {
            get { return stack; }
            set
            {
                stack = value;
                OnPropertyChanged(nameof(Stack));
            }
        }
        private int stackPointer;
        public int StackPointer
        {
            get { return stackPointer; }
            set
            {
                stackPointer = value;
                OnPropertyChanged(nameof(StackPointer));
            }
        }
        private int pc;
        public int PC
        {
            get { return pc; }
            set
            {
                pc = value;
                Ram[2] = pc & 255;
                Ram[130] = Ram[2];
                OnPropertyChanged(nameof(Ram));
                OnPropertyChanged(nameof(PC));
            }
        }
        private int pcLatch;
        public int PCLATCH
        {
            get { return pcLatch; }
            set
            {
                pcLatch = value;
                OnPropertyChanged(nameof(PCLATCH));
            }
        }

        private bool _isPortAChecked;
        public bool IsPortAChecked
        {
            get { return _isPortAChecked; }
            set
            {
                if (value == true)
                {
                    IsPortBChecked = false;
                }
                _isPortAChecked = value;
                if (value)
                {
                    PortAuswahl = 5;
                }
                OnPropertyChanged("IsPortAChecked");
            }
        }

        private bool _isPortBChecked;
        public bool IsPortBChecked
        {
            get { return _isPortBChecked; }
            set
            {
                if (value == true)
                {
                    IsPortAChecked = false;
                }
                _isPortBChecked = value;
                if (value)
                {
                    PortAuswahl = 6;
                }
                OnPropertyChanged("IsPortBChecked");
            }
        }

        private int portAuswahl = 6;
        public int PortAuswahl
        {
            get => portAuswahl;
            set
            {
                portAuswahl = value;
                OnPropertyChanged(nameof(PortAuswahl));
            }
        }

        public ObservableCollection<int> LED
        {
            get
            {
                if(PortAuswahl == 5)
                {
                    return WertA;
                }
                else
                {
                    return WertB;
                }
            }
        }


        public ObservableCollection<string> InOutA
        {
            get
            {
                ObservableCollection<string> inOut = new ObservableCollection<string>();
                for (int i = 0; i < 8; i++)
                {
                    inOut.Add((Ram[0x85] & (1 << i)) == 0 ? "Out" : "In");
                }
                return inOut;
            }
        }
        private ObservableCollection<int> wertAInput = new ObservableCollection<int>() {0, 0, 0, 0, 0, 0, 0, 0};
        public ObservableCollection<int> WertA
        {
            get
            {
                ObservableCollection<int> wert = new ObservableCollection<int>();
                for (int i = 0; i < 8; i++)
                {
                    if ((Ram[0x85] & (1 << i)) == 0)
                    {
                        wert.Add((Ram[0x5] & (1 << i)) == 0 ? 0 : 1);
                        wertAInput[i] = wert[i];
                    }
                    else
                    {
                        wert.Add(wertAInput[i]);
                    }
                }
                return wert;
            }
        }
        public ObservableCollection<string> InOutB
        {
            get
            {
                ObservableCollection<string> inOut = new ObservableCollection<string>();
                for (int i = 0; i < 8; i++)
                {
                    inOut.Add((Ram[0x86] & (1 << i)) == 0 ? "Out" : "In");
                }
                return inOut;
            }
        }
        private ObservableCollection<int> wertBInput = new ObservableCollection<int>() { 0, 0, 0, 0, 0, 0, 0, 0 };
        public ObservableCollection<int> WertB
        {
            get
            {
                ObservableCollection<int> wert = new ObservableCollection<int>();
                for (int i = 0; i < 8; i++)
                {
                    if ((Ram[0x86] & (1 << i)) == 0)
                    {
                        wert.Add((Ram[0x6] & (1 << i)) == 0 ? 0 : 1);
                        wertBInput[i] = wert[i];
                    }
                    else
                    {
                        wert.Add(wertBInput[i]);
                    }
                }
                return wert;
            }
        }
        public int CarryBit
        {
            get { return ram[3] & 1; }
        }
        public int DCBit
        {
            get { return (ram[3] & 2) >> 1; }
        }
        public int ZeroBit
        {
            get { return (ram[3] & 4) >> 2; }
        }
        private int wReg;
        public int WReg
        {
            get { return wReg; }
            set
            {
                wReg = value;
                OnPropertyChanged(nameof(WReg));
            }
        }
        private bool wdtActive;
        public bool WDTActive
        {
            get { return wdtActive; }
            set
            {
                wdtActive = value;
                OnPropertyChanged(nameof(WDTActive));
            }
        }
        private int wdtTimer;
        public int WDTTimer
        {
            get { return wdtTimer; }
            set
            {
                wdtTimer = value;
                OnPropertyChanged(nameof(WDTTimer));
            }
        }
        private int wdtPrescaler;
        public int WDTPrescaler
        {
            get { return wdtPrescaler; }
            set
            {
                wdtPrescaler = value;
                OnPropertyChanged(nameof(WDTPrescaler));
            }
        }


        private string testString;
        public string TestString
        {
            get { return testString; }
            set
            {
                testString = value;
                OnPropertyChanged(nameof(TestString));
            }
        }
        //Commands
        public ICommand LoadFileCommand { get; }
        public void LoadFileButton()
        {
            started = false;
            pic = new Pic(this);
            WDTActive = false;
            pic.LoadFile();
            //Code = pic.Code;
            DateiPfad = pic.SourceFilePath;
            CodeTimerString = CodeTimerFormat(CodeTimer);
            WDTTimerString = CodeTimerFormat(WDTTimer);
        }
        public ICommand StepCommand { get; }
        public void StepButton()
        {
            if(!pic.Step() && Started)
                Started = false;
            CodeTimerString = CodeTimerFormat(CodeTimer);
            WDTTimerString = CodeTimerFormat(WDTTimer);
            OnPropertyChanged(nameof(Code));
            Ram = Ram;
        }
        public ICommand StartCommand { get; }
        public async void StartButton()
        {
            Started = !Started;
            OnPropertyChanged(nameof(Started));
            await Task.Run(() =>
            { 
                while (Started)
                {
                    StepButton();
                }
            });
        }
        public ICommand ResetCommand { get; }
        public void ResetButton()
        {
            pic.commands.MCLR();
            Ram = Ram;
        }
        public ICommand InputCommand { get; }
        public void InputButton(object parameter)
        {
            if (parameter is string str)
            {
                var port = str.Substring(3, 1);
                int index = int.Parse(str.Substring(4));
                if (port == "A")
                {
                    int value = (WertA[index] == 0) ? 1 : 0;
                    wertAInput[index] = value;
                    pic.commands.writeBit(value, index, 0x05);
                }
                else if (port == "B")
                {
                    int value = (WertB[index] == 0) ? 1 : 0;
                    wertBInput[index] = value;
                    pic.commands.writeBit(value, index, 0x06);
                }
                Ram = Ram;
            }
        }
        public ICommand RamEditCommand { get; }
        public void RamEdit(object parameter)
        {
            if (parameter is string str)
            {
                int index = int.Parse(str.Substring(3));
                string input = Interaction.InputBox("Wert für Register " + index, "Input", Ram[index].ToString("X"));
                if (int.TryParse(input, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int result))
                {
                    if (result > 255)
                        result = 255;
                    if (result < 0)
                        result = 0;
                    pic.commands.writeByte(result, index, false);
                    Ram = Ram;
                }
            }
        }
        public ICommand SFRCommand { get; }
        public void SFRButton(object parameter)
        {
            if (parameter is string str)
            {
                int index = int.Parse(str.Substring(1));
                string register = str.Substring(0, 1);
                switch (register)
                {
                    case "S":
                        pic.commands.writeBit((Ram[0x03] & (1 << index)) == 0 ? 1: 0, index, 0x03);
                        return;
                    case "O":
                        pic.commands.writeBit((Ram[0x81] & (1 << index)) == 0 ? 1 : 0, index, 0x81);
                        return;
                    case "I":
                        pic.commands.writeBit((Ram[0x0B] & (1 << index)) == 0 ? 1 : 0, index, 0x0B);
                        return;
                }
                Ram = Ram;
            }
        }

        //PropertyChanged
        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof (pic.SourceFilePath):
                    DateiPfad = pic.SourceFilePath;
                    break;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }

            return false;
        }

        private bool _is032MHzChecked;
        public bool Is032MHzChecked
        {
            get { return _is032MHzChecked; }
            set
            {
                if (value == true)
                {
                    Is1MHzChecked = false;
                    Is4MHzChecked = false;
                    Is8MHzChecked = false;
                    Is16MHzChecked = false;
                }
                _is032MHzChecked = value;
                if (value)
                {
                    AusgewaehlteQuarzfrequenzString = "32 KHz";
                    AusgewaehlteQuarzfrequenz = 0.032;
                }
                OnPropertyChanged("Is032MHzChecked");
            }
        }

        private bool _is1MHzChecked;
        public bool Is1MHzChecked
        {
            get { return _is1MHzChecked; }
            set
            {
                if (value == true)
                {
                    Is032MHzChecked = false;
                    Is4MHzChecked = false;
                    Is8MHzChecked = false;
                    Is16MHzChecked = false;
                }
                _is1MHzChecked = value;
                if (value)
                {
                    AusgewaehlteQuarzfrequenzString = "1 MHz";
                    AusgewaehlteQuarzfrequenz = 1;
                }
                OnPropertyChanged("Is1MHzChecked");
            }
        }
        private bool _is4MHzChecked;
        public bool Is4MHzChecked
        {
            get { return _is4MHzChecked; }
            set
            {
                if (value == true)
                {
                    Is032MHzChecked = false;
                    Is1MHzChecked = false;
                    Is8MHzChecked = false;
                    Is16MHzChecked = false;
                }
                _is4MHzChecked = value;
                if (value)
                {
                    AusgewaehlteQuarzfrequenzString = "4 MHz";
                    AusgewaehlteQuarzfrequenz = 4;
                }
                OnPropertyChanged("Is4MHzChecked");
            }
        }

        private bool _is8MHzChecked;
        public bool Is8MHzChecked
        {
            get { return _is8MHzChecked; }
            set
            {
                if (value == true)
                {
                    Is032MHzChecked = false;
                    Is1MHzChecked = false;
                    Is4MHzChecked = false;
                    Is16MHzChecked = false;
                }
                _is8MHzChecked = value;
                if (value)
                {
                    AusgewaehlteQuarzfrequenzString = "8 MHz";
                    AusgewaehlteQuarzfrequenz = 8;
                }
                OnPropertyChanged("Is8MHzChecked");
            }
        }

        private bool _is16MHzChecked;
        public bool Is16MHzChecked
        {
            get { return _is16MHzChecked; }
            set
            {
                if (value == true)
                {
                    Is032MHzChecked = false;
                    Is1MHzChecked = false;
                    Is4MHzChecked = false;
                    Is8MHzChecked = false;
                }
                _is16MHzChecked = value;
                if (value)
                {
                    AusgewaehlteQuarzfrequenzString = "16 MHz";
                    AusgewaehlteQuarzfrequenz = 16;
                }
                OnPropertyChanged("Is16MHzChecked");
            }
        }


        private string ausgewaehlteQuarzfrequenzString;
        public string AusgewaehlteQuarzfrequenzString
        {
            get => ausgewaehlteQuarzfrequenzString;
            set
            {
                SetProperty(ref ausgewaehlteQuarzfrequenzString, value);
            }
        }
        private double ausgewaehlteQuarzfrequenz;
        public double AusgewaehlteQuarzfrequenz
        {
            get => ausgewaehlteQuarzfrequenz;
            set
            {
                SetProperty(ref ausgewaehlteQuarzfrequenz, value);
            }
        }

        private int codeTimer;

        public int CodeTimer
        {
            get { return codeTimer; }
            set
            {
                codeTimer = value;
                OnPropertyChanged(nameof(CodeTimer));
            }
        }
        private string codeTimerString;
        public string CodeTimerString
        {
            get { return codeTimerString; }
            set
            {
                codeTimerString = value;
                OnPropertyChanged(nameof(CodeTimerString));
            
            }
        }
        private string wdtTimerString = "0";
        public string WDTTimerString
        {
            get { return wdtTimerString; }
            set
            {
                wdtTimerString = value;
                OnPropertyChanged(nameof(WDTTimerString));
            }
        }
        private string CodeTimerFormat(int timer)
        {
            var tmp = ((4/(AusgewaehlteQuarzfrequenz * 1e6))*timer);
            if(tmp >= 1)
                return tmp.ToString("0.###", CultureInfo.InvariantCulture).Replace(".", ",") + " s";
            else if(tmp >= 1e-1)
                return (tmp * 1e3).ToString("0.#", CultureInfo.InvariantCulture).Replace(".", ",") + " ms";
            else if(tmp >= 1e-2)
                return (tmp * 1e3).ToString("0.##", CultureInfo.InvariantCulture).Replace(".", ",") + " ms";
            else if(tmp >= 1e-3)
                return (tmp * 1e3).ToString("0.###", CultureInfo.InvariantCulture).Replace(".", ",") + " ms";
            else
                return (tmp * 1e6).ToString("0.###", CultureInfo.InvariantCulture).Replace(".", ",") + " µs";
        }

        private string dateiPfad;

        public string DateiPfad { get => dateiPfad; set => SetProperty(ref dateiPfad, value); }
        
    }
}