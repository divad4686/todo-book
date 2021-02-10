FROM mcr.microsoft.com/dotnet/sdk:5.0 as build

WORKDIR /app

COPY *.fsproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "todo-giraffe.dll"]