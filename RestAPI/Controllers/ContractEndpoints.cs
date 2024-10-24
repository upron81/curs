using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TelecomWeb.Models;

namespace RestAPI.Controllers;


public static class ContractEndpoints
{
    public static void MapContractEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Contract").WithTags(nameof(Contract));

        group.MapGet("/", async (TelecomDbContext db) =>
        {
            return await db.Contracts.ToListAsync();
        })
        .WithName("GetAllContracts")
        .WithOpenApi();

        group.MapGet("/{contractid}", async Task<Results<Ok<Contract>, NotFound>> (int contractid, TelecomDbContext db) =>
        {
            return await db.Contracts.AsNoTracking()
                .FirstOrDefaultAsync(model => model.ContractId == contractid)
                is Contract model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetContractById")
        .WithOpenApi();

        group.MapPut("/{contractid}", async Task<Results<Ok, NotFound>> (int contractid, Contract contract, TelecomDbContext db) =>
        {
            var affected = await db.Contracts
                .Where(model => model.ContractId == contractid)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.SubscriberId, contract.SubscriberId)
                  .SetProperty(m => m.TariffPlanId, contract.TariffPlanId)
                  .SetProperty(m => m.ContractDate, contract.ContractDate)
                  .SetProperty(m => m.ContractEndDate, contract.ContractEndDate)
                  .SetProperty(m => m.PhoneNumber, contract.PhoneNumber)
                  .SetProperty(m => m.StaffId, contract.StaffId)
                  );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateContract")
        .WithOpenApi();

        group.MapPost("/", async (Contract contract, TelecomDbContext db) =>
        {
            db.Contracts.Add(contract);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Contract/{contract.ContractId}", contract);
        })
        .WithName("CreateContract")
        .WithOpenApi();

        group.MapDelete("/{contractid}", async Task<Results<Ok, NotFound>> (int contractid, TelecomDbContext db) =>
        {
            var affected = await db.Contracts
                .Where(model => model.ContractId == contractid)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteContract")
        .WithOpenApi();
    }
}
