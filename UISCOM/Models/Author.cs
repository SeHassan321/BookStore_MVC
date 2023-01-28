namespace BookStore.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List <AuthorBook> AuthorBook { get; set; }
    }
}