#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Idasen.RESTAPI.MicroService.Desk/Idasen.RESTAPI.MicroService.Desk.csproj", "Idasen.RESTAPI.MicroService.Desk/"]
COPY ["Idasen.RESTAPI.MicroService.Shared/Idasen.RESTAPI.MicroService.Shared.csproj", "Idasen.RESTAPI.MicroService.Shared/"]
RUN dotnet restore "Idasen.RESTAPI.MicroService.Desk/Idasen.RESTAPI.MicroService.Desk.csproj"
COPY . .
WORKDIR "/src/Idasen.RESTAPI.MicroService.Desk"
RUN dotnet build "Idasen.RESTAPI.MicroService.Desk.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Idasen.RESTAPI.MicroService.Desk.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Idasen.RESTAPI.MicroService.Desk.dll"]