# Use .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY TMCC.csproj ./
RUN dotnet restore "TMCC.csproj"

# Copy the rest of the files
COPY . .

# Publish the app
RUN dotnet publish "TMCC.csproj" -c Release -o /app/publish

# Use runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published files from build stage
COPY --from=build /app/publish .

# Expose port (Render uses PORT environment variable)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Start the application
ENTRYPOINT ["dotnet", "TMCC.dll"]