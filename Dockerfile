FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and projects
COPY ["KinoDev.Payment.sln", "./"]
COPY ["src/KinoDev.Payment.WebApi/KinoDev.Payment.WebApi.csproj", "src/KinoDev.Payment.WebApi/"]
COPY ["src/KinoDev.Payment.Infrastructure/KinoDev.Payment.Infrastructure.csproj", "src/KinoDev.Payment.Infrastructure/"]
RUN dotnet restore "src/KinoDev.Payment.WebApi/KinoDev.Payment.WebApi.csproj"

# Copy full source and build
COPY . .
WORKDIR "/src/src/KinoDev.Payment.WebApi"
RUN dotnet build "KinoDev.Payment.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KinoDev.Payment.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KinoDev.Payment.WebApi.dll"]