using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

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
                ram = value;
                OnPropertyChanged(nameof(Ram));
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



    }
}