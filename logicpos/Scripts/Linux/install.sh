#!/bin/sh

#Tested with Ubuntu 18.04.3 LTS

#nano install.sh
#sudo chmod u+x install.sh

USR=$SUDO_USER

#if [ "$USR" == "" ]; then
#  echo "Execute script with 'sudo' command"
#  exit 0
#fi

#Info
INFO_SOFTWARE="LogicPOS Ubuntu 18.04.3 LTS Install Script v1.0"

#Files
FILE_INSTALL=logicpos_linux.zip
FILE_THEME=plymouth_theme_logicpos.zip
#Urls
URL_DOWNLOAD_SERVER=http://box.logicpulse.com/files/latest
URL_FILE_INSTALL=$URL_DOWNLOAD_SERVER/$FILE_INSTALL
URL_FILE_THEME=$URL_DOWNLOAD_SERVER/$FILE_THEME

#Passwords
MAIN_PASSWORD_ROOT=Logicpos#2019

#Network Config
NETWORK_ADDRESS=192.168.1.99
NETWORK_NETWORK=192.168.1.0
NETWORK_GATEWAY=192.168.1.1
NETWORK_BROADCAST=192.168.1.255
NETWORK_NETMASK=255.255.255.0
NETWORK_NAMESERVERS=8.8.8.8

#Notes ------------------------------------------------------------------------------------------------------------------

clear
	
#Mysql Root Password 

echo $INFO_SOFTWARE
echo ""
echo "Notes:"
echo "Linux is case sensitive because 'a' and 'A' are different, choose the options exactly as they appear."
echo "When Ask for Configuring gdm, choose lightdm"
echo ""

read -p "Press [Enter] key to continue..." ENTER
echo ""

#Request Inputs from user ------------------------------------------------------------------------------------------------------------------

read -r -p "Configure network card to $NETWORK_ADDRESS from script parameters(y) or Skip it(N) ? [y/N] " NETWORK_USE
NETWORK_USE=$(echo "$NETWORK_USE" | tr '[:upper:]' '[:lower:]')
#Network
if [ "$NETWORK_USE" = "y" ] ; then
  echo "Target Network Script Configuration Parameters"
	echo "NETWORK_ADDRESS: $NETWORK_ADDRESS"
	echo "NETWORK_NETWORK: $NETWORK_NETWORK"
	echo "NETWORK_GATEWAY: $NETWORK_GATEWAY"
	echo "NETWORK_BROADCAST: $NETWORK_BROADCAST"
	echo "NETWORK_NETMASK: $NETWORK_NETMASK"
	echo "NETWORK_NAMESERVERS: $NETWORK_NAMESERVERS"
	echo ""
	read -p "Press [Enter] key to continue..." ENTER
else
  echo "Skipped Network Configuration from Script"
fi

echo ""
read -r -p "Choose App Operation Mode: [Restaurant] | Parking | Bakery | Butchery | Cafe | ClothingStore | HardwareStore | SeafoodShop | ShoeStore | BackOfficeMode " APP_OPERATION_MODE
if [ -z "$APP_OPERATION_MODE" ]
then
    APP_OPERATION_MODE="Restaurant"
fi
echo App Operation Mode : $APP_OPERATION_MODE

echo ""
read -r -p "Choose Database Type: [MySql] | SQLite " DATABASE_TYPE 
if [ -z "$DATABASE_TYPE" ]
then
    DATABASE_TYPE="MySql"
fi
echo Database Type : $DATABASE_TYPE

echo ""
read -r -p "Create Demo Database ? [true] | false " USE_DATABASE_DEMO
if [ -z "$USE_DATABASE_DEMO" ]
then
    USE_DATABASE_DEMO="true"
fi
echo Create Demo Database : $USE_DATABASE_DEMO


if [ $DATABASE_TYPE = "MySql" ]; then
    echo "When Ask for New password for the MySQL root user: use $MAIN_PASSWORD_ROOT"
	echo ""
	DB_CONNECTION="XpoProvider=MySql;server=localhost;user id=root;password=$MAIN_PASSWORD_ROOT;database=logicposdb;persist security info=true;CharSet=utf8;"
else
    DB_CONNECTION="XpoProvider=SQLite;Data Source=logicpos.db"
fi

echo ""
read -r -p "Install Additional Programs - Skype and Anydesk (y) or Skip it(N) ? [y/N]  " ADDITIONAL	
ADDITIONAL=$(echo "$ADDITIONAL" | tr '[:upper:]' '[:lower:]')
if [ "$ADDITIONAL" = "y" ] ; then
	echo "Install Additional Programs"
else
  echo "Skipped Install Additional Programs "
fi

echo ""
read -r -p "Setup logicpos in KiosK Mode(y) or Skip it(N) ? [y/N]  " KIOSKMODE	
KIOSKMODE=$(echo "$KIOSKMODE" | tr '[:upper:]' '[:lower:]')
if [ "$KIOSKMODE" = "y" ] ; then
	echo "Setup KiosK Mode"

else
  echo "Skipped KiosK Mode"
fi

echo ""
read -r -p "Define Language: [EN] | PT | AO | MZ | BR | ES | FR " LANGUAGE	
if [ -z "$LANGUAGE" ]
then
    LANGUAGE="EN"
fi

if [ $LANGUAGE = "EN" ]; then
    CULTURE_LANG="en-US"
	SYSTEM_COUNTRY="bb5dfffe-5bcc-4a8c-8c3c-f693413c24e4"
	SYSTEM_COUNTRY_CODE2="US"
	SYSTEM_CURRENCY="28d692ad-0083-11e4-96ce-00ff2353398c"
elif [ $LANGUAGE = "PT" ]; then
    CULTURE_LANG="pt-PT"
	SYSTEM_COUNTRY="e7e8c325-a0d4-4908-b148-508ed750676a"
	SYSTEM_COUNTRY_CODE2="PT"
	SYSTEM_CURRENCY="28dd2a3a-0083-11e4-96ce-00ff2353398c"
elif [ $LANGUAGE = "AO" ]; then
    CULTURE_LANG="pt-AO"
	SYSTEM_COUNTRY="9655510a-ff58-461e-9719-c037058f10ed"
	SYSTEM_COUNTRY_CODE2="AO"
	SYSTEM_CURRENCY="28da9212-3423-11e4-96ce-00ff2353398c"
elif [ $LANGUAGE = "MZ" ]; then
    CULTURE_LANG="pt-MZ"
	SYSTEM_COUNTRY="16fcd7f2-e885-48d8-9f8e-9d224cc36f32"
	SYSTEM_COUNTRY_CODE2="MZ"
	SYSTEM_CURRENCY="28d16be0-0083-11e4-96ce-00ff2353398c"
elif [ $LANGUAGE = "BR" ]; then
    CULTURE_LANG="pt-BR"
	SYSTEM_COUNTRY="425c47ae-ad2f-4f11-865d-b564afc0949a"
	SYSTEM_COUNTRY_CODE2="BR"
	SYSTEM_CURRENCY="28db4a4d-0083-11e4-96ce-00ff2353398c"
elif [ $LANGUAGE = "ES" ]; then
    CULTURE_LANG="es-ES"
	SYSTEM_COUNTRY="1ed82771-b814-44e7-832e-2cb8e2683563"
	SYSTEM_COUNTRY_CODE2="ES"
	SYSTEM_CURRENCY="28dd2a3a-0083-11e4-96ce-00ff2353398c"
else
    CULTURE_LANG="fr-FR"
	SYSTEM_COUNTRY="146da2e5-3659-47fb-ab78-7b74bc23ac07"
	SYSTEM_COUNTRY_CODE2="FR"
	SYSTEM_CURRENCY="28dd2a3a-0083-11e4-96ce-00ff2353398c"
fi

echo ""
echo Language : $LANGUAGE
echo Culture Language : $CULTURE_LANG
echo System Country : $SYSTEM_COUNTRY
echo System Country Code2 : $SYSTEM_COUNTRY_CODE2
echo System Currency : $SYSTEM_CURRENCY
echo DBConnection : $DB_CONNECTION
echo ""
read -p "Press [Enter] key to continue..." ENTER


#Packages ------------------------------------------------------------------------------------------------------------------

echo "Update Package Repository"
sudo apt-get update -y --force-yes

echo "Install SSH Server"
sudo apt-get install openssh-server -y --force-yes

echo "Install Packages"
sudo apt-get install ssh mc screen gnome-tweak-tool samba xinput-calibrator unclutter cups -y --force-yes

echo "Install Language Packs"
sudo apt-get install language-pack-pt language-pack-gnome-pt language-pack-pt-base language-pack-gnome-pt-base -y --force-yes

echo "Install Mono"
sudo apt install gnupg ca-certificates
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
echo "deb https://download.mono-project.com/repo/ubuntu stable-bionic main" | sudo tee /etc/apt/sources.list.d/mono-official-stable.list
sudo apt update
sudo apt-get install mono-complete -y

echo "Install GTK libraries"
sudo apt-get install libgdiplus gtk-sharp2 -y
sudo apt-get install libcanberra-gtk-module libcanberra-gtk3-module -y

echo "Install Theme Engines"
sudo apt-get install gtk2-engines-nodoka -y

if [ "$KIOSKMODE" = "y" ]; then
	echo "Install LXDE"
	sudo apt-get install lxde -y
fi

sudo apt install plymouth-themes -y

echo "Auto Remove"
sudo apt-get autoremove -y --force-yes

if [ $DATABASE_TYPE = "MySql" ]; then
	echo "Install MySQL"
	sudo apt-get install mysql-server -y
	sudo service mysql start
	#sudo mysql_secure_installation
	sudo ufw allow mysql
	sudo apt-get install mysql-workbench -y
	sudo mysql -uroot -e"ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY '$MAIN_PASSWORD_ROOT';"
	sudo service mysql restart
else
	echo "Install SQlite"
    	sudo apt-get install sqlite3 -y
	sudo apt-get install sqlitebrowser -y
fi

read -p "Press [Enter] key to continue..." ENTER

#Updates ---------------------------------------------------------------------------------------------------------------------

echo "Disable Automatic Updates via Console";
sudo sed -e '/"${distro_id}:${distro_codename}-security";/ s/^\/*/\/\//' -i /etc/apt/apt.conf.d/50unattended-upgrades
sudo sed -e '/"${distro_id}:${distro_codename}-updates";/ s/^\/*/\/\//' -i /etc/apt/apt.conf.d/50unattended-upgrades
sudo sed -e '/"${distro_id}:${distro_codename}-proposed";/ s/^\/*/\/\//' -i /etc/apt/apt.conf.d/50unattended-upgrades
sudo sed -e '/"${distro_id}:${distro_codename}-backports";/ s/^\/*/\/\//' -i /etc/apt/apt.conf.d/50unattended-upgrades

read -p "Press [Enter] key to continue..." ENTER


#Downloads ---------------------------------------------------------------------------------------------------------------------

echo "Download Files"

[ -f $FILE_INSTALL ] && echo "Skipped downloading $FILE_INSTALL" || echo "Downloading $FILE_INSTALL" && wget $URL_FILE_INSTALL
[ -f $FILE_THEME ] && echo "Skipped downloading $FILE_THEME" || echo "Downloading $FILE_THEME" &&wget $URL_FILE_THEME

read -p "Press [Enter] key to continue..." ENTER


#Users and Groups---------------------------------------------------------------------------------------------------------------------

echo "Add Users"

echo "Add Printer Group to Users"
sudo usermod -a -G lp logicpulse
#manage cups
sudo usermod -a -G lpadmin logicpulse

read -p "Press [Enter] key to continue..." ENTER

#Configure Network-------------------------------------------------------------------------------------------------------------------

if [ "$NETWORK_USE" = "y" ]
then
	echo "Config Network"
	sudo mv /etc/network/interfaces /etc/network/interfaces_org
	sudo touch /etc/network/interfaces
	sudo chmod 777 /etc/network/interfaces
	sudo echo "# eth0" >> /etc/network/interfaces
	sudo echo "auto eth0" >> /etc/network/interfaces
	sudo echo "iface eth0 inet static" >> /etc/network/interfaces
	sudo echo "address   $NETWORK_ADDRESS" >> /etc/network/interfaces
	sudo echo "network   $NETWORK_NETWORK" >> /etc/network/interfaces
	sudo echo "gateway   $NETWORK_GATEWAY" >> /etc/network/interfaces
	sudo echo "broadcast $NETWORK_BROADCAST" >> /etc/network/interfaces
	sudo echo "netmask   $NETWORK_NETMASK" >> /etc/network/interfaces
	sudo echo "dns-nameservers $NETWORK_NAMESERVERS" >> /etc/network/interfaces
	sudo chmod 700 /etc/network/interfaces
	read -p "Press [Enter] key to continue..." ENTER
fi

#Configure Samba-------------------------------------------------------------------------------------------------------------------

echo "Config Samba"
sudo cp /etc/samba/smb.conf /etc/samba/smb.conf_ORG
sudo chmod 777 /etc/samba/smb.conf
sudo echo "" >> /etc/samba/smb.conf
sudo echo "[root]" >> /etc/samba/smb.conf
sudo echo "    comment = Root" >> /etc/samba/smb.conf
sudo echo "    path = /" >> /etc/samba/smb.conf
sudo echo "    writable = yes" >> /etc/samba/smb.conf
sudo echo "    printable = no" >> /etc/samba/smb.conf
sudo echo "    write list = root logicpulse" >> /etc/samba/smb.conf
sudo echo "    browseable = no" >> /etc/samba/smb.conf
sudo echo "" >> /etc/samba/smb.conf
sudo echo "[logicpos]" >> /etc/samba/smb.conf
sudo echo "    comment = LogicPOS" >> /etc/samba/smb.conf
sudo echo "    path = /home/logicpos/logicpos" >> /etc/samba/smb.conf
sudo echo "    writable = yes" >> /etc/samba/smb.conf
sudo echo "    printable = no" >> /etc/samba/smb.conf
sudo echo "    write list = root logicpulse logicpos" >> /etc/samba/smb.conf
sudo echo "    browseable = yes" >> /etc/samba/smb.conf
sudo chmod 700 /etc/samba/smb.conf
#Add Passwords
sudo echo -e "$MAIN_PASSWORD_ROOT\n$MAIN_PASSWORD_ROOT" | sudo smbpasswd -s -a root
sudo echo -e "$MAIN_PASSWORD_ROOT\n$MAIN_PASSWORD_ROOT" | sudo smbpasswd -s -a logicpulse
sudo /etc/init.d/smbd restart
read -p "Press [Enter] key to continue..." ENTER
sudo service smbd restart


#CUPS --------------------------------------------------------------------------------------------------------------

sudo cp /etc/cups/cupsd.conf /etc/cups/cupsd.conf_ORG
sudo cupsctl --remote-admin
sudo service cups reload


#Software ---------------------------------------------------------------------------------------------------------------------

if [ "$ADDITIONAL" = "y" ]
then
	echo "Install Anydesk"	
	wget https://download.anydesk.com/linux/anydesk_5.1.1-1_amd64.deb
	sudo dpkg -i anydesk_5.1.1-1_amd64.deb
	sudo apt-get install -f -y
	read -p "Press [Enter] key to continue..." ENTER

	echo "Install Skype"
	sudo apt-get install gdebi -y --force-yes
	wget https://go.skype.com/skypeforlinux-64.deb
	sudo gdebi skypeforlinux-64.deb -n
	read -p "Press [Enter] key to continue..." ENTER
fi

#LogicPOS ---------------------------------------------------------------------------------------------------------------------

echo "Install LogicPOS"

sudo mkdir /home/logicpulse/logicpos -p
sudo unzip $FILE_INSTALL -d /home/logicpulse/logicpos
sudo chown logicpulse /home/logicpulse/logicpos -R
sudo chmod 755 /home/logicpulse/logicpos/*.exe
sudo chmod 755 /home/logicpulse/logicpos/Scripts/Linux/Cron.sh
sudo chmod 755 /home/logicpulse/logicpos/Scripts/Linux/Kiosk.sh
sudo chmod 755 /home/logicpulse/logicpos/Scripts/Linux/PostInstall.sh
sudo chmod 755 /home/logicpulse/logicpos/Scripts/Linux/Run.sh

#create icons
sudo echo "[Desktop Entry]" >> /home/logicpulse/Desktop/LogicPos.desktop
sudo echo "Name=LogicPOS" >> /home/logicpulse/Desktop/LogicPos.desktop
sudo echo "Comment=Open LogicPOS" >> /home/logicpulse/Desktop/LogicPos.desktop
sudo echo "Icon=/home/logicpulse/logicpos/Images/logicpos.ico" >> /home/logicpulse/Desktop/LogicPos.desktop
sudo echo "Exec=/home/logicpulse/logicpos/Scripts/Linux/Run.sh" >> /home/logicpulse/Desktop/LogicPos.desktop
sudo echo "Terminal=true" >> /home/logicpulse/Desktop/LogicPos.desktop
sudo echo "Type=Application" >> /home/logicpulse/Desktop/LogicPos.desktop
sudo chmod +x /home/logicpulse/Desktop/LogicPos.desktop
sudo chown logicpulse /home/logicpulse/Desktop/LogicPos.desktop

#sudo echo "[Desktop Entry]" >> /home/logicpulse/Desktop/LogicPosPostInstall.desktop
#sudo echo "Name=LogicPOS PostInstall" >> /home/logicpulse/Desktop/LogicPosPostInstall.desktop
#sudo echo "Comment=Open LogicPOS Post Install Script" >> /home/logicpulse/Desktop/LogicPosPostInstall.desktop
#sudo echo "Icon=utilities-terminal" >> /home/logicpulse/Desktop/LogicPosPostInstall.desktop
#sudo echo "Exec=/home/logicpulse/logicpos/Scripts/Linux/PostInstall.sh" >> /home/logicpulse/Desktop/LogicPosPostInstall.desktop
#sudo echo "Terminal=false" >> /home/logicpulse/Desktop/LogicPosPostInstall.desktop
#sudo echo "Type=Application" >> /home/logicpulse/Desktop/LogicPosPostInstall.desktop
#sudo chmod +x /home/logicpulse/Desktop/LogicPosPostInstall.desktop
#sudo chown logicpulse /home/logicpulse/Desktop/LogicPosPostInstall.desktop

sudo sed -i -e "s/APP_OPERATIONMODETOKEN/$APP_OPERATION_MODE/g" /home/logicpulse/logicpos/logicpos.exe.config
sudo sed -i -e "s/DB_CONNECTION/$DB_CONNECTION/g" /home/logicpulse/logicpos/logicpos.exe.config
sudo sed -i -e "s/DB_PROVIDER/$DATABASE_TYPE/g" /home/logicpulse/logicpos/logicpos.exe.config
sudo sed -i -e "s/DB_DEMODATA/$USE_DATABASE_DEMO/g" /home/logicpulse/logicpos/logicpos.exe.config
sudo sed -i -e "s/CULTURE_LANG/$CULTURE_LANG/g" /home/logicpulse/logicpos/logicpos.exe.config
sudo sed -i -e "s/SYSTEM_CURRENCY/$SYSTEM_CURRENCY/g" /home/logicpulse/logicpos/logicpos.exe.config
sudo sed -i -e "s/SYSTEM_COUNTRY/$SYSTEM_COUNTRY/g" /home/logicpulse/logicpos/logicpos.exe.config
sudo sed -i -e "s/SYSTEM_COUNTRY_CODE2/$SYSTEM_COUNTRY_CODE2/g" /home/logicpulse/logicpos/logicpos.exe.config

read -p "Press [Enter] key to continue..." ENTER

#Kiosk Mode ---------------------------------------------------------------------------------------------------------------------

if [ "$KIOSKMODE" = "y" ]
then

	echo "Setup logicpos KiosK Mode"

	sudo touch /usr/share/xsessions/logicpos.desktop
	sudo chmod 777 /usr/share/xsessions/logicpos.desktop
	sudo echo "[Desktop Entry]" >> /usr/share/xsessions/logicpos.desktop
	sudo echo "Encoding=UTF-8" >> /usr/share/xsessions/logicpos.desktop
	sudo echo "Name=logicpos Kiosk Mode" >> /usr/share/xsessions/logicpos.desktop
	sudo echo "Comment=logicpos" >> /usr/share/xsessions/logicpos.desktop
	sudo echo "Exec=/home/logicpulse/logicpos/Scripts/Linux/Kiosk.sh" >> /usr/share/xsessions/logicpos.desktop
	sudo echo "Icon=/home/logicpulse/logicpos/Images/logicpos.ico" >> /usr/share/xsessions/logicpos.desktop
	sudo echo "Type=Application" >> /usr/share/xsessions/logicpos.desktop
	sudo chmod 644 /usr/share/xsessions/logicpos.desktop

#auto boot "Kiosk Mode" session with
#sudo mv /etc/lightdm/lightdm.conf /etc/lightdm/lightdm.conf_ORG
#sudo touch /etc/lightdm/lightdm.conf
#sudo chmod 777 /etc/lightdm/lightdm.conf
#sudo echo "[SeatDefaults]" >> /etc/lightdm/lightdm.conf
#sudo echo "autologin-user=salestrack" >> /etc/lightdm/lightdm.conf
#sudo echo "autologin-user-timeout=25" >> /etc/lightdm/lightdm.conf
#sudo echo "user-session=Sales.Track Kiosk Mode" >> /etc/lightdm/lightdm.conf
#sudo echo "greeter-session=unity-greeter" >> /etc/lightdm/lightdm.conf
#sudo echo "allow-guest=false" >> /etc/lightdm/lightdm.conf
#sudo chmod 700 /etc/lightdm/lightdm.conf

	read -p "Press [Enter] key to continue..." ENTER

	#Plymouth Theme ---------------------------------------------------------------------------------------------------------------------

	echo "Setup Plymouth Theme. Select logicpos.plymouth theme"

	#sudo tar ixzf $FILE_THEME -C /usr/share/plymouth/themes/
	#sudo update-alternatives --install /usr/share/plymouth/themes/default.plymouth default.plymouth /usr/share/plymouth/themes/logicpos/logicpos.plymouth 100
	#sudo update-alternatives --config default.plymouth
	#sudo update-initramfs -u 
	
	
	sudo unzip $FILE_INSTALL -d /usr/share/plymouth/themes/
	sudo update-alternatives --install /usr/share/plymouth/themes/default.plymouth default.plymouth /usr/share/plymouth/themes/logicpos/logicpos.plymouth 100
	sudo update-alternatives --config default.plymouth
	sudo update-initramfs -u 

	read -p "Press [Enter] key to continue..." ENTER
fi

#Post Install --------------------------------------------------------------------------------------------------------------

echo "Configure Firewall"

sudo ufw allow ssh/tcp
sudo ufw allow mysql
sudo ufw allow samba
sudo ufw allow cups
sudo ufw logging on
sudo ufw enable
sudo ufw status

read -p "Press [Enter] key to continue..." ENTER

#Restart Network ---------------------------------------------------------------------------------------------------------------------

if [ "$NETWORK_USE" = "y" ]
then
	echo "Restarting Network, network addresss: $NETWORK_ADDRESS"
	sudo ifdown eth0 && sudo ifup eth0
	read -p "Press [Enter] key to continue..." ENTER
fi

#Final Steps ---------------------------------------------------------------------------------------------------------------------

read -p "Reboot system or CTRL+C to abort" ENTER

sudo reboot





