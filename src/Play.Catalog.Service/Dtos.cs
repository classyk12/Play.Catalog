using System.ComponentModel.DataAnnotations;

namespace Play.Catalog.Service;

public record ItemDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    DateTimeOffset CreatedDate
);

public record CreateItemDto(
    [property: Required]
    string Name,

    [property:Required]
    string Description,

    [property: Range(0, 9999999)]
    decimal Price
);

public record UpdateItemDto(
    [property: Required]
    string Name,

    [property:Required]
    string Description,

    [property: Range(0, 9999999)]
    decimal Price
);