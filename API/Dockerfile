# build-env is the build stage. This will stage an area for us inside docker as we go about building our image. We build inside layers. So as make code(or co) changes, we create a new image. It's not going to start from scratch and re-download the SDK. It's going to start from a particular point and only create a new image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
# workdir /app specifies a workig directory. Inside our docker container, a directory is called /app. This is not our own file system. This is dockers file system inside the container. Any command we run, such as dotnet run will be happening inside this directory.
WORKDIR /app
EXPOSE 8080

# copy csproj and restore as distinct layers
# copy the file and put it in ./ which means it is in the app folder
COPY *.csproj ./
# It downloads and installs any packages that we need for this project based on what's inside the csproj file.
RUN dotnet restore

# copy everything else and build
COPY . ./
# To create a production build of our app and we'll give it a configuration of release and output directory of out which will be in /app/out
RUN dotnet publish -c Release -o out

# build a runtime image
# we don't need to use the full SDK once we get this point because we don't need any more dotnet run commands or dotnet SDK commands and SDK contains the dotnet compiler and everything. We just need something that's capable of running our published application, which is in the our directory. So we are going from a new image. aspnet is the runtime which is much smaller than SDK. 
FROM mcr.microsoft.com/dotnet/aspnet:7.0
# then we need to specify the work directory again as we've specified a new from location
WORKDIR /app 
# Copy everything that's been staged inside our build environments into our app folder. 
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "API.dll"]
