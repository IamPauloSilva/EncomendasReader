using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Encomendas.Models;
using EncomendasProject.Data;

namespace EncomendasProject.Pages.Encomenda
{
    public class DeleteModel : PageModel
    {
        private readonly EncomendasProject.Data.ApplicationDbContext _context;

        public DeleteModel(EncomendasProject.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public EncomendaModel EncomendaModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var encomendamodel = await _context.Encomendas.FirstOrDefaultAsync(m => m.Id == id);

            if (encomendamodel == null)
            {
                return NotFound();
            }
            else
            {
                EncomendaModel = encomendamodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var encomendamodel = await _context.Encomendas.FindAsync(id);
            if (encomendamodel != null)
            {
                EncomendaModel = encomendamodel;
                _context.Encomendas.Remove(EncomendaModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
