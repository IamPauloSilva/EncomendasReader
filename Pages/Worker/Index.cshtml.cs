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
    public class IndexModel : PageModel
    {
        private readonly EncomendasProject.Data.ApplicationDbContext _context;

        public IndexModel(EncomendasProject.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<WorkerModel> WorkerModel { get;set; } = default!;

        public async Task OnGetAsync()
        {
            WorkerModel = await _context.Workers.ToListAsync();
        }
    }
}
