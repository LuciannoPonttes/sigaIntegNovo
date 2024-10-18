FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SigaDocIntegracao.Web/SigaDocIntegracao.Web.csproj", "SigaDocIntegracao.Web/"]
COPY ["GerenciaEmail/GerenciaEmail.csproj", "GerenciaEmail/"]
COPY ["CargaSigaDoc/CargaSigaDoc.csproj", "CargaSigaDoc/"]
RUN dotnet restore "SigaDocIntegracao.Web/SigaDocIntegracao.Web.csproj"
COPY . .
WORKDIR "/src/SigaDocIntegracao.Web"
RUN dotnet build "SigaDocIntegracao.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SigaDocIntegracao.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SigaDocIntegracao.Web.dll"]