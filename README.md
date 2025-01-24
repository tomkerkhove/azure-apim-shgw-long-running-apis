# Supporting long-running requests in Azure API Management's self-hosted gateway

This repository contains a sample application that demonstrates how to support long-running requests in Azure API Management's self-hosted gateway, which by default times out after 5 minutes.

> ⚠️ While self-hosted gateway can support long-running requests, it is required to support this in the whole stack given other dependencies can have different timeouts defined.
>
> For example:
> - Load balancer
> - NAT gateway
> - Public IP address, etc can have a different timeout.

## How do I increase the timeout for the gateway?

It is simple, you can influence this through the `net.client.http.call-timeout` & `net.client.tcp.connection.timeouts.idle` settings by specifying, for example, `00:30:00` to allow up to 30 minutes.

## Running the sample

1. Create a new Azure API Management service
2. Create a new self-hosted gateway
3. Create a new API (you can import [`deploy\Timeout-API.openapi.json`](deploy\Timeout-API.openapi.json)) and use `timeout` as API URL suffix
4. Allow your self-hosted gateway to expose the API
5. Apply the [`deploy\operation.policy.xml`](deploy\operation.policy.xml) policy on the "Timeout" operation
6. Configure the self-hosted gateway in [`src\docker-compose.yml`](src\docker-compose.yml) to talk to your new Azure API Management service
7. Deploy the self-hosted gateway with long-running API simulator locally by using `docker-compose up --build --force-recreate` in `/src` folder
8. Send a request to the API with a `timeoutInSeconds` query parameter, for example `http://localhost:8080/timeout/docker-sidecar?timeoutInSeconds=450`

You will see that request will complete after 450 seconds, with a 200 OK and notice a log line similar to this:

```cli
[Info] 2025-01-24T09:45:41.035 [GatewayLogs], isRequestSuccess: true, totalTime: 451569, category: "GatewayLogs", callerIpAddress: "172.21.0.1", timeGenerated: 2025-01-24T09:45:41.035, region: "Anywhere", correlationId: "e7b1689f-5578-4a1c-bdb0-d875a0df26c2", method: "GET", url: "http://localhost:8080/timeout/?timeoutInSeconds=450", backendResponseCode: 200, responseCode: 200, responseSize: 592, cache: "none", backendTime: 450448, apiId: "timeout-api", operationId: "679264a1f549735d4dc99874", apimSubscriptionId: "Tom", clientTime: 9, clientProtocol: "HTTP/1.1", backendProtocol: "HTTP/1.1", apiRevision: "1", isTraceRequested: true, isTraceAllowed: true, backendMethod: "GET", backendUrl: "http://longrunning.api:8088/long-running/?timeoutInSeconds=450", correlationId: e7b1689f-5578-4a1c-bdb0-d875a0df26c2
```

When using [API Inspector](https://learn.microsoft.com/azure/api-management/api-management-howto-api-inspector), you will see that your configured timeout is being logged:

```diff
{
    "source": "request-forwarder",
    "timestamp": "2025-01-24T09:45:41.9251095Z",
    "elapsed": "00:00:00.8498637",
    "data": {
+       "message": "Request is being forwarded to the backend service. Timeout set to 1800 seconds",
        "request": {
            "method": "GET",
            "protocol": "http",
            "url": "http://longrunning.api:8088/long-running/?timeoutInSeconds=450",
            "proxyUrl": null,
            "headers": [
                {
                    "name": "Host",
                    "value": "longrunning.api"
                }
            ],
            "clientCertificates": []
        }
    }
}
```

You can find a full trace example in [`request-trace.json`](request-trace.json).


## License Information

This is licensed under The MIT License (MIT). Which means that you can use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the web application. But you always need to state that Tom Kerkhove is the original author of this application.

Read the full license [here](LICENSE).
