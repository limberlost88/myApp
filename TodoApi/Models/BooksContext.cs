using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace BooksApi.Models
{
    public class BooksContext : DbContext
    {
        public BooksContext(DbContextOptions<BooksContext> options)
            : base(options)
        {
        }

        public DbSet<BookItem> BookItems { get; set; }
        public double[] genreRatings {get; set;}
        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookItem>()
            .Ignore(b => b.category);
    }*/
    }
}