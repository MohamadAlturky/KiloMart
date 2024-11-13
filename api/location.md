API Documentation for LocationController
======================================

Overview
--------

The LocationController API provides endpoints for managing location data. The API is part of a larger application and is designed to be consumed by a Flutter app.

Endpoints
--------

### 1. Insert Location

* **Method:** `POST`
* **Path:** `/api/location`
* **Request Body:** `LocationInsertModel`
* **Response:**
	+ `201 Created`: Location successfully inserted
	+ `400 Bad Request`: Validation errors or other insert errors
* **Description:** Inserts a new location into the database.

#### Request Body

```json
{
  "name": string,
  "longitude": float,
  "latitude": float,
  "party": int
}
```

#### Response

```json
{
  "id": int,
  "name": string,
  "longitude": float,
  "latitude": float,
  "party": int
}
```

### 2. Update Location

* **Method:** `PUT`
* **Path:** `/api/location/{id}`
* **Request Body:** `LocationUpdateModel`
* **Response:**
	+ `200 OK`: Location successfully updated
	+ `400 Bad Request`: Validation errors or other update errors
	+ `404 Not Found`: Location not found
* **Description:** Updates an existing location in the database.

#### Request Body

```json
{
  "name": string,
  "longitude": float,
  "latitude": float
}
```

#### Response

```json
{
  "id": int,
  "name": string,
  "longitude": float,
  "latitude": float,
  "party": int
}
```

### 3. Get Mine

* **Method:** `GET`
* **Path:** `/api/location/mine`
* **Response:**
	+ `200 OK`: List of locations for the current user
* **Description:** Retrieves a list of locations associated with the current user.

#### Response

```json
[
  {
    "id": int,
    "name": string,
    "longitude": float,
    "latitude": float,
    "party": int
  },
  {
    "id": int,
    "name": string,
    "longitude": float,
    "latitude": float,
    "party": int
  },
  ...
]
```

 Authentication
--------------

This API requires authentication for all endpoints. The `GetMine` endpoint is restricted to users with the `Customer` role.

API Security
------------

This API uses HTTPS to encrypt data transmitted between the client and server. Authentication is handled via a separate authentication service.

Flutter App Usage
-----------------

To use this API in a Flutter app, you will need to make HTTP requests to the API endpoints using a library such as `http` or `dio`. You will need to authenticate the user and obtain an authentication token, which should be included in the `Authorization` header of each request.

Example using `http` library:

```dart
import 'package:http/http.dart' as http;
import 'package:location/location.dart';

class LocationApi {
  final String _baseUrl = 'https://example.com/api';
  final String _apiKey = 'YOUR_API_KEY_HERE';

  Future<Location> insertLocation(LocationInsertModel model) async {
    final url = '$_baseUrl/location';
    final headers = {
      'Authorization': 'Bearer $_apiKey',
      'Content-Type': 'application/json'
    };
    final response = await http.post(
      url,
      headers: headers,
      body: jsonEncode(model),
    );
    if (response.statusCode == 201) {
      return Location.fromJson(jsonDecode(response.body));
    } else {
      throw Exception('Failed to insert location');
    }
  }

  // ...
}
```

Note: This is just an example and you should handle errors and edge cases properly in your production code.