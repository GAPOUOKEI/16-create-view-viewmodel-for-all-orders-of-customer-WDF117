using Pizza.Models;
using Pizza.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizza.ViewModels
{
    class OrderViewModel : BindableBase
    {
        private Guid _id;
        public Guid Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }
        public readonly IOrderRepository _orderRepository;  
        public OrderViewModel(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            CreateOrderCommand = new RelayCommand(OnCreateOrder);
            CancelCommand = new RelayCommand(OnCancel);
        }

        

        private Guid _customerId;
        public Guid CustomerId
        {
            get => _customerId;
            set => SetProperty(ref _customerId, value);
        }
        private ObservableCollection<Order> _orders;
        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set => SetProperty(ref _orders, value);
        }
        private Order _newOrder;
        public Order NewOrder
        {
            get => _newOrder;
            set => SetProperty(ref _newOrder, value);
        }
        public RelayCommand CreateOrderCommand { get; }
        public RelayCommand CancelCommand { get; }
        public event Action Done;
        public RelayCommand LoadOrdersCommand { get; }
        private List<Order>? _orderList;
        public async Task LoadOrderAsync()
        {
            var orders = await _orderRepository.GetOrdersByCustomerAsync(CustomerId);
            Orders = new ObservableCollection<Order>(orders.OrderBy( o => o.OrderDate));
        }
        private void OnCancel()
        {
            Done?.Invoke();
        }
        private async void OnCreateOrder()
        {
            NewOrder.OrderDate = DateTime.Now;
            NewOrder.CustomerId = CustomerId;
            await _orderRepository.AddOrderAsync(NewOrder);
            Done?.Invoke();
        }
        public void InitializeNeworder()
        {
            NewOrder = new Order
            {
                CustomerId = CustomerId,
                OrderDate = DateTime.Now,
                OrderStatusId = 1
            };
        }
        public async void LoadData(Guid Id)
        {
            this.Id = Id;

            _orderList = await _orderRepository.GetOrdersByCustomerAsync(Id);
            Orders = new ObservableCollection<Order>(_orderList);
        }
    }
}
