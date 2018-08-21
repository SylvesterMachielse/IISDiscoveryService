[![Build Status](https://travis-ci.org/SylvesterMachielse/IISDiscoveryService.svg?branch=master)](https://travis-ci.org/SylvesterMachielse/IISDiscoveryService)
# IISDiscoveryService
A service that scans IIS websites and reports them on an enpoint

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

## Architecture
![architecture](https://github.com/SylvesterMachielse/IISDiscoveryService/raw/master/architecture.PNG "Architecture")

### How to install
There is a work in progress installation script [here](https://raw.githubusercontent.com/SylvesterMachielse/IISDiscoveryService/master/InstallScript.ps)

The idea is:
1. Download the [latest release](https://github.com/SylvesterMachielse/IISDiscoveryService/releases/download/v1.0/publish.7z)
2. Unzip 
3. Modify the settings file to your needs (not in the script)
4. Create a service. The script does this with [NSSM](https://nssm.cc/)
