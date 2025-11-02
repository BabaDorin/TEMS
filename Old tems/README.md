# TEMS
### Technical Equipment Management System

## IMPORTANT

>This project is going to be completely refactored. This process might take several months, as I am the only one working on it, and there is no interest apart from >offering this project as a free solution for invetory management, therefore do not expect any stable releases in the next period of time.

>If you are interested in contributing to this project, please reach out to me - bvd.dorin@gmail.com.

>As part of the refactoring process, the project is going to be split into two separate repositories: 
>- TEMS Backend -> ASP.NET WebAPI project (.NET 7). 
>- TEMS Frontend -> Angular

>Therefore, this repository can be considered obsolete.

TEMS is an IT Service Management solution which serves for managing IT assets of an institution and provides a set of instruments oriented to the team of technicians. It is built according to the needs of an educational institution and adapted for such an ecosystem, but it can be implemented is any other organization that needs an effective and easy-to-use inventory management instrument.

> Note: The application is still being tested and there is no stable version released yet. When it is 100% ready for production, a detailed documentation will be delivered.

### Architecture
- Client-server web application: 
    - ASP.NET WebAPI (.NET 5);
    - Entity Framework Core;
    - Angular (V.11);
    - MS SQL Server;

### Features
- IT Hardware asset management
    - You define the set of equipment types that will get managed by the system;
        - ex: Computers, Scanners, TVs, Projectors, Cars, Jets, UFOs etc. 
    - You define the set of attributes assigned to the equipment types 
        - ex: For my printers, I would like to know the manufacturer, model and whether it supports colour printing or not;
    - Reduce information redundancy via creating definitions for your types;
        - ex: HP LaserJet 1018 is a printer definition that contains the following property-value associations: Manufacturer - _Hewlett Packard_, Model - _Lasejet 1018_, Colour printing - _false_. When registering a printer of this type, you can specify the LaserJet 1018 definition and provide only the information that uniquely identifies it within the system;
    - Uniquely identify equipment via TEMSID (a short and unique ID label, also called as inventory number) and / or serial number;
    - Walk the equipment through the entire IT Asset management life cycle;
    
- Personnel and rooms management
    - Register organization's personnel and rooms;
    - Allocate registered equipment to personnel and rooms;
    - Send E-mails to personnel;
- Create tickets
    - Application's users and guests can report technical related problems;
    - Technicians are notified via E-mail when a new ticket is created and are encouraged to solve it as fast as possible;
- Generate excel reports 
- Upload files (like printer drivers, utilities etc) to the library (Useful tool for technicians)
- Claims and role based user system
    - System administrators can create user accounts and to assign them both roles and independent claims;
    - JWT Authentication :( 
- Manage keys and key allocations
- Internationalization
- Other stuff

### Overview
A small part of the app, just to have a visual representation of what it is all about :)

![TEMS](https://i.ibb.co/QpP7BTV/website-presentation.jpg)

### Installation

I assume that you already have ms sql server, nodeJs, angular and .net 5 installed on your machine. 

- Initial configuration: 
Consult the app.settings.json file (`API/temsAPI`) to check some of the configuration data you might be interested in. An admin account is created at the first launch of the app (Login: tems@admin, password: it's indicated in app.settings.json file). Set a secure password before launching the app and, for safety reasons, you can remove it from the configuration file once the account is created.
- Create the database: 
Open the app.settings.json file from `API/temsAPI` and set the `DefaultConnectionString` according to your machine. After that, open the NuGet Package Manager Console and type the `update-database` command in order to create the database.
- Install npm packages: 
Open a terminal window inside angular project: `UI/temsUI` and type `npm i` to install all of the required dependencies.
- Launch the project: 
Launch the ASP.NET WebApi solution (within VS or dotnet cli) and run the `ng-serve` command inside angular project to launch the angular app. By default, the app is accesible at the following address: _localhost:4200_
 

