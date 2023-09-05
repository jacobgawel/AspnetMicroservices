using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reflection;

namespace Catalog.API.Controllers
{
    [ApiController]
    // this is the route of the controller where the endpoint is
    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {

        // injects the abstracted catalog database context (repository)
        private readonly IProductRepository _productRepository;

        // enables logging capabilities for the controller 
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(IProductRepository repository, ILogger<CatalogController> logger)
        {
            _productRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet]
        // specifies the return type of the api endpoint
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct()
        {
            var products = await _productRepository.GetProducts();
            return Ok(products);
        }

        // adds a restriction of what the length should be of the id string
        // names the endpoint GetProduct/getproduct
        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> GetProductById(string id)
        {
            var product = await _productRepository.GetProduct(id);
            
            if(product ==  null)
            {
                _logger.LogError($"Product with the id: {id}, not found");
                return NotFound();
            }

            return Ok(product);
        }


        [HttpGet]
        // action defines the route dynamically and takes the [action] and replaces it with the name of the method
        // e.g. GetProductByCategory
        // {category} is defined in the paramater which will be passed dynamically as well
        [Route("[action]/{category}", Name = "GetProductByCategory")]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category)
        {
            var products = await _productRepository.GetProductByCategory(category);
            return Ok(products);
        }

        [HttpGet]
        [Route("[action]/{name}", Name = "GetProductByName")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByName(string name)
        {
            var product = await _productRepository.GetProductByName(name);
            return Ok(product);
        }

        // POST: api/v1/catalog Product object
        [HttpPost]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            await _productRepository.CreateProduct(product);
            // redirects the route to GetProduct alonside product id
            return CreatedAtRoute("GetProduct", new {id =  product.Id}, product);
        }

        // UPDATE: api/v1/catalog/ Product object
        [HttpPut]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            return Ok(await _productRepository.UpdateProduct(product));
        }

        // DELETE: api/v1/catalog/{id}
        [HttpDelete]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            return Ok(await _productRepository.DeleteProduct(id));
        }
    }
}
