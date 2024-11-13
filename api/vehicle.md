**Vehicle Controller API Documentation**

### Introduction

The Vehicle Controller API provides endpoints for managing vehicle data. This document outlines the available endpoints, request and response formats, and any applicable authentication and authorization requirements.

### Endpoints

#### 1. Create Vehicle

* **URL:** `POST /api/vehicle`
* **Request Body:** `VehicleInsertModel`
	+ `number`: string (required)
	+ `model`: string (required)
	+ `type`: string (required)
	+ `year`: string (required)
* **Response:** `HttpCreatedResponse` with `VehicleApiResponse` or `BadRequestResponse` with error messages

Example Request:
```json
{
  "number": "ABC123",
  "model": "Toyota",
  "type": "Car",
  "year": "2022"
}
```
Example Response:
```json
{
  "id": 1,
  "number": "ABC123",
  "model": "Toyota",
  "type": "Car",
  "year": "2022"
}
```

#### 2. Update Vehicle

* **URL:** `PUT /api/vehicle`
* **Request Body:** `VehicleUpdateModel`
	+ `id`: int (required)
	+ `number`: string (optional)
	+ `model`: string (optional)
	+ `type`: string (optional)
	+ `year`: string (optional)
* **Response:** `OkResponse` with `VehicleApiResponse` or `BadRequestResponse` with error messages

Example Request:
```json
{
  "id": 1,
  "number": "DEF456",
  "model": "Honda",
  "type": "Car",
  "year": "2022"
}
```
Example Response:
```json
{
  "id": 1,
  "number": "DEF456",
  "model": "Honda",
  "type": "Car",
  "year": "2022"
}
```

#### 3. Get Mine (Delivery)

* **URL:** `GET /api/vehicle/mine`
* **Authentication:** Required (Delivery role)
* **Response:** `OkResponse` with array of `VehicleApiResponse`

Example Response:
```json
[
  {
    "id": 1,
    "number": "ABC123",
    "model": "Toyota",
    "type": "Car",
    "year": "2022"
  },
  {
    "id": 2,
    "number": "DEF456",
    "model": "Honda",
    "type": "Car",
    "year": "2022"
  }
]
```

### Error Handling

* `BadRequestResponse`: 400 status code with error messages
* `NotFoundResponse`: 404 status code
* `InternalServerErrorResponse`: 500 status code with error messages

### Flutter App Implementation

To use these APIs in your Flutter app, you can create a `VehicleService` class with methods that make HTTP requests to these endpoints.

Example:
```dart
import 'package:http/http.dart' as http;
import 'dart:convert';

class VehicleService {
  final Uri _baseUrl = Uri.parse('https://example.com/api/vehicle');

  Future<Vehicle> createVehicle(VehicleInsertModel model) async {
    final response = await http.post(
      _baseUrl,
      headers: {
        'Content-Type': 'application/json',
      },
      body: jsonEncode(model),
    );

    if (response.statusCode == 201) {
      return Vehicle.fromJson(jsonDecode(response.body));
    } else {
      throw Exception('Failed to create vehicle');
    }
  }

  Future<Vehicle> updateVehicle(VehicleUpdateModel model) async {
    final response = await http.put(
      _baseUrl,
      headers: {
        'Content-Type': 'application/json',
      },
      body: jsonEncode(model),
    );

    if (response.statusCode == 200) {
      return Vehicle.fromJson(jsonDecode(response.body));
    } else {
      throw Exception('Failed to update vehicle');
    }
  }

  Future<List<Vehicle>> getMine() async {
    final response = await http.get(
      _baseUrl.replace(path: '/mine'),
      headers: {
        'Authorization': 'Bearer [token]',
      },
    );

    if (response.statusCode == 200) {
      return (jsonDecode(response.body) as List)
          .map((json) => Vehicle.fromJson(json))
          .toList();
    } else {
      throw Exception('Failed to get mine');
    }
  }
}
```