using Encomendas.Enums;
using EncomendasProject.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Permissions;

namespace Encomendas.Models
{
    public class EncomendaModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O número da encomenda é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O número da encomenda deve ser maior que zero.")]
        public string EncomendaNumero { get; set; }

        [Required(ErrorMessage = "O cliente é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione um cliente válido.")]
        public int ClientId { get; set; }
        public ClientModel? Client { get; set; }
        public int? WorkerID { get; set; }
        public WorkerModel? Worker { get; set; }

        public EncomendasStatusEnum Status { get; set; } = EncomendasStatusEnum.NotTaken;

        public List<EncomendaProduct> EncomendaProducts { get; set; } = new List<EncomendaProduct>();

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataInicioPreparacao { get; set; }

        public DateTime? DataFimPreparacao { get; set; } 
    }
}