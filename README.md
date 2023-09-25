# Identity Server 4
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
* Using the instructions from the official Identity Server documentation, create a new solution and add the project file (the .csproj file) to it so it can be opened by Visual Studio
```
dotnet new sln -n <name-of-solution>
dotnet sln add <path-to-solution>
```
* Add IdentityServer4 as a package to the application using NuGet package manager or the command line

## Configure Identity Server
* In Startup.cs
    * Add IdentityServer to the ConfigureServices method and specify where it can find its resources (initialize with empty lists until Config file is added)
    * Add IdentityServer to the pipeline in the Configure method
* At this point, the app can be run and the discovery document should be available for viewing at /.well-known/openid-configuration
* This tutorial uses Config.cs as a catchall for test users, clients, and resources for configuring Startup.cs
    * Once these resources are added to Startup.cs, these scopes and claims will show up in the discovery document
* The way the app works, is it requests a token from IdentityServer and then use that token to communicate with the API
* On Windows, you can use the following curl command to request a token from the server
    * This tutorial has configured launchSettings.json to use localhost:5443 for https
    * The command is a POST request to our localhost/connect/token endpoint (which can be found int he discovery document), by specifying the client id, client secret, the scope, and the grant type (here, we are a client, so we are requesting a client token)
    * This will return a JWT Bearer Token - once a client has this token, it can make requests to protected API endpoints
```
curl -XPOST "https://localhost:5443/connect/token" -H "Content-Type: application/x-www-form-urlencoded" -H "Cache-Control: no-cache" -d "client_id=m2m.client&scope=weatherapi.read&client_secret=SuperSecretPassword&grant_type=client_credentials"
```

## Adding the API
* The goal is to create a protected API wherein, in order to access that API, the app first has to ask IdentityServer for a bearer token, then have the API check that token
* In a new directory, create a protected API using ASP.NET Core Web API template
```
dotnet new webapi
```
* Then, create a new solution and add the project file to it
* Now, if the weatherapi project is run, navigating to its url (/weatherforcast) (here, the tutorial set it to localhost:5445) will return randomly generated weather data as json
* Once the API is protected, all clients must have a bearer token in order to be authorized to access a resource, for the application to be able to execute this, add the IdentityServer AccessTokenValidation NuGet package
* In the weatherapi's Startup.cs, configure the service to use authentication in the ConfigureServices method and add authentication to the pipeline in the Configure method
* To protect an API route, use the [Authorize] decorator in the Controller - at this point, if you re-run the application, it will return a 401 unauthorized error if you try to access the /weatherforecast endpoint
* To simulate an authorized API request, generate a new Bearer token using the curl command from the previous section and then add it to the following curl command, which should print randomly generated weather data to the console
```
curl -XGET "https://localhost:5445/weatherforecast" -H "Content-Type: application/x-www-form-urlencoded" -H "Authorization: Bearer <bearer-token-here>"
```

## Adding MVC
* This app should be able to do two things: call the protected API application (set the bearer token and make calls to the IdentityServer to get that token) and to be able to login to the app so that the application itself is protected (authorization and authentication)
* Create a client folder and add the weatherapi directory to it, then create a new client solution within the client directory that will bundle the weatherapi and MVC solutions together and add the weatherapi project file to it
* The add a new ASP.NET Core Web App to the client solution (make sure it's an MVC Web App template)
* Update the launchSettings of the MVC web app to match the ports that were configured for the interactive client in the Config of the IdentityServer (here we use port 5444)
* Add the IdentityModel NuGet package to access specific helpers for the application
* In the Views directory, create a .cshtml file that will display the data (make sure to add any relevant data models into the Models directory)
* Add a new route to access and display the weather data in the HomeController.cs file
    * Make an Http request to the API url (here, it's localhost:5445/weatherforecast), deserialize the data, and pass it to the View that was created
    * Here, you can comment out the [Authorize] decorator briefly to test the MVC app