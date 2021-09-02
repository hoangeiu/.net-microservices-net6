using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mango.Web.Controllers;
public class ProductController : Controller
{
    private readonly IProductService _productService;
    public ProductController(IProductService productService)
    {
        _productService = productService;
    }
    public async Task<IActionResult> Index()
    {
        List<ProductDto> list = new();
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var response = await _productService.GetAllProductsAsync<ResponseDto>(accessToken);
        if (response != null && response.IsSuccess)
        {
            list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
        }
        return View(list);
    }

    public async Task<IActionResult> Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductDto model)
    {
        if (ModelState.IsValid)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.CreateProductAsync<ResponseDto>(model, accessToken);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }
        }        

        return View(model);
    }

    public async Task<IActionResult> Edit(int productId)
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var response = await _productService.GetProductByIdAsync<ResponseDto>(productId, accessToken);
        if (response != null && response.IsSuccess)
        {
            ProductDto model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));

            return View(model);
        }
        return NotFound();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductDto model)
    {
        if (ModelState.IsValid)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.UpdateProductAsync<ResponseDto>(model, accessToken);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        return View(model);
    }

    [Authorize(Roles ="Admin")]
    public async Task<IActionResult> Delete(int productId)
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var response = await _productService.GetProductByIdAsync<ResponseDto>(productId, accessToken);
        if (response != null && response.IsSuccess)
        {
            ProductDto model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));

            return View(model);
        }
        return NotFound();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(ProductDto model)
    {
        if (ModelState.IsValid)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.DeleteProductAsync<ResponseDto>(model.ProductId, accessToken);
            if (response.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        return View(model);
    }
}
