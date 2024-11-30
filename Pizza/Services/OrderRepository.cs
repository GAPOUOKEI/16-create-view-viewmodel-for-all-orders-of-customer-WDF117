using Microsoft.EntityFrameworkCore;
using Pizza.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Pizza.Services
{
    internal class OrderRepository :IOrderRepository
    {
        private readonly S117Context _context 
            = new S117Context();
        public void AttachProduct(Product product)
        {
            if (_context.Entry(product).State == EntityState.Detached)
            {
                _context.Products.Attach(product);
            }
        }
        public async Task<Order> AddOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task DeleteOrderAsync(long orderId)
        {
            using (TransactionScope scope = new TransactionScope()) 
            { 
                var order = _context.Orders.Include("OrderItems")
                    .Include("OrderItems.OrderItemsOptions")
                    .FirstOrDefault(o => o.Id == orderId); 

                if (order != null) 
                { 
                    foreach (OrderItem item in order.OrderItems)
                    {
                        foreach(var itemOpt in item.OrderItemOptions)
                        {
                            _context.OrderItemOptions.Remove(itemOpt);
                        }
                        _context.OrderItems.Remove(item);
                    }
                    _context.Orders.Remove(order);
                }
                
                await _context.SaveChangesAsync();
                scope.Complete();
            }
        }

        public Task<List<Order>> GetAllOrdersAsync()=> _context.Orders.ToListAsync();

        public Task<List<OrderItem>> GetOrderItems() => _context.OrderItems.Include(p => p.Product).ToListAsync();
        public Task<List<OrderStatus>> GetAllOrderStatusesAsync()=> _context.OrderStatuses.ToListAsync();

        public Task<List<ProductOption>> GetAllProductOptionsAsync()=> _context.ProductOptions.ToListAsync();

        public Task<List<Product>> GetAllProductsAsync() => _context.Products.ToListAsync();    

        public Task<List<ProductSize>> GetAllProductSizesAsync() => _context.ProductSizes.ToListAsync();

        public Task<List<Order>> GetOrdersByCustomerAsync(Guid customerId)=>
            _context.Orders.Where(o => o.CustomerId == customerId).ToListAsync();   

        public async Task<Order> UpdateOrderAsync(Order order)
        {
            if(!_context.Orders.Local.Any(o => o.Id == order.Id))
            {
                _context.Orders.Attach(order);
            }
            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return order;
        }
    }
}
