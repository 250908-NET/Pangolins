# Pangolins - Pangolivia

## Building the CD pipeline (Christian Brewer)

Tips for deploying a docker CD pipeline:
- Follow this guide on making a service principle: https://learn.microsoft.com/en-us/entra/identity-platform/howto-create-service-principal-portal
- When retrieving information for client credentials or OIDC, make sure to use ***Application (client) id*** that is given by the Registered App
- In the Container Registry, give the Application access to *AcrPush* and *AcrPull*

