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
            Console.WriteLine("Received QRCodeData: " + QRCodeData); // Logging the received data

            if (string.IsNullOrEmpty(QRCodeData))
            {
                return new JsonResult(new { success = false, message = "QR Code data is missing." });
            }

            var parts = QRCodeData.Split('_');
            if (parts.Length != 2)
            {
                return new JsonResult(new { success = false, message = "Invalid QR Code format." });
            }

            var encomendaNumero = parts[0];
            var workerNumberString = parts[1];

            if (!int.TryParse(workerNumberString, out var workerNumber))
            {
                return new JsonResult(new { success = false, message = "Invalid worker number format." });
            }

            Encomenda = await _context.Encomendas
                .Include(e => e.Worker)
                .FirstOrDefaultAsync(e => e.EncomendaNumero == encomendaNumero);

            Worker = await _context.Workers
                .FirstOrDefaultAsync(w => w.WorkerNumber == workerNumber);

            if (Encomenda == null || Worker == null)
            {
                return new JsonResult(new { success = false, message = "Encomenda or Worker not found." });
            }

            if (Encomenda.Status == EncomendasStatusEnum.NotTaken)
            {
                Encomenda.Status = EncomendasStatusEnum.InPreparation;
                Encomenda.WorkerID = Worker.WorkerNumber;
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

            Console.WriteLine("Update successful, status: " + Encomenda.Status);

            return new JsonResult(new { success = true, message = Message });
        }
    }
}
