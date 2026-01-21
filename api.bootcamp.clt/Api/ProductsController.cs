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
    /// Obtiene el detalle de un producto por su identificador.
    /// </summary>
    /// <param name="id">Identificador del producto.</param>
    /// <returns>Producto encontrado.</returns>
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
        catch (Exception)
        {
            _logger.LogError("Error al obtener los productos.");
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
            _logger.LogError(ex, "Error al obtener el producto con ID {ProductId}", id);

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
            // Crear el comando con los datos recibidos en la solicitud
            var command = new CreateProductCommand(request);

            // Enviar el comando al mediador para ser manejado por el handler
            var result = await _mediator.Send(command);

            // Retornar la respuesta con el estado de "Creado" y los datos del producto creado
            return CreatedAtAction(nameof(CreateProducto), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            // Manejo de errores, si ocurre algún problema
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
            var result = await _mediator.Send(new UpdateProductCommand(id, request));

            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Message = "No se ha encontrado el producto especificado"});
        }
        catch(Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error al procesar la solicitud, inténtelo nuevamente más tarde." });
        }
    }

    /// <summary>
    /// Actualiza parcialmente un producto existente.
    /// Solo se modificarán los campos enviados.
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
            var command = new UpdateProductStatusCommand(id, request.Activo);

            var result = await _mediator.Send(command);

            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Message = "Producto no encontrado." });
        }
        catch (Exception ex)
        {
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
            var result = await _mediator.Send(new DeleteProductCommand(id));

            if (!result)
            {
                return NotFound(new { Message = "Producto no encontrado." });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
        }
    }
}
