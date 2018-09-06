------------------------------
## Recreate Create XPO Objects

	1) In PosOn.cs, change bool to true
		 bool xpoCreateDatabaseAndSchema = true;
		 bool xpoCreateDatabaseObjectsWithFixtures = true;
	
	2) Drop Database. ex: poson
	
	3) Run PosOn, wait until it recreate database, close PosOn
	
	4) In PosOn.cs, change bool to false
		 bool xpoCreateDatabaseAndSchema = false;
		 bool xpoCreateDatabaseObjectsWithFixtures = false;
	
	5) Extract/Dump Database Scheme Only (without records)
	   to
 		 Resources\Database\MySql\databaseschema.sql
	 	 Resources\Database\MSSqlServer\databaseschema.sql
		 Resources\Database\SQLite\databaseschema.sql

	6) Drop database Again
	
	7) Run PosOn, wait until it recreate database, this time from sql scripts
	
	8) Done!

---------
## SQLite
	Optional: Remove Blank Lines
	Required: Always Remove

-------------
##MSSqlServer
	Optional: 
	Required: Always Remove

--------
## MySQL
	Optional: 
	Required: Always Remove
	
	CREATE DATABASE  IF NOT EXISTS poson /*!40100 DEFAULT CHARACTER SET utf8 */;
	USE poson;

	Search and Remove all
	AUTO_INCREMENT=xxx
