FROM mcr.microsoft.com/dotnet/sdk:6.0

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

RUN apt-get update \
     && apt-get install curl gnupg -yq \
     && curl -sL https://deb.nodesource.com/setup_14.x | bash \
     && apt-get install nodejs -yq

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./DocumentsApi.sln ./
COPY ./DocumentsApi/DocumentsApi.csproj ./DocumentsApi/
COPY ./DocumentsApi.Tests/DocumentsApi.Tests.csproj ./DocumentsApi.Tests/

RUN dotnet restore ./DocumentsApi/DocumentsApi.csproj
RUN dotnet restore ./DocumentsApi.Tests/DocumentsApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out DocumentsApi.Tests/DocumentsApi.Tests.csproj
RUN cd ./DocumentsApi/V1/Node && npm ci

CMD dotnet test
