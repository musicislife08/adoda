# Introduction 

Builds and deploys the Azure Devops Build Agent Controller that controls the build agent pods in aks

# Overview

This Project is heavily based on [this](https://github.com/cloudoven/azdo-k8s-agents) project by cloudoven
but refactored for better error handling, resource cleanup and updated to a newer codebase however the architecture remains the same

## Controller

The controller operator is the heart of this project.  
It connects to both azure devops and kubernetes API's to dynamically spin up new agents and clean up resources after jobs finish
