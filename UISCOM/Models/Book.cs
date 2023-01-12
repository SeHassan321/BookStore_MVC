
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BookStore.Models
{
    public class Book 
    {
        public int Id { get; set; }

        [Required, MaxLength(250)]
        public string Title { get; set; }
        public List<AuthorBook> AuthorBooks { get; set; }
        public double Rate { get; set; }

        [Required, MaxLength(2500)]
        public string Storeline { get; set; }
        public int Year { get; set; }
        public int Price { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        [ForeignKey("Author")]
        public int AutherId { get; set; }
        public Author Author { get; set; }
        public byte[] Poster { get; set; }
    }

}
