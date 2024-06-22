# Testcontainers #

Testcontainers is an open source framework for providing throwaway, lightweight instances of databases, message brokers, web browsers, or just about anything that can run in a Docker container.

https://testcontainers.com/

## Installation ##

With only Docker Engine installed on Ubuntu Testcontainers initially complained that Docker was not properly configured. There wasn't
a Docker configuration file in my home directory.

Couldn't install Docker Desktop as it was not yet available on Ubuntu 24.04.

Installed Testcontainers Cloud from https://testcontainers.com/cloud/ and authenticated with my DockerHub account.
Was installed from a downloaded DEB file (i.e. ‘sudo apt install ~/Downloads/testcontainers-desktop_linux_x86-64.deb’).

After the Testcontainers Cloud client was running I was able to successfully run tests, in fact even after the 
client was shut down I could still run them so whatever the original issue was it was resolved (bit more below).

The cloud client lets you run tests in the cloud to reduce load on your machine.

Custom Configuration: https://dotnet.testcontainers.org/custom_configuration/



## Docker Config ##

Had an issue initially running Testcontainers, which was resolved by running the Testcontainers Cloud client. One
possible issue was that it couldn't find the Docker configuration file in my home directory. Will often have files
like the examples below installed, but I think they might only be installed on Ubuntu if you install Docker Desktop or
possibly for a rootless installation.

    daemon.json:
    
        {
        "builder": {
            "gc": {
            "defaultKeepStorage": "20GB",
            "enabled": true
            }
        },
        "experimental": false
        }

    config.json (don’t think any of this is actually needed, so what should I put in there?):
    
        {
            "auths": {
                "twrcicdregister.azurecr.io": {}
            },
            "credsStore": "desktop",
            "currentContext": "default",
            "plugins": {
                "-x-cli-hints": {
                    "enabled": "true"
                }
            }
        }

