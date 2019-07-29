
@echo off

dotnet clean
dotnet restore
dotnet build

DEL /F/Q/S "D:\Publishes\lin-cms-dotnetcore" > NUL && RMDIR /Q/S "D:\Publishes\lin-cms-dotnetcore"

::dotnet publish -c Release -r win-x64   --self-contained false -o "D:\Publishes\lin-cms-dotnetcore\win-x64"
::dotnet publish -c Release -r win-x86   --self-contained false -o "D:\Publishes\lin-cms-dotnetcore\win-x86"
::dotnet publish -c Release -r osx-x64   --self-contained false -o "D:\Publishes\lin-cms-dotnetcore\osx-x64"
dotnet publish -c Release -r linux-x64 --self-contained false -o "D:\Publishes\lin-cms-dotnetcore\linux-x64"