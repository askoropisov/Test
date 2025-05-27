using System.ComponentModel;

namespace Visual_Matrix.Models
{
    public class Cell : INotifyPropertyChanged
    {
        private int _color;
        public Cell() { }
        public int Cost { get; set; }      // Стоимость клетки


        public int Color 
        {
            get => _color;
            set
            {
                _color = value;
                OnPropertyChanged(nameof(Color));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
