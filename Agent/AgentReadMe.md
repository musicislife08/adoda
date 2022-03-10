# Agent Build

The first step in deploying the dynamic agents is to create an agent image to run.  
All the steps are setup in the docker file.  Please edit the dockerfile if you need to modify the installed software.

By Default the dockerfile based on ubuntu 20.04 in this repo installs the following software:

- curl
- git
- gpg
- gpg-agent
- iputils-ping
- iptables
- jq
- netcat
- openssh-client
- sudo
- supervisor
- unzip
- wget
- zip
- docker
- python3
- python3-pip
- python3-setuptools
- powershell
- dotnet-sdk-3.1
- dotnet-sdk-5.0
- dotnet-sdk-6.0
- nodejs 14
- azure-cli
- newman
- kubectl
- helm
- krew
- kubepug
- terraform
- packer
- gcloud
- aws-cli

## Azure Pipeline Setup

### Pipeline Requirements

This pipeline uses a third party task to replace values in files you will need to install

- [Replace Tokens](https://marketplace.visualstudio.com/items?itemName=qetza.replacetokens)