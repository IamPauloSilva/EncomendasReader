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
            try
            {
                if (string.IsNullOrEmpty(encomendaNumero))
                {
                    return BadRequest("Encomenda número não fornecido.");
                }

                // Initialize QR Code generator
                QRCodeGenerator qrGenerator = new QRCodeGenerator();

                // Create QR Code data
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(encomendaNumero, QRCodeGenerator.ECCLevel.Q);

                // Generate bitmap from QR Code data
                BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
                byte[] qrCodeBytes = qrCode.GetGraphic(20);

                // Return QR Code as PNG
                return File(qrCodeBytes, "image/png");
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Erro interno do servidor.");
            }
        }
    }
}
