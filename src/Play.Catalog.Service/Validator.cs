using System.ComponentModel.DataAnnotations;
namespace Play.Catalog.Service
{
    public static class ModelValidator
    {
        public static List<string> ValidateDto(object dto)
        {
            var errors = new List<string>();
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(dto, context, results, validateAllProperties: true))
            {
                foreach (var result in results)
                {
                    errors.Add(result.ErrorMessage ?? result.ToString());
                }
            }

            return errors;
        }
    }
}