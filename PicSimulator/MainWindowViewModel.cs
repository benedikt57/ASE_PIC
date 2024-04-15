using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PicSimulator
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand ButtonClick { get; private set; }

        public MainWindowViewModel()
        {
            ButtonClick = new RelayCommand(Ausfuehren, KannAusfuehren);
        }
        private void Ausfuehren(object obj)
        {
            TestString = "Hallo";
        }
        private bool KannAusfuehren(object obj)
        {
            return true;
        }


        private string testString = "BLala";
        public string TestString
        {
            get { return testString; }
            set
            {
                testString = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(TestString)));
            }
        }
        
    }
}
