#CityInfo REST API Made with ASP.NET Web API 6  
In this API you can find 2 entities: City and PointOfInterest  
All Cities have their own PointOfInterest, so there is 1 to many relationship between them.  
I'm using Microsoft SQL Server for the database.  
You can make all the CRUD operations including PATCH request too.  
I'm using AutoMapper to map the DTO's when returning them to the client.  
I'm also using Logger to logging out some info.  
In the api there is FileController where you can download a file.  
I'm using Entity Framework to manipulating with the Db.  
The API is designed with Interface-Repository pattern and is using asynchronous calls.  
I've implemented Searching, Filtering and Paging too.  
You can search the cities by their name or filter them out by their name. Also you can put the current page and how many cities per page for the paging funcionality.  
This API is using Token based Authentication and Authorization.  
You can login with Username and Password and get Token in return. Then the API is secured so you must provide the Bearer token with every API call in order to get the data.  
Also I've implementend an Authorization Policy but only for demo purposes( it is commented out ) where users only for specific city can pull out info from the API.  
This API is also using Versioning, the City Controller is using Version 1.0 and 2.0 while the PointOfInterests Controller is using only version 2.0  
Also I've been documented this API. I'm using Swagger support together with XML comments and Described Response Types to see what is the return type and what are the schemas.  

The API is using the following packages:  
 - AutoMapper.Extensions.Microsoft.DependencyInjection  
 - Microsoft.AspNetCore.Authentication.JwtBearer
 - Microsoft.AspNetCore.JsonPatch
 - Microsoft.AspNetCore.Mvc.NewtonsoftJson
 - Microsoft.AspNetCore.Mvc.Versioning
 - Microsoft.EntityFrameworkCore
 - Microsoft.EntityFrameworkCore.Design
 - Microsoft.EntityFrameworkCore.SqlServer
 - Microsoft.EntityFrameworkCore.Tools
 - Microsoft.IdentityModel.Tokens
 - Serilog.AspNetCore
 - Swashbuckle.AspNetCore
 - System.IdentityModel.Tokens.Jwt

