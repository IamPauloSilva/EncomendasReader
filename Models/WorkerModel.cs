using Encomendas.Enums;
using System.ComponentModel.DataAnnotations;

namespace Encomendas.Models
{
    public class WorkerModel
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public int WorkerNumber { get; set; }

        public ShiftsEnum Shift { get; set; }
        public JobsEnum Job { get; set; }

        public ICollection<EncomendaModel>? Encomendas { get; set; }

    }
}
