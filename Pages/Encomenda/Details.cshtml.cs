using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Encomendas.Models;
using EncomendasProject.Data;
using QRCoder;
using System.Drawing;
using iTextSharp.text.pdf.qrcode;
using System.Drawing.Imaging;

namespace EncomendasProject.Pages.Encomenda
{
    public class DetailsModel : PageModel
    {
        private readonly EncomendasProject.Data.ApplicationDbContext _context;

        public DetailsModel(EncomendasProject.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public EncomendaModel EncomendaModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var encomendamodel = await _context.Encomendas
                .Include(e => e.Client)                // Inclui o cliente associado
                .Include(e => e.Worker)                // Inclui o trabalhador associado
                .Include(e => e.EncomendaProducts)    // Inclui os produtos da encomenda
                    .ThenInclude(ep => ep.Product)    // Inclui os detalhes do produto associado
                .FirstOrDefaultAsync(m => m.Id == id);

            if (encomendamodel == null)
            {
                return NotFound();
            }

            EncomendaModel = encomendamodel;
            return Page();
        }
        public IActionResult OnGetGenerateQRCode(string encomendaNumero)
        {
            // Step 1: Initialize the QR Code generator
            QRCodeGenerator qrGenerator = new QRCodeGenerator();

            // Step 2: Create the QR Code data
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(encomendaNumero, QRCodeGenerator.ECCLevel.Q);

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
