dotnet build;
dotnet pack -p:PackageID=hydrolizer
dotnet tool install --global hydrolizer --add-source ./nupkg --ignore-failed-sources

