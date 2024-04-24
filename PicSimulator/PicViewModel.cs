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

            // Set default value to 4 MHz
            Is4MHzChecked = true;
        }
        private void Pic_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
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
                if (ram[3] != value[3])
                {
                    value[131] = value[3];
                }
                if (ram[131] != value[131])
                {
                    value[3] = value[131];
                }
                ram = value;
                OnPropertyChanged(nameof(Ram));
                OnPropertyChanged(nameof(CarryBit));
                OnPropertyChanged(nameof(DCBit));
                OnPropertyChanged(nameof(ZeroBit));
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
            pic.LoadFile();
            Code = pic.Code;
            DateiPfad = pic.SourceFilePath;
            pic.ChangeString();
            TestString = pic.TestString;
        }
        public ICommand StepCommand { get; }
        public void StepButton()
        {
            pic.Step();
            Ram = pic.Ram;
            WReg = pic.WReg;
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
                if (value) AusgewaehlteQuarzfrequenz = "4 MHz";
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
                if (value) AusgewaehlteQuarzfrequenz = "8 MHz";
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
                if (value) AusgewaehlteQuarzfrequenz = "16 MHz";
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

        private string dateiPfad;

        public string DateiPfad { get => dateiPfad; set => SetProperty(ref dateiPfad, value); }

        public class ActiveLineConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if ((int)value == (int)parameter)
                {
                    return Brushes.LightBlue; // oder eine andere Farbe Ihrer Wahl
                }
                else
                {
                    return Brushes.Transparent; // oder die Standardfarbe Ihrer Wahl
                }
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

    }
}