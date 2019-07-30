using Data_Access_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;

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

    public class DbInitializer : CreateDatabaseIfNotExists<DealContext>
    {
        protected override void Seed(DealContext context)
        {
            var products = new List<Product>
            {
                new Product(){Name = "Case Corsair Crystal",Manufacturer = "Corsair",YearOfIssue = new DateTime(2018,5,27)},
                new Product(){Name = "Seasonic Prime Ultra",Manufacturer = "Seasonic",YearOfIssue = new DateTime(2017,1,20)},
                new Product(){Name = "Intel Core i7-7700K",Manufacturer = "Intel",YearOfIssue = new DateTime(2019,8,10)}
            };

            context.Products.AddRange(products);

            context.SaveChanges();
        }
    }
}