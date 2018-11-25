FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["AIMS.APIs/AIMS.APIs.csproj", "AIMS.APIs/"]
COPY ["AIMS.Models/AIMS.Models.csproj", "AIMS.Models/"]
COPY ["AIMS.DAL/AIMS.DAL.csproj", "AIMS.DAL/"]
COPY ["AIMS.Services/AIMS.Services.csproj", "AIMS.Services/"]
RUN dotnet restore "AIMS.APIs/AIMS.APIs.csproj"
COPY . .
WORKDIR "/src/AIMS.APIs"
RUN dotnet build "AIMS.APIs.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AIMS.APIs.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AIMS.APIs.dll"]