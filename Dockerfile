# Use .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY BackEndProject/BackEndProject.csproj BackEndProject/
RUN dotnet restore BackEndProject/BackEndProject.csproj

# Copy the rest of the files
COPY . .

# Publish the app
WORKDIR /src/BackEndProject
RUN dotnet publish -c Release -o /app/out

# Use runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .

# Expose port and start
EXPOSE 5000
ENTRYPOINT ["dotnet", "BackEndProject.dll"]
