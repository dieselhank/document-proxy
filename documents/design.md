## DocumentProxy Solution/Project

The `DocumentProxy` project contains the implementation of the proxy to the 3rd party service. It uses Azure Functions for each endpoint and relies on Azure Cosmos DB for data persistence.

### Functions

#### DocumentRequest POST
This function is the first step in the interaction with the service. Requests are made via `POST` requests to the `/request` endpoint.

A `JSON` body is expected with the `POST` of the form.
```JSON
{
    "body": string
}
```
On sucess the function returns a 200 status with a generated unique id for the request in the response body.

Internally the function will perform some simple validation of the request, generate the documentId, and make a dummy request to the 3rd party service that contains the following data.

```JSON
{
    "body": {body},
    "callback": "/callback/{documentId}"
}
```

The request data is saved in the `DocumentProxy` Cosmos DB in the `Documents` collection using the `DocumentDetails` DTO. The initially saved data is of the form:

```JSON
{
    "id": "{documentId}",
    "Body": "body string",
    "CreatedOn": "UTC date"
}
```

#### DocumentPostCallback POST

This function supports the first callback status request from the 3rd party service. Requests are expected to be made by the 3rd party service to urls of the form `/callback/{documentId}`.

The request body is expected to contain the string `STARTED`.

The `documentId` is used to lookup the previous request data in Cosmos DB. The `STARTED` status is added to the record in Cosmos DB.

The data in record in Cosmos DB would then be of the form:

```JSON
{
    "id": "{documentId}",
    "Body": "body string",
    "CreatedOn": "UTC date",
    "Status": [
        {
            "Status": "STARTED",
            "Detail": "",
            "CreatedOn": "UTC date"
        }
    ]
}
```

On success a status code of 204 is returned.

#### DocumentPutCallback PUT

This function supports subsequent callback status update requests to be made by the 3rd party service to urls of the form `/callback/{documentId}`.

The request body is expected to be of the form:

```JSON
{
  "status": string,
  "detail": string
}
```

Where `status` can be either `PROCESSED`, `COMPLETED`, or `ERROR` and `detail` can be any string value.

The `documentId` is used to lookup the previous document data in Cosmos DB. The submitted status is added to the record in Cosmos DB.

The data in record in Cosmos DB would then be of the form:

```JSON
{
    "id": "{documentId}",
    "Body": "body string",
    "CreatedOn": "UTC date",
    "Status": [
        {
            "Status": "STARTED",
            "Detail": "",
            "CreatedOn": "UTC date"
        },
        {
            "Status": "PROCESSED",
            "Detail": "successfully processed",
            "CreatedOn": "UTC date"
        }
    ]
}
```

On success a status code of 204 is returned.

#### DocumentStatusRequest GET

This function supports clients requesting the current status of a previous document request. The request url is of the form `/status/{documentId}`.

The `documentId` is used to lookup the document data in Cosmos DB.

On success a status code of 200 is returned.

The data returned by the function is a mapping of the data in Cosmos DB.

```JSON
{
    "status": "{Status[last].Status}|null",
    "detail": "{Status[last].Detail}|null",
    "body": "{Body}",
    "createdOn": "{CreatedOn}",
    "updatedOn": "{Status[last].CreatedOn}|{CreatedOn}"
}
```

## Unit Tests

The `DocumentProxyTests` project contains unit tests for the `DocumentProxy` project.
