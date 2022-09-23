# Example API - Feature Folder, DDD, .Net Core 6 WebAPI sample 
This is a simple sample project consisting of an Asp.Net Core WebAPI.

## Technologies
C# 10 - .Net 6 - Asp.Net - Dapper - MediatR - XUnit - PostgreSQL

## Backend Design

### Feature Folders
Each main feature is seperated into seperate folders. All releveant classes (besides services which are common between all features) are put in the same cohesive namespace.

### DDD

Entitiy properties are exposed in a protected fashion, with only getters and read-only collections. Properties can only be changed via explicit methods which ensure that the entitiy's invariants are maintained.

Domain Events are readonly records of actions which took place within the system. Domain Events are published when any action executed which changes the state of an entity.

### Optimistic Concurrency

All queries to a specific resource will include the aggregate version in the ETag header. When a POST or PUT request is recieved, and the ETag header is set in the request, if the header value does not match the current entity version then the server will respond with a 412 status code.

## Repository

### Workflows

#### Test
When any pull request is created a GitHub action is run which will build the project and run tests.

#### Deploy
When changes are pushed to the `master` branch a Github action is run which will ubild the project, run tests, copy the project to the production server, rebuild the docker container and deploy the updated image.
