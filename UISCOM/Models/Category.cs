

using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Category 
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }
        public List<Book> Books { get; set; }
    }
}
