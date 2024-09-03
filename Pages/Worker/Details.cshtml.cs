using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EncomendasProject.Data;
using Encomendas.Models;

namespace EncomendasProject.Pages.Worker
{
    public class DetailsModel : PageModel
    {
        private readonly EncomendasProject.Data.ApplicationDbContext _context;

        public DetailsModel(EncomendasProject.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public WorkerModel WorkerModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workermodel = await _context.Workers.FirstOrDefaultAsync(m => m.Id == id);
            if (workermodel == null)
            {
                return NotFound();
            }
            else
            {
                WorkerModel = workermodel;
            }
            return Page();
        }

        // Handler to generate QR code
        public IActionResult OnGetGenerateQRCode(string workerNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(workerNumber))
                {
                    return BadRequest("Número do trabalhador não fornecido.");
                }

                // Initialize QR Code generator
                QRCodeGenerator qrGenerator = new QRCodeGenerator();

                // Create QR Code data
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(workerNumber, QRCodeGenerator.ECCLevel.Q);

                // Generate bitmap from QR Code data
                BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
                byte[] qrCodeBytes = qrCode.GetGraphic(20);

                // Return QR Code as PNG
                return File(qrCodeBytes, "image/png");
            }
            catch (Exception ex)
            {
                // Log exception (ex) if needed
                return StatusCode(500, "Erro interno do servidor.");
            }
        }
    }
}
