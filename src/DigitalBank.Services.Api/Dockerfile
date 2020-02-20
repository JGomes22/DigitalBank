FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/DigitalBank.Services.Api/DigitalBank.Services.Api.csproj", "src/DigitalBank.Services.Api/"]
COPY ["src/DigitalBank.Domain/DigitalBank.Domain.csproj", "src/DigitalBank.Domain/"]
COPY ["src/DigitalBank.Application.AppServices/DigitalBank.Application.AppServices.csproj", "src/DigitalBank.Application.AppServices/"]
COPY ["src/DigitalBank.Infra.CrossCutting/DigitalBank.Infra.CrossCutting.csproj", "src/DigitalBank.Infra.CrossCutting/"]
COPY ["src/DigitalBank.Infra.CrossCutting.Ioc/DigitalBank.Infra.CrossCutting.Ioc.csproj", "src/DigitalBank.Infra.CrossCutting.Ioc/"]
COPY ["src/DigitalBank.Infra.Data/DigitalBank.Infra.Data.csproj", "src/DigitalBank.Infra.Data/"]
RUN dotnet restore "src/DigitalBank.Services.Api/DigitalBank.Services.Api.csproj"
COPY . .
WORKDIR "/src/src/DigitalBank.Services.Api"
RUN dotnet build "DigitalBank.Services.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DigitalBank.Services.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DigitalBank.Services.Api.dll"]