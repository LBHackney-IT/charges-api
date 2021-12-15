FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

ENV DynamoDb_LocalMode='true'

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./ChargesApi.sln ./
COPY ./ChargesApi/ChargesApi.csproj ./ChargesApi/
COPY ./ChargesApi.Tests/ChargesApi.Tests.csproj ./ChargesApi.Tests/
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./ChargesApi/ChargesApi.csproj
RUN dotnet restore ./ChargesApi.Tests/ChargesApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out ChargesApi.Tests/ChargesApi.Tests.csproj

CMD dotnet test
