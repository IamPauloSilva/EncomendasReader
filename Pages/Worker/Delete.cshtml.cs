using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Encomendas.Models;
using EncomendasProject.Data;

namespace EncomendasProject.Pages.Worker
{
    public class DeleteModel : PageModel
    {
        private readonly EncomendasProject.Data.ApplicationDbContext _context;

        public DeleteModel(EncomendasProject.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workermodel = await _context.Workers.FindAsync(id);
            if (workermodel != null)
            {
                WorkerModel = workermodel;
                _context.Workers.Remove(WorkerModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
