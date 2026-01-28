using api.bootcamp.clt.Api.Request;
using api.bootcamp.clt.Api.Response;
using api.bootcamp.clt.Aplication.Command.CreateProduct;
using api.bootcamp.clt.Aplication.Command.DeleteProduct;
using api.bootcamp.clt.Aplication.Command.UpdateProduct;
using api.bootcamp.clt.Aplication.Command.UpdateProductStatus;
using api.bootcamp.clt.Aplication.Query.GetProductById;
using api.bootcamp.clt.Aplication.Query.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

[ApiController]
[Route("v1/api/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProducts()
    {
        var products = await _mediator.Send(new GetProductsQuery());
        var list = products?.ToList() ?? new List<ProductResponse>();

        _logger.LogInformation("Productos obtenidos: {Count}", list.Count);

        return Ok(list);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> GetProductById([FromRoute] int id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id));

        if (product is null)
        {
            _logger.LogWarning("Producto con ID {ProductId} no encontrado.", id);
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductResponse>> CreateProduct([FromBody] CreateProductRequest request)
    {
        _logger.LogInformation("Creando producto: {@Request}", request);
        var result = await _mediator.Send(new CreateProductCommand(request));
        _logger.LogInformation("Producto creado con ID: {ProductId}", result.Id);
        return CreatedAtAction(nameof(GetProductById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductResponse>> UpdateProduct([FromRoute] int id, [FromBody] UpdateProductRequest request)
    {
        _logger.LogWarning("Inicia proceso de actualización del {ProductId}.", id);
        var result = await _mediator.Send(new UpdateProductCommand(id, request));
        _logger.LogWarning("Inicia proceso de actualización del {ProductId}.", id);
        return Ok(result);
    }

    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductResponse>> UpdateProductStatus([FromRoute] int id, [FromBody] PatchProductRequest request)
    {
        _logger.LogWarning("Inicia proceso de actualización estado del {ProductId}.", id);
        var result = await _mediator.Send(new UpdateProductStatusCommand(id, request.Activo));
        _logger.LogWarning("Inicia proceso de actualización estado del {ProductId}.", id);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProduct([FromRoute] int id)
    {
        _logger.LogWarning("Inicia proceso de eliminación del {ProductId}.", id);
        await _mediator.Send(new DeleteProductCommand(id));
        _logger.LogWarning("Finaliza proceso de eliminación del {ProductId}.", id);
        return NoContent();
    }
}