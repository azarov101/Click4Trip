using Click4Trip.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Click4Trip.DAL
{
    public class DataLayer : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Customer>().ToTable("Customer");
            modelBuilder.Entity<FlightOrder>().ToTable("FlightOrder");
            modelBuilder.Entity<HotelOrder>().ToTable("HotelOrder");
            modelBuilder.Entity<HotelOrderDetails>().ToTable("HotelOrderDetails");
            modelBuilder.Entity<HotelReview>().ToTable("HotelReview");
            modelBuilder.Entity<Order>().ToTable("Order");
            modelBuilder.Entity<PackageOrder>().ToTable("PackageOrder");
            modelBuilder.Entity<QuickSupportTicket>().ToTable("QuickSupportTicket");
            modelBuilder.Entity<RentCarOrder>().ToTable("RentCarOrder");
            modelBuilder.Entity<SmartAgent>().ToTable("SmartAgent");
            modelBuilder.Entity<SupportTicket>().ToTable("SupportTicket");
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<UserPass>().ToTable("UserPass");
            modelBuilder.Entity<UserSalt>().ToTable("UserSalt");
            modelBuilder.Entity<Locations>().ToTable("Locations");

        }

        public DbSet<Customer> customers { get; set; }
        public DbSet<FlightOrder> flightOrders { get; set; }
        public DbSet<HotelOrder> hotelOrders { get; set; }
        public DbSet<HotelOrderDetails> hotelOrderDetails { get; set; }
        public DbSet<HotelReview> hotelReviews { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<PackageOrder> packageOrders { get; set; }
        public DbSet<QuickSupportTicket> quickSupportTickets { get; set; }
        public DbSet<RentCarOrder> rentCarOrder { get; set; }
        public DbSet<SmartAgent> smartAgent { get; set; }
        public DbSet<SupportTicket> supportTickets { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<UserPass> userPass { get; set; }
        public DbSet<UserSalt> userSalt { get; set; }
        public DbSet<Locations> locations { get; set; }
    }
}