#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["AIMS.APIs/AIMS.APIs.csproj", "AIMS.APIs/"]
COPY ["AIMS.Models/AIMS.Models.csproj", "AIMS.Models/"]
COPY ["AIMS.DAL/AIMS.DAL.csproj", "AIMS.DAL/"]
COPY ["AIMS.Services/AIMS.Services.csproj", "AIMS.Services/"]
COPY ["AIMS.IATILib/AIMS.IATILib.csproj", "AIMS.IATILib/"]
RUN dotnet restore "AIMS.APIs/AIMS.APIs.csproj"
COPY . .
WORKDIR "/src/AIMS.APIs"
RUN dotnet build "AIMS.APIs.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AIMS.APIs.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AIMS.APIs.dll"]