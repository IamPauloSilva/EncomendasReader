using Encomendas.Enums;
using Encomendas.Models;
using EncomendasProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EncomendasProject.Pages.Reader
{
    public class ReaderModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ReaderModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string QRCodeData { get; set; }

        public EncomendaModel Encomenda { get; set; }
        public WorkerModel Worker { get; set; }
        public string Message { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(QRCodeData))
            {
                Message = "QR Code data is missing.";
                return Page();
            }

            // Assume QR code data has the format "EncomendaNumber_WorkerNumber"
            var parts = QRCodeData.Split('_');
            if (parts.Length != 2)
            {
                Message = "Invalid QR Code format.";
                return Page(); // Invalid format
            }

            var encomendaNumero = parts[0];
            var workerNumberString = parts[1];

            // Convert workerNumberString to integer
            if (!int.TryParse(workerNumberString, out var workerNumber))
            {
                Message = "Invalid worker number format.";
                return Page(); // Invalid worker number format
            }

            // Fetch Encomenda
            Encomenda = await _context.Encomendas
                .Include(e => e.Worker)
                .FirstOrDefaultAsync(e => e.EncomendaNumero == encomendaNumero);

            // Fetch Worker
            Worker = await _context.Workers
                .FirstOrDefaultAsync(w => w.WorkerNumber == workerNumber);

            if (Encomenda == null || Worker == null)
            {
                Message = "Encomenda or Worker not found.";
                return Page(); // Encomenda or Worker not found
            }

            // Update Encomenda status
            if (Encomenda.Status == EncomendasStatusEnum.NotTaken)
            {
                Encomenda.Status = EncomendasStatusEnum.InPreparation;
                Encomenda.Worker = Worker;
                Encomenda.DataInicioPreparacao = DateTime.Now;
                Message = "Encomenda Iniciada com sucesso";
            }
            else if (Encomenda.Status == EncomendasStatusEnum.InPreparation)
            {
                Encomenda.Status = EncomendasStatusEnum.Prepared;
                Encomenda.DataFimPreparacao = DateTime.Now;
                Message = "Encomenda Entregue com sucesso";
            }

            _context.Encomendas.Update(Encomenda);
            await _context.SaveChangesAsync();

            return Page();
        }
    }
}
