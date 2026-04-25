namespace Tems.Host.Configuration;

public sealed class AiSupportOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.deepseek.com";
    public string Model { get; set; } = "deepseek-chat";
    public string SystemPrompt { get; set; } =
        "You are TEMS AI Agent, focusing on providing technical guidance and stuff.\n" +
        "Do not answer questions that do not relate to equipment management or technical support.\n" +
        "You can respond in markdown and the UI will render it automatically.";
    public double Temperature { get; set; } = 0.2;
    public int MaxTokens { get; set; } = 1200;
    public int TimeoutMinutes { get; set; } = 30;
    public string TicketSummarySystemPrompt { get; set; } =
        "You are TEMS AI Agent.\n" +
        "Write a concise AI summary for an IT support ticket.\n" +
        "Use markdown.\n" +
        "Keep the answer to 2-3 short paragraphs maximum.\n" +
        "Focus only on the technical problem, likely cause, impact, and next steps.\n" +
        "Do not include any content unrelated to equipment management or technical support.\n" +
        "Do not add a title.";
    public double TicketSummaryTemperature { get; set; } = 0.2;
    public int TicketSummaryMaxTokens { get; set; } = 450;
}
