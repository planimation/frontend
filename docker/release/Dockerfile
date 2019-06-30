FROM rakeshchoyal92/unity3d:2018 As builder

ENV UNITY_PATH='/opt/Unity/Editor/Unity'

ARG LICENSE_FILE
ARG PROJECT_PATH=/root/project
ARG BUILD_SCRIPT=Buildscript.Build
ARG LOGFILE=./logs/build.log
ARG UNITY_ARGS='-quit -batchmode -nographics'

WORKDIR /root/project
COPY . .

COPY ${LICENSE_FILE} /root/.local/share/unity3d/Unity/Unity_lic.ulf
RUN ${UNITY_PATH} ${UNITY_ARGS} ${PROJECT_PATH} -executeMethod ${BUILD_SCRIPT} -logFile

FROM nginx 
COPY --from=builder /root/project/build /usr/share/nginx/html
