[![Build Status](https://travis-ci.org/SylvesterMachielse/IISDiscoveryService.svg?branch=master)](https://travis-ci.org/SylvesterMachielse/IISDiscoveryService)
# IISDiscoveryService
A service that scans IIS websites and syncs them with the PrometheusFileServiceDiscoveryApi. 

## Configuration:
```javascript
{
  "TargetClient": "http://localhost:9099",      //the location of PrometheusFileServiceDiscoveryApi
  "GlobalTags": { "globalkey": "globalvalue" }, // tags that will be added to ALL targets
  "SynchronizationRules": [                     // a list of rules by which targets will be identified, added, removed and patched
  {
    "Name": "mysite",                           // name of the rule
    "HostRegexFilter": ".+(?=.mysite\\.com)",   //the regex match by which sites for this rule are identified
    "TargetLabels": { "host": "(.+)" }          //labelnames with a fixed key and a regex match for it's value
  },
  {
    "Name": "websites",
     "HostRegexFilter": "(website)",
     "TargetLabels": { "environment": "(.+)" }
  }]
}
```

In the above example:
All sites that end with `mysite.com` will be synced with the PrometheusFileServiceDiscoveryApi. 
* sites that are not listed as a target are added,
* targets that are not listed as sites are removed
* sites that have corresponding targets, will get relabeled if the label configuration is changed.

The synchronization runs every minute. The configuration file is re-read on every run.

## Architecture
![architecture](https://github.com/SylvesterMachielse/IISDiscoveryService/raw/master/architecture.PNG "Architecture")

## Installation
### Prerequisites
This service works in conjuction with the [PrometheusFileServiceDiscoveryApi](https://github.com/SylvesterMachielse/PrometheusFileServiceDiscoveryApi). The PrometheusFileServiceDiscoveryApi must be running somewhere.

### How to install
There is a work in progress installation script [here](https://raw.githubusercontent.com/SylvesterMachielse/IISDiscoveryService/master/InstallScript.ps)

The idea is:
1. Download the [latest release](https://github.com/SylvesterMachielse/IISDiscoveryService/releases/download/v1.0/publish.7z)
2. Unzip 
3. Modify the settings file to your needs (not in the script)
4. Create a service. The script does this with [NSSM](https://nssm.cc/)
