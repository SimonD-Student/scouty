FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Scouty.Backend/Scouty.Backend.csproj", "Scouty.Backend/"]
COPY ["Scouty.Shared/Scouty.Shared.csproj", "Scouty.Shared/"]
RUN dotnet restore "Scouty.Backend/Scouty.Backend.csproj"
COPY . .
WORKDIR "/src/Scouty.Backend"
RUN dotnet build "Scouty.Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Scouty.Backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Scouty.Backend.dll"]