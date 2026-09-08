using Hyper.Application.Features.Surveys.Commands;
using Hyper.Application.Features.Surveys.Queries;
using Hyper.Domain.Entities.Promotions.Surveys.Enums;

namespace Hyper.CustomerPortal.Api.Controllers;

[ApiController]
[AppRoute("Hyper", "surveys")]
[Tags("surveys")]
public class SurveyController(IMediator mediator) : ControllerBase
{

    /// <summary>
    /// دریافت لیست نظرسنجی‌ها
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<SurveyDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSurveys([FromQuery] int? tenantId, [FromQuery] SurveyType? surveyType, [FromQuery] bool? isActive)
    {
        var query = new GetSurveysQuery
        {
            TenantId = tenantId,
            SurveyType = surveyType,
            IsActive = isActive
        };
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// دریافت جزئیات یک نظرسنجی
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SurveyDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSurveyById(int id, [FromQuery] int? customerTenantId)
    {
        var query = new GetSurveyByIdQuery
        {
            Id = id,
            CustomerTenantId = customerTenantId
        };
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// دریافت نظرسنجی‌های فعال برای مشتری
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(List<ActiveSurveyDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveSurveys([FromQuery] int tenantId, [FromQuery] int? customerTenantId, [FromQuery] SurveyType? surveyType)
    {
        var query = new GetActiveSurveysQuery
        {
            TenantId = tenantId,
            CustomerTenantId = customerTenantId,
            SurveyType = surveyType
        };
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// ایجاد نظرسنجی جدید
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSurvey([FromBody] CreateSurveyCommand command)
    {
        var result = await mediator.Send(command);
        return CreatedAtAction(nameof(GetSurveyById), new { id = result }, result);
    }

    /// <summary>
    /// ویرایش نظرسنجی
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSurvey(int id, [FromBody] UpdateSurveyCommand command)
    {
        if (id != command.Id)
            return BadRequest("شناسه نظرسنجی با داده ارسالی مطابقت ندارد");

        await mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// حذف نظرسنجی
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSurvey(int id)
    {
        var command = new DeleteSurveyCommand { Id = id };
        await mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// شرکت در نظرسنجی (رأی دادن)
    /// </summary>
    [HttpPost("{surveyId}/participate")]
    [ProducesResponseType(typeof(ParticipateSurveyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Participate(int surveyId, [FromBody] ParticipateSurveyRequest request)
    {
        var command = new ParticipateSurveyCommand
        {
            SurveyId = surveyId,
            CustomerTenantId = request.CustomerTenantId,
            SelectedItemId = request.SelectedItemId,
            Comment = request.Comment
        };
        var result = await mediator.Send(command);
        return Ok(result);
    }
}

public record ParticipateSurveyRequest
{
    public int CustomerTenantId { get; set; }
    public int SelectedItemId { get; set; }
    public string? Comment { get; set; }
}



