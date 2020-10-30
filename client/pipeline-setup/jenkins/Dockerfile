FROM jenkins/jenkins:lts

USER root

RUN apt-get update && \
    apt-get -y install \
    apt-transport-https \
    ca-certificates \
    gnupg2 \
    software-properties-common
RUN curl -fsSL https://download.docker.com/linux/$(. /etc/os-release; echo "$ID")/gpg | apt-key add -
RUN add-apt-repository \
    "deb [arch=amd64] https://download.docker.com/linux/$(. /etc/os-release; echo "$ID") \
    $(lsb_release -cs) \
    stable"  
RUN apt-get update
RUN apt-get -y install docker-ce
RUN apt-get -y install python3-pip
RUN curl -sL https://deb.nodesource.com/setup_10.x  | bash -
RUN apt-get -y install nodejs
RUN pip3 install --no-input awscli --upgrade
# COPY plugins.txt /usr/share/jenkins/ref/plugins.txt
# RUN `/usr/local/bin/install-plugins.sh < /usr/share/jenkins/ref/plugins.txt` || echo "Some errors occurred during plugin installation."

USER jenkins