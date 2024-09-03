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

namespace EncomendasProject.Pages.Encomenda
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public EncomendaModel EncomendaModel { get; set; }

        [BindProperty]
        public List<EncomendaProduct> EncomendaProducts { get; set; } = new List<EncomendaProduct>();

        
        public int SelectedProductId { get; set; }

        
        public int SelectedQuantity { get; set; }

        public IActionResult OnGet()
        {
            LoadSelectLists();
            return Page();
        }

        private void LoadSelectLists()
        {
            // Carrega as listas necessárias para os dropdowns do formulário
            ViewData["ClientList"] = new SelectList(_context.Clients, "Id", "Name");
        
            ViewData["ProductsList"] = _context.Products
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                }).ToList();
        }

        

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("Iniciando OnPostAsync");

            if (!ModelState.IsValid)
            {
                LoadSelectLists();
                return Page();
            }

            var productsJson = Request.Form["EncomendaProductsJson"];
            Console.WriteLine($"EncomendaProductsJson recebido: {productsJson}");

            if (string.IsNullOrEmpty(productsJson))
            {
                ModelState.AddModelError(string.Empty, "Adicione pelo menos um produto à encomenda.");
                LoadSelectLists();
                return Page();
            }

            List<EncomendaProduct> encomendaProducts;
            try
            {
                // Deserializa o JSON para uma lista de ProductDto
                var productsList = System.Text.Json.JsonSerializer.Deserialize<List<ProductDto>>(productsJson);

                // Log dos produtos recebidos após desserialização
                Console.WriteLine("Produtos recebidos após desserialização:");
                foreach (var product in productsList)
                {
                    Console.WriteLine($"ProductId: {product.ProductId}, Quantity: {product.Quantity}");
                }

                if (productsList == null || !productsList.Any())
                {
                    ModelState.AddModelError(string.Empty, "Adicione pelo menos um produto válido à encomenda.");
                    LoadSelectLists();
                    return Page();
                }

                var productIds = productsList.Select(p => p.ProductId).ToList();
                Console.WriteLine($"ProductIds: {string.Join(", ", productIds)}");

                var existingProducts = await _context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();

                // Log dos produtos encontrados no banco de dados
                Console.WriteLine("Produtos existentes no banco de dados:");
                foreach (var product in existingProducts)
                {
                    Console.WriteLine($"ProductId: {product.Id}, Name: {product.Name}");
                }

                // Verifica se todos os produtos existem
                if (existingProducts.Count != productsList.Count)
                {
                    ModelState.AddModelError(string.Empty, "Um ou mais produtos não foram encontrados no sistema.");
                    LoadSelectLists();
                    return Page();
                }

                // Mapeia os produtos selecionados para EncomendaProducts
                encomendaProducts = productsList.Select(p => new EncomendaProduct
                {
                    ProductId = p.ProductId,
                    Quantity = p.Quantity
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Ocorreu um erro ao processar os produtos.");
                LoadSelectLists();
                return Page();
            }

            // Atribui a lista de produtos à encomenda
            EncomendaModel.EncomendaProducts = encomendaProducts;

            try
            {
                _context.Encomendas.Add(EncomendaModel);
                await _context.SaveChangesAsync();
                Console.WriteLine("Encomenda salva com sucesso.");
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Ocorreu um erro ao salvar a encomenda. Tente novamente.");
                LoadSelectLists();
                return Page();
            }
        }


    }
}
