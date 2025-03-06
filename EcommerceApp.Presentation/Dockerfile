# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["EcommerceApp.Presentation/EcommerceApp.Presentation.csproj", "EcommerceApp.Presentation/"]
COPY ["EcommerceApp.Application/EcommerceApp.Application.csproj", "EcommerceApp.Application/"]
COPY ["EcommerceApp.Infrastructure/EcommerceApp.Infrastructure.csproj", "EcommerceApp.Infrastructure/"]
COPY ["EcommerceApp.Domain/EcommerceApp.Domain.csproj", "EcommerceApp.Domain/"]
RUN dotnet restore "./EcommerceApp.Presentation/EcommerceApp.Presentation.csproj"
COPY . .
WORKDIR "/src/EcommerceApp.Presentation"
RUN dotnet build "./EcommerceApp.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EcommerceApp.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EcommerceApp.Presentation.dll"]