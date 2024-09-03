using Encomendas.Enums;
using Encomendas.Models;
using EncomendasProject.Models;
using EncomendasProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EncomendasProject.Models.Dto;
using Newtonsoft.Json;

namespace EncomendasProject.Pages.Encomenda
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public EncomendaModel EncomendaModel { get; set; }

        [BindProperty]
        public List<EncomendaProduct> EncomendaProducts { get; set; } = new List<EncomendaProduct>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            EncomendaModel = await _context.Encomendas
                .Include(e => e.EncomendaProducts)
                    .ThenInclude(ep => ep.Product)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (EncomendaModel == null)
            {
                return NotFound();
            }

            LoadSelectLists();

            // Inicializa EncomendaProducts se for nulo
            EncomendaModel.EncomendaProducts ??= new List<EncomendaProduct>();

            // Serializa os produtos existentes para o ViewData
            var existingProducts = EncomendaModel.EncomendaProducts
                .Select(p => new { p.ProductId, p.Quantity })
                .ToList();
            ViewData["ExistingProductsJson"] = JsonConvert.SerializeObject(existingProducts);

            return Page();
        }

        private void LoadSelectLists()
        {
            // Carrega as listas necessárias para os dropdowns do formulário
            ViewData["ClientList"] = new SelectList(_context.Clients, "Id", "Name");
            ViewData["WorkerList"] = new SelectList(_context.Workers, "Id", "Name");
            ViewData["StatusList"] = new SelectList(Enum.GetValues(typeof(EncomendasStatusEnum)).Cast<EncomendasStatusEnum>());
            ViewData["ProductsList"] = _context.Products
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                }).ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                LoadSelectLists();
                return Page();
            }

            var productsJson = Request.Form["EncomendaProductsJson"];
            if (string.IsNullOrEmpty(productsJson))
            {
                ModelState.AddModelError(string.Empty, "Adicione pelo menos um produto à encomenda.");
                LoadSelectLists();
                return Page();
            }

            List<EncomendaProduct> encomendaProducts;
            try
            {
                var productsList = JsonConvert.DeserializeObject<List<ProductDto>>(productsJson);

                if (productsList == null || !productsList.Any())
                {
                    ModelState.AddModelError(string.Empty, "Adicione pelo menos um produto válido à encomenda.");
                    LoadSelectLists();
                    return Page();
                }

                var productIds = productsList.Select(p => p.ProductId).ToList();
                var existingProducts = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync();

                if (existingProducts.Count != productIds.Count)
                {
                    var missingProductIds = productIds.Except(existingProducts.Select(p => p.Id)).ToList();
                    ModelState.AddModelError(string.Empty, "Um ou mais produtos não foram encontrados no sistema.");
                    LoadSelectLists();
                    return Page();
                }

                encomendaProducts = productsList.Select(p => new EncomendaProduct
                {
                    ProductId = p.ProductId,
                    Quantity = p.Quantity
                }).ToList();

                if (EncomendaModel.Id == 0) // Criar nova encomenda
                {
                    _context.Encomendas.Add(EncomendaModel);
                }
                else // Atualizar encomenda existente
                {
                    var existingEncomenda = await _context.Encomendas
                        .Include(e => e.EncomendaProducts)
                        .FirstOrDefaultAsync(e => e.Id == EncomendaModel.Id);

                    if (existingEncomenda == null)
                    {
                        ModelState.AddModelError(string.Empty, "Encomenda não encontrada.");
                        LoadSelectLists();
                        return Page();
                    }

                    _context.Entry(existingEncomenda).CurrentValues.SetValues(EncomendaModel);
                    _context.EncomendaProducts.RemoveRange(existingEncomenda.EncomendaProducts);
                    _context.EncomendaProducts.AddRange(encomendaProducts);
                }

                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                ModelState.AddModelError(string.Empty, "Ocorreu um erro ao salvar a encomenda. Tente novamente.");
                LoadSelectLists();
                return Page();
            }
        }


        private bool EncomendaExists(int id)
        {
            return _context.Encomendas.Any(e => e.Id == id);
        }
    }
}
