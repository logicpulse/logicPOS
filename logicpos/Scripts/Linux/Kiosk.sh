#!/bin/bash

exec gnome-session &
#exec gnome-session --session=classic-gnome &
#exec gnome-session --session=gnome-fallback &

#Turn off screensaver
xset s off

#turn off power management
xset -dpms

#hide the cursor
#unclutter -grab
#unclutter -idle 0.01 -root

gsettings set org.gnome.desktop.screensaver lock-enabled false
gsettings set org.gnome.desktop.session idle-delay 0

while true; 
do 
	cd /home/logicpulse/logicpos/;
	sudo mono ./logicpos.exe; 
	sleep 5s; 
done