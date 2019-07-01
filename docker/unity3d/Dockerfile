FROM ubuntu:18.04

ARG COMPONENTS=Unity,WebGL
ARG DOWNLOAD_URL=https://beta.unity3d.com/download/dad990bf2728/UnitySetup-2018.2.7f1

ENV DEBIAN_FRONTEND noninteractive
ENV DEBCONF_NONINTERACTIVE_SEEN true

RUN echo "UTC" > /etc/timezone && \
    apt-get -qq update && apt-get -qq install -y \
    wget git x11vnc xvfb libgtk-3-0 libgtk-3-dev \
    imagemagick curl gconf-service lib32gcc1 \
    lib32stdc++6 libasound2 libc6 libc6-i386 \
    libcairo2 libcap2 libcups2 libdbus-1-3 \
    libexpat1 libfontconfig1 libfreetype6 \
    libgcc1 libgconf-2-4 libgdk-pixbuf2.0-0 \
    libgl1-mesa-glx libglib2.0-0 libglu1-mesa \
    libgtk2.0-0 libnspr4 libnss3 libpango1.0-0 \
    libstdc++6 libx11-6 libxcomposite1 libxcursor1 \
    libxdamage1 libxext6 libxfixes3 libxi6 libxrandr2 \
    libxrender1 libxtst6 zlib1g debconf npm xdg-utils \
    lsb-release libpq5 xvfb libsoup2.4-1 libarchive13 \
    libpng-dev python-pip python3-venv python3-pip \
    && rm -rf /var/lib/apt/lists/*

RUN wget -nv ${DOWNLOAD_URL} -O UnitySetup && \
    # make executable
    chmod +x UnitySetup && \
    # agree with license
    echo y | \
    # install unity with required components
    ./UnitySetup \
    --unattended \
    --install-location=/opt/Unity \
    --verbose \
    --download-location=/tmp/unity \
    --components=$COMPONENTS && \
    # remove setup & temp files
    rm UnitySetup && \
    rm -rf /tmp/unity && \
    rm -rf /root/.local/share/Trash/*