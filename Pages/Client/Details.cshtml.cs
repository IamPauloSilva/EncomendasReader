using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Encomendas.Models;
using EncomendasProject.Data;

namespace EncomendasProject.Pages.Client
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public ClientModel ClientModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientmodel = await _context.Clients.FirstOrDefaultAsync(m => m.Id == id);
            if (clientmodel == null)
            {
                return NotFound();
            }
            else
            {
                ClientModel = clientmodel;
            }
            return Page();
        }
    }
}
