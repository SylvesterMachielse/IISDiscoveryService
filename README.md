[![Build Status](https://travis-ci.org/SylvesterMachielse/IISDiscoveryService.svg?branch=master)](https://travis-ci.org/SylvesterMachielse/IISDiscoveryService)
# IISDiscoveryService
A service that scans IIS websites and reports them on an enpoint

## What does it do
When installed, you can call `GET` http://myserver:9080/api/v1/websites and the endpoint will return websites running in IIS:
```javascript
["site1.com",
"site2.com",
"test.site3.com",
"moresites.com",
"etc.com"]
```

## Architecture
* Install this tool on the server that hosts iis websites you want to scrape with prometheus.
* Install the [File service discovery](https://github.com/SylvesterMachielse/PrometheusFileServiceDiscoveryApi) next to prometheus

At this point there is no middleware that can tie these services together. The idea is you get websites from your IIS service and compare them to the targets in prometheus. PUT/PATCH/DELETE targets when necessary.

### How to install
There is a work in progress installation script [here](https://raw.githubusercontent.com/SylvesterMachielse/IISDiscoveryService/master/InstallScript.ps)

The idea is:
1. Download the [latest release](https://github.com/SylvesterMachielse/IISDiscoveryService/releases/download/v1.0/publish.7z)
2. Unzip 
3. Modify the settings file to your needs (not in the script)
4. Create a service. The script does this with [NSSM](https://nssm.cc/)

## Configuration:
```javascript
{
  "Host": "http://localhost:9080",
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*"
}

```
