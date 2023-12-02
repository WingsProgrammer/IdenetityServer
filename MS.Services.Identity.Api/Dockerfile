#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["MS.Services.Identity.Api/MS.Services.Identity.Api.csproj", "MS.Services.Identity.Api/"]
COPY ["MS.Services.Identitty.Domain/MS.Services.Identitty.Domain.csproj", "MS.Services.Identitty.Domain/"]
COPY ["MS.Services.Identity.Application/MS.Services.Identity.Application.csproj", "MS.Services.Identity.Application/"]
COPY ["KavehNegarSmsProvider/KavehNegarSmsProvider.csproj", "KavehNegarSmsProvider/"]
COPY ["MS.Services.Identity.Infrastructure/MS.Services.Identity.Infrastructure.csproj", "MS.Services.Identity.Infrastructure/"]
RUN dotnet restore "MS.Services.Identity.Api/MS.Services.Identity.Api.csproj"
COPY . .
WORKDIR "/src/MS.Services.Identity.Api"
RUN dotnet build "MS.Services.Identity.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MS.Services.Identity.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MS.Services.Identity.Api.dll"]