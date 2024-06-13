using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SWPApp.Models;

public class CustomerService
{
    private readonly DiamondAssesmentSystemDBContext _context;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(DiamondAssesmentSystemDBContext context, ILogger<CustomerService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task CreateCustomerAsync(RegisterModel model)
    {
        var customer = new Customer
        {
            Email = model.Email,
            Password = model.Password, // Make sure to hash the password before storing it
            ConfirmationToken = GenerateConfirmationToken(),
            EmailConfirmed = false,
            Status = true // or some default value based on your business logic
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
    }

    private string GenerateConfirmationToken()
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            var byteToken = new byte[32];
            rng.GetBytes(byteToken);
            return Convert.ToBase64String(byteToken);
        }
    }
}
