FROM mcr.microsoft.com/dotnet/sdk:3.1-buster as build
WORKDIR /driver
COPY /driver/chromedriver .
WORKDIR /nuget
COPY /NuGetPackages/* .
WORKDIR /screenshots

WORKDIR /browser
copy /browser/google-chrome-stable_current_amd64.deb .
#RUN 	wget https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb
RUN ls -l
RUN apt-get update
RUN apt install -y ./google-chrome-stable_current_amd64.deb

WORKDIR /source
COPY . .
COPY specDrillConfig.json.Docker specDrillConfig.json


CMD dotnet test --filter "ShouldHaveWikipediaAmongResultsOnGoogleSearch"
#RUN ls -l /screenshots

#ENTRYPOINT dotnet test --filter "ShouldHaveWikipediaAmongResultsOnGoogleSearch" & ls -l /screenshots