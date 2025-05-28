using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OMNOS.Pages 
{
    public class Product : INotifyPropertyChanged
    {
        private string _sku = string.Empty;
        public string SKU
        {
            get => _sku;
            set { if (_sku != value) { _sku = value ?? string.Empty; OnPropertyChanged(); } }
        }

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set { if (_name != value) { _name = value ?? string.Empty; OnPropertyChanged(); } }
        }

        private string _categoria = string.Empty;
        public string Categoria
        {
            get => _categoria;
            set { if (_categoria != value) { _categoria = value ?? string.Empty; OnPropertyChanged(); } }
        }

        private double _price;
        public double Price
        {
            get => _price;
            set { if (_price != value) { _price = value; OnPropertyChanged(); } }
        }

        private int _stock;
        public int Stock
        {
            get => _stock;
            set { if (_stock != value) { _stock = value; OnPropertyChanged(); } }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}