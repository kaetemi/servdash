# servdash
Service Dashboard

![Screenshot](screenshot.png)

# Example

```
[]
Title=Web

[access_log]
Terminal=1
Title=Access Log
LaunchCmd=powershell
LaunchArgs=Get-Content -Path ".\nginx\logs\access.log" -Wait

[error_log]
Terminal=1
Title=Error Log
LaunchCmd=powershell
LaunchArgs=Get-Content -Path ".\nginx\logs\error.log" -Wait

[mariadb]
Terminal=1
Title=MariaDB
WorkingDirectory=.\mariadb
LaunchCmd=.\bin\mysqld
LaunchArgs=--no-defaults --port=9040 --datadir=..\..\pipeline\shard_dev\mysql --character-set-server=utf8mb4 --collation-server=utf8mb4_general_ci --console --standalone
ShutdownCmd=.\bin\mysqladmin
ShutdownArgs=-u root -P 9040 shutdown

[php]
Terminal=1
Title=PHP-CGI
WorkingDirectory=.\php
LaunchCmd=.\php-cgi
LaunchArgs=-b 127.0.0.1:9041

[nginx]
Terminal=1
Title=Nginx
WorkingDirectory=.\nginx
LaunchCmd=.\nginx
ShutdownCmd=.\nginx
ShutdownArgs=-s stop
```

# Notices

Some icons by Yusuke Kamiyamane. Licensed under a Creative Commons Attribution 3.0 License.
