using Microsoft.AspNetCore.Mvc;
using Store.G02.Shared.Dtos.Products;
using Store.G02.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Store.G02.Shared.ErrorModels;
using Store.G02.Presentation.Attributes;


namespace Store.G02.Presentation.Controllers
{
    [ApiController]
    [Route("apis/[controller]")]
    public class ProductsController (IServiceManager _serviceManager) : ControllerBase
    {

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginationResponse<ProductResponse>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [Cache(60)]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductQueryParameters Params)
        {
            var Products = await _serviceManager.productService.GetAllProductsAsync(Params);
            if (Products is null)
                return BadRequest();
            return Ok(Products);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> GetProductById(int? id)
        {
            if(id is null)
                return BadRequest();
            var Product = await _serviceManager.productService.GetProductByIdAsync(id.Value);
            //if (Product is null)
            //    return NotFound();
            return Ok(Product);
        }


        [HttpGet("Brands")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BrandTypeResponse>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> GetAllBrands()
        {
            var Brands = await _serviceManager.productService.GetAllBrandsAsync();
            if (Brands is null)
                return BadRequest();
            return Ok(Brands);
        }


        [HttpGet("Types")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BrandTypeResponse>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        public async Task<ActionResult<IEnumerable<BrandTypeResponse>>> GetAllTypes()
        {
            var Types = await _serviceManager.productService.GetAllTypesAsync();
            if (Types is null)
                return BadRequest();
            return Ok(Types);
        }


    }
}
