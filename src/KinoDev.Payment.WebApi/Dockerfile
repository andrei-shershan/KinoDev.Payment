FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/KinoDev.Payment.WebApi/KinoDev.Payment.WebApi.csproj", "src/KinoDev.Payment.WebApi/"]
RUN dotnet restore "src/KinoDev.Payment.WebApi/KinoDev.Payment.WebApi.csproj"
COPY . .
WORKDIR "/src/src/KinoDev.Payment.WebApi"
RUN dotnet build "KinoDev.Payment.WebApi.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "KinoDev.Payment.WebApi.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KinoDev.Payment.WebApi.dll"]
