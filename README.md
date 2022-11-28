# DbWriter
This is a tiny application to write database structure to file in an organized way ready for storing in version control.

## Why it exists?
- SQL Server developers modify scripting behavior in every single version of SQL Server and sometimes even between minor versions.

- If your company has SQL server SQL files stored in version control, you will have a really hard time to figure out which changes are made by your developers and which ones are made by Microsoft developers, especially when a database contains thousands of tables, procedures, views...

- We noticed sometimes even scripting options are being changed between version and those changes have to be set again.

- Process of scripting from SSMS is seriously slow. For some strange reason, even though this application uses same SQL server object repository to read properties, it is up to 25 times faster. 

- It organizes tables, procedures, views ... to different folders under their schema, and does not include type of the file in its name. This means when you open the file in SSMS, you can see actual name of the procedure, view, function in its title instead of just schema name and file type.

- It makes comparison of procedures and finding the ones that are changed easier. When using `GIT` `GUI` (such as `TortoiseGit`) a red overlay is shown on top of the file icon.
- To deploy changed files again using `GIT`, open commit interface, select modified files and then drag them out to copy them to a new folder.

## Limitations
Since this application is implemented to be used with C# projects under visual studio, and is part of a toolset for generating n-Tier applications:

1- It includes column, parameter and object descriptions for procedures and tables, even if they are empty.
2- User types are not supported by .NET thus all of them are changed to actual system types (unless it is not possible)
3- Table types as input and output of procedures and functions will be listed as user types, but user types are not scripted.
4- Database location, users, properties are not scripted. This was a decision I had to make due to the way these users were created by DBA in our organization (3 out of 4 in my previous jobs).
5- No `DROP` scripted for tables, this is to protect data from accidental deletion.

## NOTE
This application stores database connection string in registry with encrypted password, however if extra security is required, these settings can be removed using registry file included with release.
