#!/bin/bash

xset s off
xset -dpms
gsettings set org.gnome.desktop.screensaver lock-enabled false
gsettings set org.gnome.desktop.session idle-delay 0

cd /home/logicpos/logicpos
./logicpos.exe
