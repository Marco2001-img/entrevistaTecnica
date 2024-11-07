using EntrevistaBack.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;


namespace EntrevistaBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductosController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Acción GET: Para listar productos
        public async Task<IActionResult> Index(string search)
        {
            var client = _httpClientFactory.CreateClient();
            var url = "https://fakeapi.platzi.com/products";
            var response = await client.GetStringAsync(url);

            if (string.IsNullOrEmpty(response))
            {
                return View(new List<Producto>());
            }

            var productos = JsonSerializer.Deserialize<List<Producto>>(response);

            // Filtrado por nombre
            if (!string.IsNullOrEmpty(search))
            {
                productos = productos.Where(p => p.Nombre.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return View(productos);
        }

        // Acción POST: Para agregar un nuevo producto
        [HttpPost]
        public async Task<IActionResult> Crear(Producto producto, IFormFile imagen)
        {
            var client = _httpClientFactory.CreateClient();
            var url = "https://fakeapi.platzi.com/products";

            // Subida de archivo de imagen
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(producto.Nombre), "nombre");
            formData.Add(new StringContent(producto.Precio.ToString()), "precio");
            formData.Add(new StringContent(producto.Categoria), "categoria");

            if (imagen != null)
            {
                var fileContent = new StreamContent(imagen.OpenReadStream());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imagen.ContentType);
                formData.Add(fileContent, "imagen", imagen.FileName);
            }

            var response = await client.PostAsync(url, formData);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");  // Redirige a la lista de productos
            }

            return View();
        }

        // Acción PUT: Para editar un producto
        [HttpPost]
        public async Task<IActionResult> Editar(int id, Producto producto)
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"https://fakeapi.platzi.com/products/{id}";

            var jsonContent = JsonSerializer.Serialize(producto);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View();
        }

    }
}
