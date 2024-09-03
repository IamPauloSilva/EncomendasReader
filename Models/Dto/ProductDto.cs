using System.Text.Json.Serialization;

namespace EncomendasProject.Models.Dto
{

    public class ProductDto
    {
        [JsonPropertyName("productId")]
        public int ProductId { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

    }
}