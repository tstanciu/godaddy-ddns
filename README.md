# GoDaddyDDNS

GoDaddy does not have a Dynamic DNS service, but provides documentation to programmatically interact with their platform: https://developer.godaddy.com/doc/endpoint/domains

Considering this aspect, I wrote this worker to constantly check the public IP of the network in which it is installed and compare it with the one configured in type A records in the GoDaddy platform. When a difference appears between these IPs, the worker will update the correct IP in the configured type A records through the API.

The worker is developed multi-platform (it can run in any environment), but the tested version is in a docker container.

To make this worker functional, a minimum configuration is required:

```
{
  "Domain": "*********",
  "Records": [ "@" ],
  "Key": "*********",
  "Secret": "*********",
  "ExecutionTimeInSeconds": 3300
}
```

## Stack
* .NET (C#)
* Docker

## Changelog
**1.0.0** - The first release version of the service.  
**1.0.1** - ConfigureAwait(false) was set for the http client get async calls.  
**1.0.2** - .NET 6 upgrade.  
**1.0.3** - The ```ExecutionTimeInSeconds``` parameter has been replaced with ```ExecutionFrequency``` which accepts values in the format ```30s```, ```10m```, ```1h``` or ```1d```.