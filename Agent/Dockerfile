FROM ubuntu:20.04
# To make it easier for build and release pipelines to run apt-get,
# configure apt to not require confirmation (assume the -y argument by default)
ENV DEBIAN_FRONTEND=noninteractive
RUN echo "APT::Get::Assume-Yes \"true\";" > /etc/apt/apt.conf.d/90assumeyes
WORKDIR /tmp
RUN apt-get update && apt-get install --no-install-recommends \
    apt-transport-https \
    apt-utils \
    ca-certificates \
    curl \
    git \
    gpg \
    gpg-agent \
    iputils-ping \
    iptables \
    jq \
    libcurl4 \
    libunwind8 \
    libssl1.0 \
    netcat \
    openssh-client \
    slirp4netns \
    software-properties-common \
    sudo \
    supervisor \
    unzip \
    wget \
    zip

# Docker Install Settings
ENV DOCKER_CHANNEL=stable \
	DOCKER_VERSION=20.10.8 \
	DOCKER_COMPOSE_VERSION=1.29.2 \
	DEBUG=false

# Docker installation
RUN set -eux; \
	\
	arch="$(uname --m)"; \
	case "$arch" in \
        # amd64
		x86_64) dockerArch='x86_64' ;; \
        # arm32v6
		armhf) dockerArch='armel' ;; \
        # arm32v7
		armv7) dockerArch='armhf' ;; \
        # arm64v8
		aarch64) dockerArch='aarch64' ;; \
		*) echo >&2 "error: unsupported architecture ($arch)"; exit 1 ;;\
	esac; \
	\
	if ! wget -q -O docker.tgz "https://download.docker.com/linux/static/${DOCKER_CHANNEL}/${dockerArch}/docker-${DOCKER_VERSION}.tgz"; then \
		echo >&2 "error: failed to download 'docker-${DOCKER_VERSION}' from '${DOCKER_CHANNEL}' for '${dockerArch}'"; \
		exit 1; \
	fi; \
	\
	tar --extract \
		--file docker.tgz \
		--strip-components 1 \
		--directory /usr/local/bin/ \
	; \
	rm docker.tgz; \
	\
	dockerd --version; \
	docker --version
COPY modprobe /usr/local/bin/
COPY supervisor/ /etc/supervisor/conf.d/
COPY logger.sh /opt/bash-utils/logger.sh
RUN chmod +x /usr/local/bin/modprobe
VOLUME /var/lib/docker
RUN curl -L "https://github.com/docker/compose/releases/download/${DOCKER_COMPOSE_VERSION}/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose \
	&& chmod +x /usr/local/bin/docker-compose

RUN add-apt-repository universe
RUN wget -q https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb && dpkg -i packages-microsoft-prod.deb
RUN curl -sL https://deb.nodesource.com/setup_14.x | bash
#RUN echo "deb https://download.opensuse.org/repositories/devel:/kubic:/libcontainers:/stable/xUbuntu_20.04/ /" > /etc/apt/sources.list.d/kubic.list
#RUN curl -L https://download.opensuse.org/repositories/devel:/kubic:/libcontainers:/stable/xUbuntu_20.04/Release.key | sudo apt-key add -
RUN apt-get update && apt-get install --no-install-recommends \
  python3 python3-pip python3-setuptools \
  powershell \
  dotnet-sdk-3.1 dotnet-sdk-5.0 dotnet-sdk-6.0 \
  nodejs

RUN curl -LsS https://aka.ms/InstallAzureCLIDeb | bash

# Install NPM Packages
RUN npm install && npm install --no-package-lock
RUN npm install -g newman

# Install Kubectl and Helm
RUN curl -LO https://storage.googleapis.com/kubernetes-release/release/$(curl -s https://storage.googleapis.com/kubernetes-release/release/stable.txt)/bin/linux/amd64/kubectl
RUN chmod +x ./kubectl && mv ./kubectl /usr/local/bin/
RUN curl -L https://raw.githubusercontent.com/helm/helm/master/scripts/get-helm-3 | bash

# Install Terraform
RUN curl -o terraform.zip -LO https://releases.hashicorp.com/terraform/$(curl -s https://checkpoint-api.hashicorp.com/v1/check/terraform | jq -r -M '.current_version')/terraform_$(curl -s https://checkpoint-api.hashicorp.com/v1/check/terraform | jq -r -M '.current_version')_linux_amd64.zip
RUN unzip terraform.zip && mv terraform /usr/local/bin/

# Install Packer
RUN curl -o packer.zip -LO https://releases.hashicorp.com/packer/$(curl -s https://checkpoint-api.hashicorp.com/v1/check/packer | jq -r -M '.current_version')/packer_$(curl -s https://checkpoint-api.hashicorp.com/v1/check/packer | jq -r -M '.current_version')_linux_amd64.zip
RUN unzip packer.zip && mv packer /usr/local/bin/

# Install Google SDK
ENV CLOUDSDK_INSTALL_DIR /usr/local/gcloud/
RUN curl -sSL https://sdk.cloud.google.com | bash
ENV PATH $PATH:/usr/local/gcloud/google-cloud-sdk/bin

# Install AWS Cli
RUN pip3 install awscli

# Cleanup Setup Folder
RUN rm -rf /tmp/*

# Cleanup Packages
RUN apt-get autoremove --purge
RUN apt-get clean

# Install ADO Agent
ARG TARGETARCH=amd64
WORKDIR /azp
RUN AGENT_VERSION=$(curl -s https://api.github.com/repos/Microsoft/azure-pipelines-agent/releases/latest | jq -r '.tag_name' | cut -c2-) && \
    if [ "$TARGETARCH" = "amd64" ]; then \
      AZP_AGENTPACKAGE_URL=https://vstsagentpackage.azureedge.net/agent/${AGENT_VERSION}/vsts-agent-linux-x64-${AGENT_VERSION}.tar.gz; \
    else \
      AZP_AGENTPACKAGE_URL=https://vstsagentpackage.azureedge.net/agent/${AGENT_VERSION}/vsts-agent-linux-${TARGETARCH}-${AGENT_VERSION}.tar.gz; \
    fi; \
    curl -LsS "$AZP_AGENTPACKAGE_URL" | tar -xz

COPY ./start.sh .
RUN chmod +x start.sh

ENTRYPOINT [ "./start.sh" ]