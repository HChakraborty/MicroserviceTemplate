namespace ServiceName.IntegrationTests.Helpers;

public static class ContainerImages
{
    public const string Sql =
        "mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04";

    public const string Redis =
        "redis:7.2-alpine";

    public const string RabbitMq =
        "rabbitmq:3.13-management";
}
