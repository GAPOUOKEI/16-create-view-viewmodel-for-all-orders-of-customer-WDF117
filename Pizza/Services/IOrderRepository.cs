using Pizza.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizza.Services
{
    internal interface IOrderRepository
    {
        //получить список заказов конкретного клиента
        Task<List<Order>> GetOrdersByCustomerAsync(Guid customerId);
        //получить список всех заказов в системе
        Task<List<Order>> GetAllOrdersAsync();
        //добавить заказ
        Task<Order> AddOrderAsync(Order order);
        //обновить заказ
        Task<Order> UpdateOrderAsync(Order order);
        //удалить заказ
        Task DeleteOrderAsync(long orderId);

        //получить список всех продуктов
        Task<List<Product>> GetAllProductsAsync();
        //получить список ингридиентов
        Task<List<ProductOption>> GetAllProductOptionsAsync();
        //получить список размеров и объемов 
        Task<List<ProductSize>> GetAllProductSizesAsync();
        //получить список статусов заказов
        Task<List<OrderStatus>> GetAllOrderStatusesAsync();
        Task<List<OrderItem>> GetOrderItems();
        void AttachProduct(Product product);
    }
}
