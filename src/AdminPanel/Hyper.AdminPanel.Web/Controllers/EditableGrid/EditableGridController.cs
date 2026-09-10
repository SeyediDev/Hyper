using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neo.Bpms.UI.MVC.Controllers.Public;

namespace Hyper.AdminPanel.Web.Controllers.EditableGrid;

/// <summary>
/// Controller for Editable Grid feature
/// </summary>
[Authorize]
public class EditableGridController : ControllerBaseMVC
{
    private readonly ILogger<EditableGridController> _logger;

    public EditableGridController(ILogger<EditableGridController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Main editable grid page
    /// </summary>
    public IActionResult Index(
        string? endpoint = null, 
        string? namespaceId = null, 
        string? entityId = null, 
        string? formId = null)
    {
        var user = GetUser();
        SetPagePackId("/EditableGrid/Index");
        
        // Build endpoint from form information if provided
        if (string.IsNullOrEmpty(endpoint) && !string.IsNullOrEmpty(namespaceId) && 
            !string.IsNullOrEmpty(entityId) && !string.IsNullOrEmpty(formId))
        {
            endpoint = $"{namespaceId}/{entityId}/{formId}";
        }
        
        ViewBag.Endpoint = endpoint ?? "sample";
        ViewBag.NamespaceId = namespaceId;
        ViewBag.EntityId = entityId;
        ViewBag.FormId = formId;
        
        _logger.LogInformation("Editable grid accessed by user {User} for endpoint {Endpoint} (Namespace: {NamespaceId}, Entity: {EntityId}, Form: {FormId})", 
            User.Identity?.Name, endpoint, namespaceId, entityId, formId);
        return View();
    }
    
    /// <summary>
    /// API endpoint for getting grid configuration
    /// Accepts endpoint as query parameter: api/grid/config?endpoint=...
    /// </summary>
    [HttpGet("api/grid/config")]
    public IActionResult GetConfig([FromQuery] string? endpoint)
    {
        try
        {
            _logger.LogInformation("Getting grid config for endpoint: {Endpoint}", endpoint);
            
            if (string.IsNullOrEmpty(endpoint))
            {
                return BadRequest(new { error = "Endpoint parameter is required" });
            }
            
            // TODO: Implement actual config retrieval from form metadata
            // For now, return a basic config structure
            object column1 = new { id = "id", name = "شناسه", type = "number", editable = false, width = 100 };
            object column2 = new { id = "name", name = "نام", type = "text", editable = true, required = true, width = 200 };
            object[] columns = new[] { column1, column2 };
            
            var config = new
            {
                columns = columns,
                rowActions = new
                {
                    hasDetails = true,
                    hasEdit = true,
                    hasDelete = false
                }
            };
            
            return Json(config);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting grid config for endpoint: {Endpoint}", endpoint);
            return StatusCode(500, new { error = "خطا در دریافت تنظیمات جدول" });
        }
    }
}

