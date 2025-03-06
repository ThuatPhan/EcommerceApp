using EcommerceApp.Application.DTO.Requests;
using EcommerceApp.Application.DTO.Responses;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Presentation.Responses;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApp.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        public readonly ILogger<CategoryController> _logger;
        private readonly ICategoryService _categoryService;

        public CategoryController(ILogger<CategoryController> logger, ICategoryService categoryService)
        {
            _logger = logger;
            _categoryService = categoryService;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CategoryDTO>>> AddCategory([FromForm] CategoryRequestDTO requestDTO)
        {
            try
            {
                var result = await _categoryService.AddCategoryAsync(requestDTO);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<CategoryDTO>.Failure(result.Message));
                }
                return CreatedAtAction(nameof(GetCategory), new { result.Data.Id }, result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when creating category");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryDTO>>> GetCategory([FromRoute] int id)
        {
            try
            {
                var result = await _categoryService.GetByIdAsync(id);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<CategoryDTO>.Failure(result.Message));
                }
                return Ok(ApiResponse<CategoryDTO>.Success(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving category");
            }
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<CategoryDTO>>>> GetAllCategories()
        {
            try
            {
                var result = await _categoryService.GetAllCategoriesAsync();
                return Ok(ApiResponse<List<CategoryDTO>>.Success(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving categories");
            }
        }

    }
}
