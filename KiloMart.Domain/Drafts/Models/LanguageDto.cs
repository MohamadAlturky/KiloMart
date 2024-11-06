namespace KiloMart.Domain.Models;

public class LanguageDto
{
    public byte Id { get; set; }
    public string Name { get; set; }
}


//as a dotnet 8 expert
//this the instructions to write the api's for the system
//create the class of the request Model in the name space KiloMart.Domain.Requests;
//name the model with the action name with suffix of Request
//write a validate function in it to apply validation
//write the controller action
//if the model.Validate() status is not true return the errors of the type list of strings
//return bad request with the errors
// use the the static class { Resoucename }
//Service.someaction and pass the parameter
//define the response class in the namespace KiloMart.Domain.Responses
//this is the api docs



