using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Encomendas.Models;
using EncomendasProject.Data;
using Encomendas.Enums;

namespace EncomendasProject.Pages.Worker
{
    public class EditModel : PageModel
    {
        private readonly EncomendasProject.Data.ApplicationDbContext _context;

        public EditModel(EncomendasProject.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public WorkerModel WorkerModel { get; set; } = default!;

        public SelectList JobsList { get; set; }
        public SelectList ShiftsList { get; set; }

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

            WorkerModel = workermodel;

            // Preencher a lista de seleção com os valores do enum JobsEnums e ShiftEnum
            JobsList = new SelectList(Enum.GetValues(typeof(JobsEnum)), WorkerModel.Job);
            ShiftsList = new SelectList(Enum.GetValues(typeof(ShiftsEnum)), WorkerModel.Shift);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Recarregar as listas de seleção se o modelo não for válido
                JobsList = new SelectList(Enum.GetValues(typeof(JobsEnum)), WorkerModel.Job);
                ShiftsList = new SelectList(Enum.GetValues(typeof(ShiftsEnum)), WorkerModel.Shift);
                return Page();
            }

            _context.Attach(WorkerModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkerModelExists(WorkerModel.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool WorkerModelExists(int id)
        {
            return _context.Workers.Any(e => e.Id == id);
        }
    }
}
