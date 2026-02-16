using Play.Catalog.Service.Repositories;

namespace Play.Catalog.Service.Endpoints;

public static class ItemEndpoints
{
    public static WebApplication MapItemEndpoints(this WebApplication app)
    {
        var itemGroup = app.MapGroup("/api/items")
        .WithOpenApi()
        .WithTags("Items");

        itemGroup.MapGet("/", async (IItemsRepository repository) => Results.Ok(await repository.GetItemsAsync()))
           .WithName("GetItemsAsync");

        itemGroup.MapGet("{id:guid}", async (Guid id, IItemsRepository repository) =>
        {
            var item = await repository.GetItemAsync(id);
            return item is not null ? Results.Ok(item) : Results.NotFound();
        })
        .WithName("GetItemByIdAsync");

        itemGroup.MapPost("/", async (CreateItemDto dto, IItemsRepository repository) =>
        {
            var errors = ModelValidator.ValidateDto(dto);
            if (errors.Count > 0)
                return Results.BadRequest(errors);

            var item = new ItemDto(Guid.NewGuid(), dto.Name, dto.Description, dto.Price, DateTimeOffset.UtcNow);
            await repository.CreateItemAsync(item.AsEntity());
            return Results.Created($"/items/{item.Id}", item);
        })
        .WithName("CreateItemAsync");

        itemGroup.MapPut("/{id:guid}", async (Guid id, UpdateItemDto dto, IItemsRepository repository) =>
        {
            var errors = ModelValidator.ValidateDto(dto);
            if (errors.Count > 0)
            {
                return Results.BadRequest(errors);
            }

            var existingItem = await repository.GetItemAsync(id);
            if (existingItem is null)
            {
                return Results.NotFound();
            }

            var updatedItem = new ItemDto(id, dto.Name, dto.Description, dto.Price, existingItem.CreatedDate);
            await repository.UpdateItemAsync(updatedItem.AsEntity());
            return Results.NoContent();
        })
        .WithName("UpdateItemAsync");

        itemGroup.MapDelete("/{id:guid}", async (Guid id, IItemsRepository repository) =>
        {
            var existingItem = await repository.GetItemAsync(id);
            if (existingItem is null)
            {
                return Results.NotFound();
            }

            await repository.DeleteItemAsync(id);
            return Results.NoContent();
        })
        .WithName("DeleteItemAsync");

        return app;
    }
}
