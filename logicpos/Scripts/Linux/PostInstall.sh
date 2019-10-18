#!/bin/bash

echo "Disable the screen lock"
gsettings set org.gnome.desktop.screensaver lock-enabled false

#echo "Disable the screen blackout"
#gsettings set org.gnome.desktop.session idle-delay 0

rm /home/logicpulse/Desktop/LogicPosPostInstall.desktop