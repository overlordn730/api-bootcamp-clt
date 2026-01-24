using api.bootcamp.clt.Api.Request;
using api.bootcamp.clt.Api.Response;
using api.bootcamp.clt.Aplication.Command.CreateProduct;
using api.bootcamp.clt.Aplication.Command.DeleteProduct;
using api.bootcamp.clt.Aplication.Command.UpdateProduct;
using api.bootcamp.clt.Aplication.Command.UpdateProductStatus;
using api.bootcamp.clt.Aplication.Query;
using api.bootcamp.clt.Aplication.Query.GetProductById;
using api.bootcamp.clt.Aplication.Query.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
public class ProductsController : Controller
{
    private readonly IMediator _mediator;
    public readonly ILogger _logger;

    public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Método para obtener todos los productos.
    /// </summary>
    /// <returns></returns>
    [HttpGet("v1/api/products")]
    [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProductoByIdAsync()
    {
        _logger.LogInformation("Iniciando obtención de productos.");
        try
        {
            var query = new GetProductsQuery();
            var products = await _mediator.Send(query);

            if (products == null || products.Count == 0)
            {
                _logger.LogWarning("No se encontraron productos.");
                return NotFound();
            }

            _logger.LogInformation("Productos obtenidos exitosamente.");
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los productos.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error al procesar la solicitud, inténtelo nuevamente más tarde." });
        }        
    }

    /// <summary>
    /// Obtiene el detalle de un producto por su identificador.
    /// </summary>
    /// <param name="id">Identificador del producto.</param>
    /// <returns>Producto encontrado.</returns>
    [HttpGet("v1/api/products/{id:int}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductResponse>> GetProductById([FromRoute] int id)
    {
        try
        {
            var query = new GetProductByIdQuery(id);

            var product = await _mediator.Send(query);

            if (product == null)
            {
                _logger.LogWarning("Producto con ID {ProductId} no encontrado.", id);
                return NotFound();
            }

            _logger.LogInformation("Producto con ID {ProductId} obtenido exitosamente.", id);

            return Ok(product);
        }
        catch (Exception ex) 
        { 
            _logger.LogError(ex, "Error al obtener el producto con ID {ProductId}.", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
        }
        
    }

    /// <summary>
    /// Crea un nuevo producto.
    /// </summary>
    /// <param name="request">Datos del producto a crear.</param>
    /// <returns>Producto creado.</returns>
    [HttpPost("v1/api/products")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductResponse>> CreateProducto([FromBody] CreateProductRequest request)
    {
        try
        {
            _logger.LogInformation("Iniciando creación de producto.");

            var command = new CreateProductCommand(request);

            var result = await _mediator.Send(command);

            _logger.LogInformation("Producto creado exitosamente con ID {ProductId}.", result.Id);

            return CreatedAtAction(nameof(CreateProducto), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el producto.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza completamente un producto existente.
    /// </summary>
    /// <param name="id">Identificador del producto.</param>
    /// <param name="request">Datos completos a actualizar.</param>
    [HttpPut("v1/api/products/{id:int}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductResponse>> UpdateProducto(
        [FromRoute] int id,
        [FromBody] UpdateProductRequest request)
    {
        try
        {

            _logger.LogInformation("Iniciando actualización del producto con ID {ProductId}.", id);

            var result = await _mediator.Send(new UpdateProductCommand(id, request));

            _logger.LogInformation("Producto con ID {ProductId} actualizado exitosamente.", id);

            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Producto con ID {ProductId} no encontrado para actualización.", id);
            return NotFound(new { Message = "No se ha encontrado el producto especificado"});
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el producto con ID {ProductId}.", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error al procesar la solicitud, inténtelo nuevamente más tarde." });
        }
    }

    /// <summary>
    /// Método para actualizar el estado de un producto existente.
    /// </summary>
    /// <param name="id">Identificador del producto.</param>
    /// <param name="request">Campos a actualizar.</param>
    [HttpPatch("v1/api/products/{id:int}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductResponse>> PatchProducto(
        [FromRoute] int id,
        [FromBody] PatchProductRequest request)
    {
        try
        {
            _logger.LogInformation("Iniciando actualización de estado del producto con ID {ProductId}.", id);

            var command = new UpdateProductStatusCommand(id, request.Activo);

            var result = await _mediator.Send(command);


            _logger.LogInformation("Estado del producto con ID {ProductId} actualizado exitosamente.", id);

            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Producto con ID {ProductId} no encontrado para actualización de estado.", id);
            return NotFound(new { Message = "Producto no encontrado." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el estado del producto con ID {ProductId}.", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Elimina un producto existente.
    /// </summary>
    /// <param name="id">Identificador del producto.</param>
    [HttpDelete("v1/api/products/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProducto([FromRoute] int id)
    {
        try
        {
            _logger.LogInformation("Iniciando eliminación del producto con ID {ProductId}.", id);

            var result = await _mediator.Send(new DeleteProductCommand(id));

            if (!result)
            {
                return NotFound(new { Message = "Producto no encontrado." });
            }

            _logger.LogInformation("Producto con ID {ProductId} eliminado exitosamente.", id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el producto con ID {ProductId}.", id); 
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
        }
    }
}
