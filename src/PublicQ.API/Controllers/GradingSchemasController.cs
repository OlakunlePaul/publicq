using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PublicQ.API.Helpers;
using PublicQ.Application.Interfaces;
using PublicQ.Application.Models.Academic;
using PublicQ.Shared;

namespace PublicQ.API.Controllers;

/// <summary>
/// API controller for managing flexible grading schemas.
/// </summary>
[ApiController]
[Authorize(Constants.ManagersPolicy)]
[Route($"{Constants.ControllerRoutePrefix}/grading-schemas")]
public class GradingSchemasController(IAcademicStructureService academicService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetGradingSchemasAsync(CancellationToken cancellationToken)
    {
        var response = await academicService.GetGradingSchemasAsync(cancellationToken);
        return response.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> CreateGradingSchemaAsync([FromBody] GradingSchemaCreateDto request, CancellationToken cancellationToken)
    {
        var response = await academicService.CreateGradingSchemaAsync(request, cancellationToken);
        return response.ToActionResult();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateGradingSchemaAsync(Guid id, [FromBody] GradingSchemaCreateDto request, CancellationToken cancellationToken)
    {
        var response = await academicService.UpdateGradingSchemaAsync(id, request, cancellationToken);
        return response.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteGradingSchemaAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await academicService.DeleteGradingSchemaAsync(id, cancellationToken);
        return response.ToActionResult();
    }
}
