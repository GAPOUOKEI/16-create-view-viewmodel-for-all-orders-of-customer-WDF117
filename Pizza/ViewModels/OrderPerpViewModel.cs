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
    class OrderPerpViewModel : BindableBase
    {
        private Guid _id;
        public Guid Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private IOrderRepository _orderRepository;


        public OrderPerpViewModel(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;

            AddOrderItemCommand = new(OnAddOrderItem);
            RemoveOrderItemCommand = new(OnRemoveOrderItem);
            SaveCommand = new(OnSave, CanSave);
            CancelCommand = new(OnCancel);
            SelectedOrderItems = new ObservableCollection<OrderItem>();

            LoadDataAsync();
        }

        private List<OrderItem> _items;
        public async void LoadDataAsync()
        {
            _items = await _orderRepository.GetOrderItems();
            AvailableOrderItems = new ObservableCollection<OrderItem>(_items);
        }
        #region Поля viewModel
        private string _phoneNumber;

        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        private string _city;

        public string City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }

        private string _street;

        public string Street
        {
            get => _street;
            set => SetProperty(ref _street, value);
        }

        private OrderItem _selectedAvailableItem;
        public OrderItem SelectedAvailableItem
        {
            get => _selectedAvailableItem;
            set => SetProperty(ref _selectedAvailableItem, value);
        }

        private OrderItem _selectedSelectedItem;
        public OrderItem SelectedSelectedItem
        {
            get => _selectedSelectedItem;
            set => SetProperty(ref _selectedSelectedItem, value);
        }

        ObservableCollection<OrderItem>? _availableOrderItems;

        public ObservableCollection<OrderItem>? AvailableOrderItems
        {
            get => _availableOrderItems;
            set => SetProperty(ref _availableOrderItems, value);
        }

        private ObservableCollection<OrderItem> _selectedOrderItems;

        public ObservableCollection<OrderItem> SelectedOrderItems
        {
            get => _selectedOrderItems;
            set => SetProperty(ref _selectedOrderItems, value);
        }
        #endregion
        public RelayCommand<OrderItem> AddOrderItemCommand { get; }
        public RelayCommand<OrderItem> RemoveOrderItemCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        public event Action Done;

        private void OnAddOrderItem(OrderItem item)
        {
            if (!SelectedOrderItems.Contains(item))
            {
                SelectedOrderItems.Add(item);
            }

            SaveCommand.OnCanExecuteChanged();
        }

        private bool CanAddOrderItem(OrderItem item) =>
            !SelectedOrderItems.Contains(item);

        private void OnRemoveOrderItem(OrderItem item)
        {
            if (SelectedOrderItems.Contains(item))
            {
                SelectedOrderItems.Remove(item);
            }

            SaveCommand.OnCanExecuteChanged();
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(PhoneNumber) &&
                  !string.IsNullOrWhiteSpace(City) &&
                  !string.IsNullOrWhiteSpace(Street) &&
                  SelectedOrderItems.Any();
        }

        private async void OnSave()
        {
            foreach (var orderItem in SelectedOrderItems)
            {
                _orderRepository.AttachProduct(orderItem.Product);
            }
            var order = new Order
            {
                CustomerId = Id,
                Phone = PhoneNumber,
                DeliveryCity = City,
                DeliveryStreet = Street,
                OrderDate = DateTime.UtcNow,
                OrderItems = SelectedOrderItems,
                OrderStatusId = 1
            };
            await _orderRepository.AddOrderAsync(order);
            Done?.Invoke();
        }

        private void OnCancel()
        {
            Done?.Invoke();
        }
    }
}
