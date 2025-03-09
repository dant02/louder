# louder

simple web app with sound playback using LibVLC and LibVLCSharp

## linux commands to prepare environment, tried on Kubuntu 24.04

sudo snap install vlc
sudo apt install dotnet-sdk-8.0

## script to run the application

required to load LibVLC
export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:/snap/vlc/current/lib/x86_64-linux-gnu/

optional to load pulse audio plugin
export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:/snap/vlc/current/usr/lib/vlc/

dotnet app.dll
