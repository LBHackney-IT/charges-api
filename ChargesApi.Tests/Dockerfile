FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

ARG SONAR_TOKEN
ENV SONAR_TOKEN=$SONAR_TOKEN

ENV DynamoDb_LocalMode='true'

WORKDIR /app

# Enable SonarCloud
RUN apt-get update && apt-get install -y openjdk-11-jdk
RUN dotnet tool install --global dotnet-sonarscanner
ENV PATH="$PATH:/root/.dotnet/tools"

RUN dotnet sonarscanner begin /k:"LBHackney-IT_charges-api" /o:"lbhackney-it" /d:sonar.host.url=https://sonarcloud.io /d:sonar.login="${SONAR_TOKEN}"

# Copy csproj and restore as distinct layers
COPY ./ChargesApi.sln ./
COPY ./ChargesApi/ChargesApi.csproj ./ChargesApi/
COPY ./ChargesApi.Tests/ChargesApi.Tests.csproj ./ChargesApi.Tests/
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./ChargesApi/ChargesApi.csproj
RUN dotnet restore ./ChargesApi.Tests/ChargesApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out ChargesApi/ChargesApi.csproj
RUN dotnet build -c debug -o out ChargesApi.Tests/ChargesApi.Tests.csproj

CMD dotnet test

RUN dotnet sonarscanner end /d:sonar.login="${SONAR_TOKEN}"