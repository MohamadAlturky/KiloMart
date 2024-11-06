using System.ComponentModel.DataAnnotations;

namespace KiloMart.Domain.Requests;

public class AddLanguageRequest
{
    [Required(ErrorMessage = "Language name is required.")]
    [StringLength(50, ErrorMessage = "Language name cannot exceed 50 characters.")]
    public string LanguageName { get; set; }

    [Required(ErrorMessage = "Language code is required.")]
    [StringLength(5, ErrorMessage = "Language code cannot exceed 5 characters.")]
    public string LanguageCode { get; set; }

    public (bool IsValid, List<string> Errors) Validate()
    {
        var errors = new List<string>();
        var context = new ValidationContext(this);
        var results = new List<ValidationResult>();

        if (!Validator.TryValidateObject(this, context, results, true))
        {
            foreach (var validationResult in results)
            {
                errors.Add(validationResult.ErrorMessage);
            }
        }

        return (errors.Count == 0, errors);
    }
}
