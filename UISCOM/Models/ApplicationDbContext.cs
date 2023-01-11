using BookStore.Models;
using Microsoft.EntityFrameworkCore;

namespace UISCOM.Models
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)   
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authers { get; set; }
        public DbSet<AuthorBook> AuthorBooks { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
