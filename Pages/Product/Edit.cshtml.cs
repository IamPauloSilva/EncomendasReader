using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EncomendasProject.Data;
using EncomendasProject.Models;

namespace EncomendasProject.Pages.Product
{
    public class EditModel : PageModel
    {
        private readonly EncomendasProject.Data.ApplicationDbContext _context;

        public EditModel(EncomendasProject.Data.ApplicationDbContext context)
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

            var productsmodel =  await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
            if (productsmodel == null)
            {
                return NotFound();
            }
            ProductsModel = productsmodel;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(ProductsModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductsModelExists(ProductsModel.Id))
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

        private bool ProductsModelExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
