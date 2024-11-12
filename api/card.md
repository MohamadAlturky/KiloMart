**Card API Documentation**
==========================

**Overview**
------------

The Card API is used to manage card information for customers. It provides endpoints for inserting, updating, getting, and listing cards.

**Endpoints**
--------------

### 1. Insert Card

`POST /api/card`

*   Insert a new card into the system.
*   Requires a valid `CardInsertModel` in the request body.
*   Returns the newly created card with a `201 Created` status code.

Example Request Body:

```json
{
    "HolderName": "John Doe",
    "Number": "1234567890",
    "SecurityCode": "123",
    "ExpireDate": "2024-12-31T00:00:00",
    "Customer": 1
}
```

Example Response:

```json
{
    "Id": 1,
    "HolderName": "John Doe",
    "Number": "1234567890",
    "SecurityCode": "123",
    "ExpireDate": "2024-12-31T00:00:00",
    "Customer": 1
}
```

### 2. Update Card

`PUT /api/card/{id}`

*   Update an existing card in the system.
*   Requires a valid `CardUpdateModel` in the request body.
*   Returns the updated card with a `200 OK` status code.
*   The `id` parameter is required and should match the `Id` of the card to be updated.

Example Request Body:

```json
{
    "HolderName": "Jane Doe",
    "Number": "9876543210",
    "SecurityCode": "456",
    "ExpireDate": "2024-12-31T00:00:00"
}
```

Example Response:

```json
{
    "Id": 1,
    "HolderName": "Jane Doe",
    "Number": "9876543210",
    "SecurityCode": "456",
    "ExpireDate": "2024-12-31T00:00:00",
    "Customer": 1
}
```

### 3. Get My Cards

`GET /api/card/mine`

*   Get the list of cards belonging to the logged-in customer.
*   Requires authentication with a valid customer account.
*   Returns an array of `CardApiResponse` objects with a `200 OK` status code.

Example Response:

```json
[
    {
        "Id": 1,
        "HolderName": "John Doe",
        "Number": "1234567890",
        "SecurityCode": "123",
        "ExpireDate": "2024-12-31T00:00:00",
        "Customer": 1
    },
    {
        "Id": 2,
        "HolderName": "Jane Doe",
        "Number": "9876543210",
        "SecurityCode": "456",
        "ExpireDate": "2024-12-31T00:00:00",
        "Customer": 1
    }
]
```

### 4. Get All Cards

`GET /api/card/list`

*   Get the list of all cards in the system.
*   Returns an array of `CardApiResponseWithCustomerName` objects with a `200 OK` status code.
*   Supports pagination using the `page` and `pageSize` query parameters.

Example Response:

```json
[
    {
        "Id": 1,
        "HolderName": "John Doe",
        "Number": "1234567890",
        "SecurityCode": "123",
        "ExpireDate": "2024-12-31T00:00:00",
        "CustomerId": 1,
        "CustomerName": "John Doe",
        "IsActive": true
    },
    {
        "Id": 2,
        "HolderName": "Jane Doe",
        "Number": "9876543210",
        "SecurityCode": "456",
        "ExpireDate": "2024-12-31T00:00:00",
        "CustomerId": 2,
        "CustomerName": "Jane Doe",
        "IsActive": true
    }
]
```

**Models**
----------

### CardInsertModel

```json
{
    "HolderName": string,
    "Number": string,
    "SecurityCode": string,
    "ExpireDate": DateTime,
    "Customer": int
}
```

### CardUpdateModel

```json
{
    "HolderName": string,
    "Number": string,
    "SecurityCode": string,
    "ExpireDate": DateTime
}
```

### CardApiResponse

```json
{
    "Id": int,
    "HolderName": string,
    "Number": string,
    "SecurityCode": string,
    "ExpireDate": DateTime,
    "Customer": int
}
```

### CardApiResponseWithCustomerName

```json
{
    "Id": int,
    "HolderName": string,
    "Number": string,
    "SecurityCode": string,
    "ExpireDate": DateTime,
    "CustomerId": int,
    "CustomerName": string,
    "IsActive": bool
}
```

**Using in Flutter App**
-------------------------

To use these APIs in a Flutter app, you can use the `http` package to make HTTP requests.

```dart
import 'package:http/http.dart' as http;

Future<CardApiResponse> insertCard(CardInsertModel model) async {
  final response = await http.post(
    Uri.parse('https://example.com/api/card'),
    headers: {
      'Content-Type': 'application/json',
    },
    body: jsonEncode(model),
  );

  if (response.statusCode == 201) {
    return CardApiResponse.fromJson(jsonDecode(response.body));
  } else {
    throw Exception('Failed to insert card');
  }
}
```

Remember to replace the `https://example.com/api/card` URL with your actual API endpoint.