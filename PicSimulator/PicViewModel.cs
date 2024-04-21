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
        }
        private void Pic_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        //Variablen
        private ObservableCollection<CodeLine> code1;
        public ObservableCollection<CodeLine> Code
        {
            get { return code1; }
            set
            {
                code1 = value;
                OnPropertyChanged(nameof(Code));
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

        private bool is4MHzChecked;
        public bool Is4MHzChecked
        {
            get => is4MHzChecked;
            set
            {
                if (SetProperty(ref is4MHzChecked, value) && value)
                {
                    AusgewaehlteQuarzfrequenz = "4 MHz";
                }
            }
        }

        private bool is8MHzChecked;
        public bool Is8MHzChecked
        {
            get => is8MHzChecked;
            set
            {
                if (SetProperty(ref is8MHzChecked, value) && value)
                {
                    AusgewaehlteQuarzfrequenz = "8 MHz";
                }
            }
        }

        private bool is16MHzChecked;
        public bool Is16MHzChecked
        {
            get => is16MHzChecked;
            set
            {
                if (SetProperty(ref is16MHzChecked, value) && value)
                {
                    AusgewaehlteQuarzfrequenz = "16 MHz";
                }
            }
        }


        private string ausgewaehlteQuarzfrequenz;
        public string AusgewaehlteQuarzfrequenz { get => ausgewaehlteQuarzfrequenz; set => SetProperty(ref ausgewaehlteQuarzfrequenz, value); }



    }
}