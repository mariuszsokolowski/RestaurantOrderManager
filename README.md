#  :hammer_and_wrench: RestaurantOrderManager
A web application for managing customer orders in restaurant, built with a .NET Core API (backend) and a Vue.js (frontend) within an ASP.NET application. This app allows users to place, track, and manage orders seamlessly.
## ✨ Features
- 📜 **API documentation with Swagger** – Full and interactive API documentation provided by Swagger, allowing users to test endpoints.
- 🔗 **Easy API integration** – Simple and fast integration with our API, with access to Swagger documentation.
- ⚙️ **Real-time API testing** – Test our API in real-time directly from the Swagger documentation interface.
## :mechanical_leg: Installation
### :pushpin: Prerequisites

- **.NET SDK**: netcoreapp2.1. [Download .NET](https://dotnet.microsoft.com/en-us/download/dotnet/2.1/)
- **Node.js & npm** [Download Nodje.js & npm](https://nodejs.org/)
- **MySql**  [MySQL](https://dev.mysql.com/downloads/)
### 🔧 Steps

Install dotnet-ef tools:
```cmd
dotnet tool install --global dotnet-ef
```

Clone repository:
```cmd
https://github.com/mariuszsokolowski/MyRestaurant.git
```

Set database and change **MysqlConnection** object in appsettings.json (set url, port, user and password):
```cmd
cd MyRestaurant.API
notepad .\appsettings.json
```

Update database:
```cmd
cd MyRestaurant.API
dotnet-ef database update
```


## 📚 Usage
### API
Go to API folder:
```bash
cd MyRestaurant.API
```

Run app:
```bash
dotnet run
```

The API endpoints can be accessed at:
```bash
https://localhost:44389/swagger
```
### Frontend
Go to Client folder:
```bash
cd MyRestaurant.Client
```
Run app:
```bash
dotnet run
```
Once the API and frontend are running, you can access the application at:
```bash
https://localhost:44360/
```


## :rocket: Future Improvements
- [ ] Update app from .Net Core 2.1 to .NET 8
- [ ] CLI Feature for Inserting Random Data