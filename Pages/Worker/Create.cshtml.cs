using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Encomendas.Models;
using EncomendasProject.Data;
using Encomendas.Enums;

namespace EncomendasProject.Pages.Worker
{
    public class CreateModel : PageModel
    {
        private readonly EncomendasProject.Data.ApplicationDbContext _context;

        public CreateModel(EncomendasProject.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            JobsList = new SelectList(Enum.GetValues(typeof(JobsEnum)));
            ShiftsList = new SelectList(Enum.GetValues(typeof(ShiftsEnum)));
            return Page();
        }

        [BindProperty]
        public WorkerModel WorkerModel { get; set; } = default!;

        public SelectList JobsList { get; set; }
        public SelectList ShiftsList { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ShiftsList = new SelectList(Enum.GetValues(typeof(ShiftsEnum)));
                JobsList = new SelectList(Enum.GetValues(typeof(JobsEnum)));
                return Page();
            }

            _context.Workers.Add(WorkerModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
