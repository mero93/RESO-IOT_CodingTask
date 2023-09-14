# Task

Developed the system which will contain two components:

* Light sensor for environment information collection
* Server to store and provide reports

## Sensor specification

* Units: Lux
* Resolution: 0.5 Lux (for example: 100.5 - correct value, 100.7 - incorrect value)
* Measurement frequency: 1 time per 15 minutes
* Frequency of sending data to the server: 1 time per hour
* Communication protocol with the server: HTTP
* Data format: JSON

## Technologies
- .NET Core lates or 6.x
- ASP.NET Core latest or 6.x
- MS SQL or Azure CosmosDB (Emulator or local databse is accepted MySQL)
- Any other technologies

## Server specification

* POST /devices/{deviceId}/telemetry - saves data from the sensor

```json
[
    {
        "illum": 123.5, // illuminance, значение освещённости в Люксах
        "time": 1692946687 // timestamp, время измерения Unix timestamp
    },
    {
        "illum": 123.0,
        "time": 1692947687
    },
    {
        "illum": 122.5,
        "time": 1692948687
    },
]
```

* GET /devices/{deviceId}/statistics - returns the maximum illuminance value for each of the last thirty days. The data is sorted by date as per the example below

```json
[
    {
        "date": "2023-06-17",
        "maxIlluminance": 150.5
    },
    {
        "date": "2023-06-18",
        "maxIlluminance": 132.5
    },
    {
        "date": "2023-06-19",
        "maxIlluminance": 141.0
    }
]
```

## Task Format
- The source code of the task must be uploaded to the github.com or https://git.epam.com/ version control system and then submit a link to the repository for review. If https://git.epam.com/ is used, you can add siarhei_zhalezka@epam.com, andrei_stselmashenka@epam.com to the project.
- Send a link to the result by email - siarhei_zhalezka@epam.com, andrei_stselmashenka@epam.com
- The desired task duration is one week - but you can submit it earlier. If it's overdue, send it too.

## Questions
Questions can be asked by email - siarhei_zhalezka@epam.com, andrei_stselmashenka@epam.com.

## Additional tasks

- Integrate authentication
     - If you have access to Azure AD using https://learn.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-client-creds-grant-flow
     - If not access, then using the key. The user who passes this key has the right to use API methods
- Logging. You need to add logging to the console. Provide the ability to change receivers for logs (console, file, http endpoint)
- Exception Handling. Every exception must be logged, including all future exceptions. Also, depending on the environment, information about the user (user id) should be included. For QA information should be included, for others it should not be available.

## Hints

- Due to the lack of real sensors, it is planned to develop a simulator. It is assumed that in the first half of the day the illumination only increases, and in the second half it only decreases; the peaks on different days may differ.

- Data types are used according to physical limitations (illumination on a clear sunny day)

- The use of patterns, Linq, stored procedures, writing Unit tests is encouraged

- In the context of this task, it is assumed that the time on the server and device is set correctly and the time zone is the same (UTC), the system does not require device registration

- The use of SOLID principles when developing a solution is encouraged

- Plus, if you plan to expand the system in the future, adding other sensors, for example, temperature

- Using Dependency Injection plus