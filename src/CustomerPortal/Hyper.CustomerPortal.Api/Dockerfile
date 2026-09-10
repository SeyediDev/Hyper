# Base stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
ARG BUILD_CONTEXT=Hyper/Backend

# Copy project files
COPY ["src/CustomerPortal/Hyper.CustomerPortal.Api/Hyper.CustomerPortal.Api.csproj", "src/CustomerPortal/Hyper.CustomerPortal.Api/"]
COPY ["src/CustomerPortal/Hyper.CustomerPortal.Application/Hyper.CustomerPortal.Application.csproj", "src/CustomerPortal/Hyper.CustomerPortal.Application/"]
COPY ["src/Core/Hyper.Domain/Hyper.Domain.csproj", "src/Core/Hyper.Domain/"]
COPY ["src/Core/Hyper.Infrastructure/Hyper.Infrastructure.csproj", "src/Core/Hyper.Infrastructure/"]
COPY ["src/Core/Hyper.Application/Hyper.Application.csproj", "src/Core/Hyper.Application/"]
COPY ["../../Neo/src/Neo.Domain/Neo.Domain.csproj", "../../Neo/src/Neo.Domain/"]
COPY ["../../Neo/src/Neo.Application/Neo.Application.csproj", "../../Neo/src/Neo.Application/"]
COPY ["../../Neo/src/Neo.Infrastructure/Neo.Infrastructure.csproj", "../../Neo/src/Neo.Infrastructure/"]
COPY ["../../Neo/src/Neo.Endpoint/Neo.Endpoint.csproj", "../../Neo/src/Neo.Endpoint/"]
COPY ["../../Neo/src/Neo.Common/Neo.Common.csproj", "../../Neo/src/Neo.Common/"]

# Restore dependencies
RUN dotnet restore "src/CustomerPortal/Hyper.CustomerPortal.Api/Hyper.CustomerPortal.Api.csproj"

# Copy all source files
COPY . .

# Build
WORKDIR "/src/src/CustomerPortal/Hyper.CustomerPortal.Api"
RUN dotnet build "Hyper.CustomerPortal.Api.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "Hyper.CustomerPortal.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hyper.CustomerPortal.Api.dll"]

