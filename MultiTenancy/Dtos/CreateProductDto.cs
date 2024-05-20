namespace MultiTenancy.Dtos
{
    public class CreateProductDto
    {
        public string Name { get; set; } = null!;
        public string Descriptions { get; set; } = null!;
        public int Rate { get; set; }
    }
}
