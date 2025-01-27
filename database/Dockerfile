FROM mcr.microsoft.com/dotnet/sdk:8.0

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

WORKDIR /app

# Install dotnet-ef globally
ENV PATH $PATH:/root/.dotnet/tools
RUN dotnet tool install --global dotnet-ef --version 8.0.0

RUN apt-get update
RUN apt-get install sudo
RUN sudo apt-get install postgresql -y

RUN apt-get update
RUN sudo apt-get install net-tools

# Copy csproj and restore as distinct layers
COPY ./DocumentsApi/*.csproj ./
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config
RUN dotnet restore

# Copy everything else and build
COPY ./DocumentsApi ./
COPY ./.editorconfig ./
RUN dotnet build

CMD ["dotnet", "ef", "database", "update", "--no-build"]
