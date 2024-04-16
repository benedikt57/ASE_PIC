using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PicSimulator
{
    public class PicViewModel : INotifyPropertyChanged
    {
        private Pic pic;
        public PicViewModel()
        {
            pic = new Pic();
            pic.PropertyChanged += Pic_PropertyChangd;
            //Commands
            //ButtonClick = new RelayCommand(_ => Click());
        }
        private void Pic_PropertyChangd(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        //Variablen
        public List<int> HexCode
        {
            get { return pic.HexCode; }
            set
            {
                pic.HexCode = value;
                OnPropertyChanged(nameof(HexCode));
            }
        }
        public List<string> Code
        {
            get { return pic.Code; }
            set
            {
                pic.Code = value;
                OnPropertyChanged(nameof(Code));
            }
        }
        //Commands
        //public ICommand ButtonClick { get; }
        //public void Click()
        //{
        //    pic.Click();
        //    OnPropertyChanged(nameof(TestString));
        //}

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        
    }
}
