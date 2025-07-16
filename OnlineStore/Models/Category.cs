namespace OnlineStore.Models
{
    public class MainCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Description { get; set; } 
        public string Image { get; set; } 
        public ICollection<MSubCategory> subCategories { get; set; }
    }
}
