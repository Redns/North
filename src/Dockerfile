#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 12121

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["North/North.csproj", "North/"]
COPY ["North.Core/North.Core.csproj", "North.Core/"]
COPY ["North.PluginBase/North.PluginBase.csproj", "North.PluginBase/"]
RUN dotnet restore "North/North.csproj"
COPY . .
WORKDIR "/src/North"
RUN dotnet build "North.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "North.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "North.dll"]