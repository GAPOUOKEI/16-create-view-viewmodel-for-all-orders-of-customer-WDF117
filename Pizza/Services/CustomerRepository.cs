using Microsoft.EntityFrameworkCore;
using Pizza.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizza.Services
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly S117Context _context = new S117Context();

        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task DeleteCustomerAsync(Guid customerId)
        {
            var customer = _context.Customers
                .FirstOrDefault(x => x.Id == customerId);
            if(customer != null) 
            {
                _context.Customers.Remove(customer);
            }
            await _context.SaveChangesAsync();
        }

        public Task<Customer> GetCustomerByIdAsync(Guid customerId)
        {
            return _context.Customers.FirstOrDefaultAsync(x => x.Id == customerId);
        }

        public Task<List<Customer>> GetCustomersAsync()
        {
            return _context.Customers.ToListAsync();
        }

        public async Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            if(!_context.Customers.Local.Any(x => x.Id == customer.Id))
            {
                _context.Customers.Attach(customer);
            }
            _context.Entry(customer).State = EntityState.Modified;  
            await _context.SaveChangesAsync();
            return customer;
        }
    }
}
