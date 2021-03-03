# Specify the unity application path
UNITY_APP_PATH=/Applications/Unity/Hub/Editor/2019.3.0a5/Unity.app/Contents/MacOS/Unity 
PROJECT_PATH=../
LOG_FILE=../Logs/build.log


echo Removing existing build files
rm -rf ../build

echo Starting build process
''$UNITY_APP_PATH'' -quit -bachmode -projectPath $PROJECT_PATH -executeMethod Buildscript.Build -logFile $LOG_FILE 
echo Ended build process
