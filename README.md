# Keylogger Project

## Introduction
This project is developing a keylogger api to record every input key from the user keyboard. The input key will be stored in a database with recorded date time. 
While recording the key input, the word detecting function will detect and record the sensitive words at the background. User can track the time of the sensitive word recorded.
Once recorded, user can navaigate to the interface to check all the record base on date and time frame. 
The Print function allowed user to export the data in .xlsx format for further analysis. 

This project build on .net core framework and Mssql as the database. 
For the user interface, it is built with Html, CSS, Jquery and Ajax for dynamic loading content. 

The library used in this project includes:<br/>

Hangfire - to create recurring job that runs on interval.<br/>
Razor Page - user interface.<br/>
Entity Framework Core - data mapping for database.<br/>
ClosedXML - to convert and generate data into xlsx. format file<br/>


![alt text](https://github.com/yihanglim/KeyLoggerApi/blob/9ace863d0d2a28731ba0bb1bdffeb70a1ea87836/KeyloggerHomePage.JPG)

Button function on the front page<br/>

**"Word List"**: Navigate to the page which store the list of words to be detected when logging.<br/>

**"Detected Word"**: Navigate to the page which store the list of detected words during logging.<br/>

**"Start Logging"**: Run a POST API to start the key logging function. The keylogging function will record the user input key on a 15 seconds interval.<br/>

**"Stop Logging"**: Stop the logging function<br/>

**"Create New"**: Manually create a new keystroke record.<br/>

**"Print All"**: Export all the data in excel format file.<br/>

**"Delete All"**: Delete all the recorded data. The data will not be able to recovered once deleted.<br/>

**"Creation Date"**: The date that the data is recorded<br/>

**"Open"**: Show the data in 6 hours time frame.<br/>

**"Closee"**: Close the data show in 6 hour time frame<br/>

**"Print"**: Print the data on the specific date<br/>

### Key Logging

The program will log the user input from the local IP Address http://192.168.4.1. The program will ping the IP address and return a string if there is any key input.

This program utilise Hangfire library to ping the IP Address on a 15 seconds interval to check if there is any new key input.
When user press the Start Logging button, the program will create a recurring job with Hangfire to run the key loggin function on a interval until the program is turned off or stopeed by the user.
To check the job status, user can navigate to the Hangfire dashboard through the following link. https://localhost:44399/hangfire. Please change the port number accordingly.

Before saving data to the database, the data will be scanned through with the Word List retireve from the database. Once a matching word is found, the word will be recorded as sensitive word and saved into the detected word table.
The scanning code is using Linq method ( wordList.Where(x => data.Contains(x)).ToList(); ) to match the word and store in a List. Then the list will be saved into the DetectedWord database.

Once the data is saved, the program will call the IP http://192.168.4.1/clear to clear the recorded data to have a clean page to record new data. 

### Database  
The database used in this project is Microsoft SQL Server. The Entity Framework Core library is used as data mapping to the database.  

The table involve in this program are:
Keyloggers table - to store all the user input key storke
WordLists table - to store a list of word used to scan the key input to detect sensitive word
DetectedWords table = to store all the sensitive word. 

The data will have a unique id in Guid format and Creation datetime to store the time when the data is recorded.

### User Interface  

The user interface is built with Razor page which combine and processed client side and server side code to send to browser as html page. 
Razor page simplify the data processing and data transfer to the interface. 

The page is styled with CSS and bootstrap. 

To load the data dynamically, Ajax is used in this case so that the new data can be retireve from database without reloading the page
When a time specific data is selected, the time data is sent to the server to retrieve the data from the database and filter based on the time data.
To retrieve or save a specific data, the unique id of the data is used to retreive the data from the database.


### Data Export

ClosedXML library is used to generate the data in excel (.xlsx) format so that user can easily analyse or perform and modification on the data.
The data in excel file will be generated in 3 column.  <br/>
Keystroke - the recorded data<br/>
Date - the date when the data is recorded in format "MM/dd/yyyy"<br/>
Time - the time when the data is recorded in format "HH:mm:ss"<br/>
