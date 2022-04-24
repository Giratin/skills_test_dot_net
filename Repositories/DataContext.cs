
//import entity framework
using skillset_test.Dtos;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace skillset_test.Repositories
{
    public class DataContext : DbContext
    {
        public DataContext() { }
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }


        public DbSet<Post> Posts { get; set; }
    }
}

