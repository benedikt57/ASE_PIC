using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace PicSimulator
{
    public class PicViewModel : INotifyPropertyChanged
    {
        private Pic pic;
        public PicViewModel()
        {
            pic = new Pic();
            pic.PropertyChanged += Pic_PropertyChanged;
            //Commands
            LoadFileCommand = new RelayCommand(_ => LoadFileButton());
            StepCommand = new RelayCommand(_ => StepButton());
            StartCommand = new RelayCommand(_ => StartButton());
            InputCommand = new RelayCommand(parameter => InputButton(parameter));
            RamEditCommand = new RelayCommand(parameter => RamEdit(parameter));

            // Set default value to 4 MHz
            Is4MHzChecked = true;
        }
        private void Pic_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }
        public bool started = false;

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
                OnPropertyChanged(nameof(CarryBit));
                OnPropertyChanged(nameof(DCBit));
                OnPropertyChanged(nameof(ZeroBit));
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
        private int pcl;
        public int PCL
        {
            get { return pcl; }
            set
            {
                pcl = value;
                Ram[2] = pcl & 255;
                Ram[130] = Ram[2];
                OnPropertyChanged(nameof(Ram));
                OnPropertyChanged(nameof(PCL));
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
        public ObservableCollection<int> WertA
        {
            get
            {
                ObservableCollection<int> wert = new ObservableCollection<int>();
                for (int i = 0; i < 8; i++)
                {
                    wert.Add((Ram[0x5] & (1 << i)) == 0 ? 0 : 1);
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
            pic = new Pic();
            pic.LoadFile();
            Code = pic.Code;
            DateiPfad = pic.SourceFilePath;
            Ram = pic.Ram;
            WReg = pic.WReg;
            pic.ChangeString();
            TestString = pic.TestString;
        }
        public ICommand StepCommand { get; }
        public void StepButton()
        {
            if(!pic.Step() && started)
                started = false;
            Ram = pic.Ram;
            WReg = pic.WReg;
            Code = pic.Code;
            CodeTimer = pic.CodeTimer;
            CodeTimerFormat();
            OnPropertyChanged(nameof(Code));
        }
        public ICommand StartCommand { get; }
        public async void StartButton()
        {
            started = !started;
            await Task.Run(() =>
            { 
                while (started)
                {
                    StepButton();
                }
            });
        }
        public ICommand InputCommand { get; }
        public void InputButton(object parameter)
        {
            if (parameter is string str)
            {
                int index = int.Parse(str.Substring(3));
                Ram[0x5] ^= 1 << index;
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
                    Ram[index] = result;
                    Ram = Ram;
                }
            }
        }

        //PropertyChanged
        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(pic.Code):
                    Code = pic.Code;
                    break;
                case nameof(pic.TestString):
                    TestString = pic.TestString;
                    break;
                case nameof(pic.Ram):
                    Ram = pic.Ram;
                    break;
                case nameof(pic.WReg):
                    WReg = pic.WReg;
                    break;
                case nameof (pic.SourceFilePath):
                    DateiPfad = pic.SourceFilePath;
                    break;
                case nameof(pic.CodeTimer):
                    CodeTimer = pic.CodeTimer;
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

        private bool _is4MHzChecked;
        public bool Is4MHzChecked
        {
            get { return _is4MHzChecked; }
            set
            {
                if (value == true)
                {
                    Is8MHzChecked = false;
                    Is16MHzChecked = false;
                }
                _is4MHzChecked = value;
                if (value)
                {
                    AusgewaehlteQuarzfrequenz = "4 MHz";
                    AusgewaehlteQuarzfrequenzInt = 4;
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
                    Is4MHzChecked = false;
                    Is16MHzChecked = false;
                }
                _is8MHzChecked = value;
                if (value)
                {
                    AusgewaehlteQuarzfrequenz = "8 MHz";
                    AusgewaehlteQuarzfrequenzInt = 8;
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
                    Is4MHzChecked = false;
                    Is8MHzChecked = false;
                }
                _is16MHzChecked = value;
                if (value)
                {
                    AusgewaehlteQuarzfrequenz = "16 MHz";
                    AusgewaehlteQuarzfrequenzInt = 16;
                }
                OnPropertyChanged("Is16MHzChecked");
            }
        }


        private string ausgewaehlteQuarzfrequenz;
        public string AusgewaehlteQuarzfrequenz
        {
            get => ausgewaehlteQuarzfrequenz;
            set
            {
                SetProperty(ref ausgewaehlteQuarzfrequenz, value);
            }
        }
        private int ausgewaehlteQuarzfrequenzInt;
        public int AusgewaehlteQuarzfrequenzInt
        {
            get => ausgewaehlteQuarzfrequenzInt;
            set
            {
                SetProperty(ref ausgewaehlteQuarzfrequenzInt, value);
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
        private void CodeTimerFormat()
        {
            var tmp = ((4/(AusgewaehlteQuarzfrequenzInt * 1e6))*CodeTimer);
            if(tmp >= 1)
                CodeTimerString = tmp.ToString("0.###", CultureInfo.InvariantCulture).Replace(".", ",") + " s";
            else if(tmp >= 1e-1)
                CodeTimerString = (tmp * 1e3).ToString("0.#", CultureInfo.InvariantCulture).Replace(".", ",") + " ms";
            else if(tmp >= 1e-2)
                CodeTimerString = (tmp * 1e3).ToString("0.##", CultureInfo.InvariantCulture).Replace(".", ",") + " ms";
            else if(tmp >= 1e-3)
                CodeTimerString = (tmp * 1e3).ToString("0.###", CultureInfo.InvariantCulture).Replace(".", ",") + " ms";
            else
                CodeTimerString = (tmp * 1e6).ToString("0.###", CultureInfo.InvariantCulture).Replace(".", ",") + " µs";
        }

        private string dateiPfad;

        public string DateiPfad { get => dateiPfad; set => SetProperty(ref dateiPfad, value); }

    }
}