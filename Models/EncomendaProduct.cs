using EncomendasProject.Models;
using System.ComponentModel.DataAnnotations;

namespace Encomendas.Models
{
    public class EncomendaProduct
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }
        public ProductsModel Product { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser pelo menos 1.")]
        public int Quantity { get; set; }

        [Required]
        public int EncomendaId { get; set; }
        public EncomendaModel Encomenda { get; set; }
    }
}
