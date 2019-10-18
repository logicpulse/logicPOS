#!/bin/bash 

# Add to Cron with "crontab -e" in user user root (sudo -s)
#*/1 * * * * /home/logicpulse/logicpos/Scripts/Linux/Cron.sh

# Optional only to test Cron
#*/1 * * * * date "+\%Y-\%m-\%d \%T" >> /tmp/crontest.txt

chmod 777 /dev/usb/lp0
chmod 777 /dev/usb/lp1
chmod 777 /dev/usb/lp2
chmod 777 /dev/usb/lp3