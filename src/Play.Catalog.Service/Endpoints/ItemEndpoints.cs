namespace Play.Catalog.Service.Endpoints;

public static class ItemEndpoints
{
    public static WebApplication MapItemEndpoints(this WebApplication app)
    {
        var itemGroup = app.MapGroup("/api/items")
        .WithOpenApi()
        .WithTags("Items");

        var items = new List<ItemDto>
        {
            new(Guid.NewGuid(), "Potion", "Restores a small amount of HP", 9, DateTimeOffset.UtcNow),
            new(Guid.NewGuid(), "Iron Sword", "A basic sword made of iron", 20, DateTimeOffset.UtcNow),
            new(Guid.NewGuid(), "Bronze Shield", "A basic shield made of bronze", 15, DateTimeOffset.UtcNow)
        };

        itemGroup.MapGet("/", () => Results.Ok(items))
           .WithName("GetItems");

        itemGroup.MapGet("{id:guid}", (Guid id) =>
        {
            var item = items.FirstOrDefault(i => i.Id == id);
            return item is not null ? Results.Ok(item) : Results.NotFound();
        })
        .WithName("GetItemById");

        itemGroup.MapPost("/", (CreateItemDto dto) =>
        {
            var errors = ModelValidator.ValidateDto(dto);
            if (errors.Count > 0)
                return Results.BadRequest(errors);

            var item = new ItemDto(Guid.NewGuid(), dto.Name, dto.Description, dto.Price, DateTimeOffset.UtcNow);
            items.Add(item);
            return Results.Created($"/items/{item.Id}", item);
        })
        .WithName("CreateItem");

        itemGroup.MapPut("/{id:guid}", (Guid id, UpdateItemDto dto) =>
        {
            var errors = ModelValidator.ValidateDto(dto);
            if (errors.Count > 0)
            {
                return Results.BadRequest(errors);
            }

            var idx = items.FindIndex(i => i.Id == id);
            if (idx < 0) return Results.NotFound();
            var updated = new ItemDto(id, dto.Name, dto.Description, dto.Price, items[idx].CreatedDate);
            items[idx] = updated;
            return Results.NoContent();
        })
        .WithName("UpdateItem");

        itemGroup.MapDelete("/{id:guid}", (Guid id) =>
        {
            var removed = items.RemoveAll(i => i.Id == id) > 0;
            return removed ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteItem");

        return app;
    }
}
