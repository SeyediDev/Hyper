FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Set working directory inside the container
WORKDIR /app

# Copy published files into the container
COPY . .

# Expose the port your app runs on (typically 80 or 5000)
EXPOSE 80

WORKDIR /app
# Run the app
ENTRYPOINT ["dotnet", "Hyper.Bpms.Web.dll"]