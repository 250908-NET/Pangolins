namespace Pangolivia.API.Options;

public class OpenAiOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-4.1-mini";
    public double Temperature { get; set; } = 0.7;
}
