namespace Domain.Abstractions
{
    public interface IIdentifiable
    {
        string Id { get; set; }
        string Identifier { get; }
    }
}
