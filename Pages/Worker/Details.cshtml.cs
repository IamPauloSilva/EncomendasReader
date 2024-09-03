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
            // Step 1: Initialize the QR Code generator
            QRCodeGenerator qrGenerator = new QRCodeGenerator();

            // Step 2: Create the QR Code data
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(workerNumber, QRCodeGenerator.ECCLevel.Q);

            // Step 3: Use BitmapByteQRCode to generate a bitmap from the QR code data
            BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
            byte[] qrCodeBytes = qrCode.GetGraphic(20);

            // Step 4: Convert the byte array to a Bitmap
            using (MemoryStream ms = new MemoryStream(qrCodeBytes))
            {
                using (Bitmap qrCodeImage = new Bitmap(ms))
                {
                    // Step 5: Save the bitmap to a stream and return it as a PNG file
                    using (var stream = new MemoryStream())
                    {
                        qrCodeImage.Save(stream, ImageFormat.Png);
                        return File(stream.ToArray(), "image/png");
                    }
                }
            }
        }
    }
}
