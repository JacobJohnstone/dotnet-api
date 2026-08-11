FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["DotnetApi.csproj", "./"]
RUN dotnet restore "DotnetApi.csproj"

COPY . .
WORKDIR "/src/DotnetApi"

RUN dotnet publish "DotnetApi.csproj" \
	-c Release \
	-o /app/publish \
	--no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "DotnetApi.dll"]