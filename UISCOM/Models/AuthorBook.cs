using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models
{
    public class AuthorBook
    {
        public int Id { get; set; }
        public Author Author { get; set; }

        [ForeignKey("Author")]
        public int AuthorId { get; set; }
        public Book Book { get; set; }

        [ForeignKey("Book")]
        public int BookId { get; set; }
    }
}