using Data_Access_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Data_Access_Layer.EF
{
    public class DealContext : DbContext
    {
        public DealContext(string connectionString)
            : base(connectionString)
        { }

        static DealContext()
        {
            Database.SetInitializer(new DbInitializer());
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
    }

    public class DbInitializer : DropCreateDatabaseAlways<DealContext>
    {
        protected override void Seed(DealContext context)
        {
            var computerParts = new List<Product>
            {
                new Product(){Name = "Case Corsair Crystal",Manufacturer = "Corsair",YearOfIssue = new DateTime(2018,5,27)},
                new Product(){Name = "Seasonic Prime Ultra",Manufacturer = "Seasonic",YearOfIssue = new DateTime(2017,1,20)},
                new Product(){Name = "Intel Core i7-7700K",Manufacturer = "Intel",YearOfIssue = new DateTime(2019,8,10)}
            };

            var bicycleParts = new List<Product>
            {
                new Product(){Name = "Bicycle Wheel",Manufacturer = "Brooks",YearOfIssue = new DateTime(2019,8,2)},
                new Product(){Name = "Grip Shift",Manufacturer = "Shimano",YearOfIssue = new DateTime(2017,3,1)},
            };

            var orders = new List<Order>()
            {
                new Order(){Name = "Computer parts" , Products = computerParts},
                new Order(){Name = "Bicycle parts", Products = bicycleParts}
            };

            context.Products.AddRange(computerParts);
            context.Products.AddRange(bicycleParts);

            context.Orders.AddRange(orders);

            context.SaveChanges();
        }
    }
}