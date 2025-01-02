
# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR FormsProyect

EXPOSE 80
EXPOSE 8080

COPY ./*.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /FormsProyect
COPY --from=build /FormsProyect/out .
ENTRYPOINT ["dotnet", "FormsProyect.dll"]