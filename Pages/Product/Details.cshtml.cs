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
    public class DetailsModel : PageModel
    {
        private readonly EncomendasProject.Data.ApplicationDbContext _context;

        public DetailsModel(EncomendasProject.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public ProductsModel ProductsModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productsmodel = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
            if (productsmodel == null)
            {
                return NotFound();
            }
            else
            {
                ProductsModel = productsmodel;
            }
            return Page();
        }
    }
}
