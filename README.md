# How to Run
1. Clone this repository to your local folder
2. in `/src` run `dotnet restore` or open `/src/OKAY.Assignment.sln` and let `Package Manager` complete the restoration
3. run `dotnet ef database update` with CLI or run `update-database` in `Package Manager`
4. run `dotnet run` with CLI or press F5 to run in Visual Studio

> Notes: SSDT is required for SQL Express   

> Notes: .NET 5 SDK should be installed