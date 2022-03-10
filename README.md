# Introduction 

Builds and deploys the Azure Devops Build Agent Controller that controls the build agent pods in aks

# Overview

This Project is heavily based on [this](https://github.com/cloudoven/azdo-k8s-agents) project by cloudoven
but refactored for better error handling, resource cleanup and updated to a newer codebase however the architecture remains the same

## Controller

The controller operator is the heart of this project.  
It connects to both azure devops and kubernetes API's to dynamically spin up new agents and clean up resources after jobs finish

## Agent & Agent Spec

The image the controller uses to spin up new pods is set by the [agent-spec.yaml file](Agent/agent-spec.yaml).
You will need to update this file with your image label and repository.
This was built for azure devops and azure kubernetes so we use the azure container registry to host the images

# Installation

Step 1: Build and install the agent spec
- [Install Instructions](Agent/AgentReadMe.md)

Step 2: Build and install the controller

**NOTE: The agent-spec.yaml file must be installed into the `azdo-agents` namespace before you can run the controller or it will restart over and over until its there**

- [Install Instructions](AgentWorker/AgentWorkerReadMe.md)



 
