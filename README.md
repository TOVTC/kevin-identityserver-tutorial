# Identity server 4
* Install IdentityServer4 templates
```
dotnet new -i identityserver4.templates
```
* You can use the following command to create a template of Identity Server 4 with In-Memory Stores and Test Users
```
dotnet new is4inmem
```
* This tutorial does not use this command and starts from scratch instead by using the following command to create an empty web project
```
dotnet new web
```
* Using the instructions from the official Identity Server documentation, create a new solution and add the project file to it so it can be opened by Visual Studio
```
dotnet new sln -n <name-of-solution>
dotnet sln add <path-to-solution>
```
* Add IdentityServer4 as a package to the application using NuGet package manager or the command line

## Configure Identity Server
* In Startup.cs
    * Add IdentityServer to the ConfigureServices method and specify where it can find its resources
    * Add IdentityServer to the pipeline in the Configure method
* At this point, the app can be run and the discovery document should be available for viewing at /.well-known/openid-configuration