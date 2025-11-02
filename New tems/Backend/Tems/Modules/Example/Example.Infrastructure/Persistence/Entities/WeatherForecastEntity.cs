using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Example.Infrastructure.Persistence.Entities;

public class WeatherForecastEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("date")]
    public DateTime Date { get; set; }

    [BsonElement("temperatureC")]
    public int TemperatureC { get; set; }

    [BsonElement("summary")]
    public string? Summary { get; set; }

    [BsonElement("location")]
    public string Location { get; set; } = string.Empty;

    [BsonElement("lastUpdated")]
    public DateTime LastUpdated { get; set; }
}
