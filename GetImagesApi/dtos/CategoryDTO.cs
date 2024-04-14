namespace GetImagesApi.dtos
{
    public class CategoryDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
    }
}
