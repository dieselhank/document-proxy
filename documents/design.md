## DocumentProxy Solution/Project

The `DocumentProxy` project contains the implementation of the proxy to the 3rd party service. It uses Azure Functions for each endpoint and relies on Azure Cosmos DB for data persistence.

### Functions

#### DocumentRequest
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

## Unit Tests

The `DocumentProxyTests` project contains unit tests for the `DocumentProxy` project.
