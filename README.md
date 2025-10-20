# Pangolins - Pangolivia

## Building the CD pipeline (Christian Brewer)

My troubles started when I needed to add credentials to github actions.
There are several client ids, and the actions require a specific one.
As it turns out, I have to find an ***Application (client) id***.

But that was not all. I not only have to give myself permissions to access cloud resources, I must give the registered application access to cloud resources. Then, I must create a special ID for github.

This process is not easy to figure. It is far from intuitive. The error functions do not towards a simple solution. The correct process had me jumping several times between Microsoft Entra ID and Subscription.

But the process did work (after one last permission error with App Deployment).
I don't like to use AI for all my stuff, but I consider it be better than google. But there are some topics AI is not good at. Microsoft Azure Documentation is one of those. I did find a page that showed me to create the service principal properly and that is below.

https://learn.microsoft.com/en-us/entra/identity-platform/howto-create-service-principal-portal

