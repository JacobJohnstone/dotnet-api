# Use the .NET 9 SDK image for the build stage.
# The SDK image contains the .NET SDK and all tools required to
# restore NuGet packages and compile/publish the application.
#
# "AS build" gives this stage a name so it can be referenced later
# when copying the published application into the runtime image.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set the working directory inside the container.
# All subsequent commands in this stage will run from /src unless
# another directory is explicitly specified.
WORKDIR /src

# Copy only the project file first.
# This allows Docker to cache the dependency-restore layer.
# If application source code changes but the .csproj file does not,
# Docker can reuse the previously completed "dotnet restore" layer.
COPY ["DotnetApi.csproj", "./"]

# Restore the NuGet dependencies defined in the project file.
# This downloads all required packages before the application is built.
RUN dotnet restore "DotnetApi.csproj"

# Copy the remaining application source code and project files
# into the /src directory.
COPY . .

# Publish the application in Release configuration.
#
# -c Release
#   Builds the application using the Release configuration.
#
# -o /app/publish
#   Places the published application files in /app/publish.
#
# /p:UseAppHost=false
#   Prevents creation of a platform-specific executable (app host).
#   The application will instead be started using:
#   dotnet DotnetApi.dll
#
# The published output is copied into the smaller runtime image
# in the next stage.
RUN dotnet publish "DotnetApi.csproj" \
	-c Release \
	-o /app/publish \
	/p:UseAppHost=false


# Start a new stage using the ASP.NET Core runtime image.
# Unlike the SDK image, this image does not contain the full .NET SDK,
# which makes the final container smaller and reduces the number of
# unnecessary tools included in the production image.
#
# Only the runtime required to run an ASP.NET Core application is included.
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

# Set the working directory for the application at runtime.
WORKDIR /app

# Copy only the published application from the build stage.
#
# "--from=build" references the stage defined earlier as "build".
# This keeps the source code, NuGet cache, SDK, and build tools out
# of the final runtime image.
COPY --from=build /app/publish .

# listen on port 8080
EXPOSE 8080

# Define the application that Docker should run when the container starts.
#
# This starts the .NET runtime and executes the published application DLL.
ENTRYPOINT ["dotnet", "DotnetApi.dll"]