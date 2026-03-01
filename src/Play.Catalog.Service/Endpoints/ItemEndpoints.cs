
using MassTransit;
using Play.Catalog.Service.Entities;
using Play.Common;
using static Play.Catalog.Contract.Contracts;

namespace Play.Catalog.Service.Endpoints;

public static class ItemEndpoints
{
    public static WebApplication MapItemEndpoints(this WebApplication app, IPublishEndpoint publishEndpoint)
    {
        var itemGroup = app.MapGroup("/api/items")
        .WithOpenApi()
        .WithTags("Items");

        itemGroup.MapGet("/", async (IRepository<Item> repository) =>
        {
            return Results.Ok((await repository.GetAllAsync()).Select(item => item.AsDto()));
        })
        .WithName("GetItemsAsync");

        itemGroup.MapGet("{id:guid}", async (Guid id, IRepository<Item> repository) =>
        {
            var item = await repository.GetAsync(id);
            return item is not null ? Results.Ok(item.AsDto()) : Results.NotFound();
        })
        .WithName("GetItemByIdAsync");

        itemGroup.MapPost("/", async (CreateItemDto dto, IRepository<Item> repository) =>
        {
            var errors = ModelValidator.ValidateDto(dto);
            if (errors.Count > 0)
                return Results.BadRequest(errors);

            var item = new ItemDto(Guid.NewGuid(), dto.Name, dto.Description, dto.Price, DateTimeOffset.UtcNow);
            await repository.CreateAsync(item.AsEntity());
            await publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));
            return Results.Created($"/items/{item.Id}", item);
        })
        .WithName("CreateItemAsync");

        itemGroup.MapPut("/{id:guid}", async (Guid id, UpdateItemDto dto, IRepository<Item> repository) =>
        {
            var errors = ModelValidator.ValidateDto(dto);
            if (errors.Count > 0)
            {
                return Results.BadRequest(errors);
            }

            var existingItem = await repository.GetAsync(id);
            if (existingItem is null)
            {
                return Results.NotFound();
            }

            var updatedItem = new ItemDto(id, dto.Name, dto.Description, dto.Price, existingItem.CreatedDate);
            await repository.UpdateAsync(updatedItem.AsEntity());
            await publishEndpoint.Publish(new CatalogItemUpdated(updatedItem.Id, updatedItem.Name, updatedItem.Description));
            return Results.NoContent();
        })
        .WithName("UpdateItemAsync");

        itemGroup.MapDelete("/{id:guid}", async (Guid id, IRepository<Item> repository) =>
        {
            var existingItem = await repository.GetAsync(id);
            if (existingItem is null)
            {
                return Results.NotFound();
            }

            await repository.DeleteAsync(id);
            await publishEndpoint.Publish(new CatalogItemDeleted(id));
            return Results.NoContent();
        })
        .WithName("DeleteItemAsync");

        return app;
    }
}
