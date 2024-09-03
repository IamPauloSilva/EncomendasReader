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
    public class DeleteModel : PageModel
    {
        private readonly EncomendasProject.Data.ApplicationDbContext _context;

        public DeleteModel(EncomendasProject.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productsmodel = await _context.Products.FindAsync(id);
            if (productsmodel != null)
            {
                ProductsModel = productsmodel;
                _context.Products.Remove(ProductsModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
