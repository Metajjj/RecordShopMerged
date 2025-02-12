
using System.ComponentModel.DataAnnotations;

public class ValidAttributes : ValidationAttribute
{
    [DataType(DataType.Date)]
    public static DateOnly Date { get; set; } = new DateOnly();
}

