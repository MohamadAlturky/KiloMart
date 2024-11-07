namespace KiloMart.Domain.Products.Add.Models;

public class ProductLocalizedDto
{
    //write the private feild Language , ProductId , MeasurementUnit ,Description ,Name
    private byte _language;
    private int _productId;
    private string _measurementUnit = string.Empty;
    private string _description = string.Empty;
    private string _name = string.Empty;
}