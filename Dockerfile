# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY Test/ Test/
COPY MessageRouter.Core/ MessageRouter.Core/
COPY MessageRouter.Providers.RabbitMQ/ MessageRouter.Providers.RabbitMQ/
COPY common.props common.props
RUN dotnet restore Test/MessageRouter.Test.Console/MessageRouter.Test.Console.csproj

WORKDIR /source/Test/MessageRouter.Test.Console

FROM build AS publish
RUN dotnet publish -c release -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MessageRouter.Test.Console.dll"]