using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EncomendasProject.Data;
using EncomendasProject.Models;

namespace EncomendasProject.Pages.Product
{
    public class IndexModel : PageModel
    {
        private readonly EncomendasProject.Data.ApplicationDbContext _context;

        public IndexModel(EncomendasProject.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<ProductsModel> ProductsModel { get;set; } = default!;

        public async Task OnGetAsync()
        {
            ProductsModel = await _context.Products.ToListAsync();
        }
    }
}
