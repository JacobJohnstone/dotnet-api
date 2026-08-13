FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY ["DotnetApi.csproj", "./"]
RUN dotnet restore "DotnetApi.csproj"

COPY . .

RUN dotnet publish "DotnetApi.csproj" \
	-c Release \
	-o /app/publish \
	/p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "DotnetApi.dll"]