internal class Config
{
    public string DeploymentOrModelId { get; } = "gpt-4o";
    public string Endpoint { get; } = "https://myfirstapp123456.openai.azure.com/";
    public string ApiKey { get; } = "AI8tkaMKv1XvUeNYXNUoasQqEzyEs3sU1KWJt3rilRUC1p1dnL9zJQQJ99BFACYeBjFXJ3w3AAAAACOGnHDV";
    public string BingMapKey { get; } = Environment.GetEnvironmentVariable("BINGMAP",
        EnvironmentVariableTarget.Machine) ?? string.Empty;

}