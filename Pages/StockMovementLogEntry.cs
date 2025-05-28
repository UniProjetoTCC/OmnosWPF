using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OMNOS.Pages // Ou OMNOS.Models
{
    public enum StockMovementType
    {
        Entrada, // Addition
        Saida    // Reduction
    }

    public class StockMovementLogEntry : INotifyPropertyChanged
    {
        private string _id;
        public string Id { get => _id; set { if (_id != value) { _id = value; OnPropertyChanged(); } } }

        private DateTime _movementDate;
        public DateTime MovementDate { get => _movementDate; set { if (_movementDate != value) { _movementDate = value; OnPropertyChanged(); } } }

        private string _productSku;
        public string ProductSku { get => _productSku; set { if (_productSku != value) { _productSku = value; OnPropertyChanged(); } } }

        private string _productName;
        public string ProductName { get => _productName; set { if (_productName != value) { _productName = value; OnPropertyChanged(); } } }

        private int _quantity; // Quantidade absoluta movimentada
        public int Quantity { get => _quantity; set { if (_quantity != value) { _quantity = value; OnPropertyChanged(); } } }

        private StockMovementType _movementType;
        public StockMovementType MovementType { get => _movementType; set { if (_movementType != value) { _movementType = value; OnPropertyChanged(); OnPropertyChanged(nameof(MovementTypeDisplay)); } } }

        public string MovementTypeDisplay => MovementType == StockMovementType.Entrada ? "Entrada" : "Saída";

        private string _reason;
        public string Reason { get => _reason; set { if (_reason != value) { _reason = value; OnPropertyChanged(); } } }

        // Opcional: para rastrear quem fez a movimentação
        // private string _userId;
        // public string UserId { get => _userId; set { if (_userId != value) { _userId = value; OnPropertyChanged(); } } }


        public StockMovementLogEntry()
        {
            Id = Guid.NewGuid().ToString(); // Gera um ID único para a entrada do log
            MovementDate = DateTime.UtcNow; // Data/Hora atual em UTC
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}