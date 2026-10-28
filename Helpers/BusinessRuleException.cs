namespace ECommerceManagement.Helpers;

public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message) { }
}