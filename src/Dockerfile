FROM mcr.microsoft.com/dotnet/aspnet:8.0.0-jammy-chiseled-amd64

# Copy files from other build jobs
COPY publish .

USER $APP_UID
EXPOSE 8080

ENTRYPOINT ["dotnet", "Nager.PublicSuffixService.dll"]