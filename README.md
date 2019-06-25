# BDD-Specflow
BDD Specflow Test Automation Framework

Uses:
•	SpecFlow 3 (Cucumber BDD)
•	Selenium (WebDriver)
•	NUnit 3.x
•	specflow-reports
•	utilises Page Object Model pattern
•	can be run using Jenkins
•	takes screenshots on failure of web tests

Background reading:
•	Getting started with Specflow: https://specflow.org/getting-started/

Installation
Software
•	Download Visual Studio 2017/2019.
•	Install Visual Studio Git provider (Visual Studio Tools for Git).
•	Ensure Chrome web browser is installed.
•	Ensure Selenium Chrome Driver (chromedriver.exe) is authorized on your PC.
Framework
•	Extract the contents of Speflow-BDD solution folder from source control.
•	Make necessary changes to projectPath and filePath in app.config for each project.
•	Build the solution. NuGet dependencies may need to be restored.
Visual Studio Solution

Solution also has data directory with the below structure:
Data/
    Test data json files
Each project has the below directory structure:
Model/
    Data models for each page
Pages/
    Selenium page objects (POM)
    
Execution
All tests are defined as Nunit test methods and can be executed directly from Visual Studio.
Tests are executed in Chrome by default. Although this is configurable, the framework has not been tested with any other browsers.
Results
Upon completion of the test run the TestResults folder will contain the following output:
SpecFlow+ Runner generates an advanced execution report for each test run. To view the report, select Tests in the Show output from field in the Output window:
Click on the link to the report file to view the report in Visual Studio:
Configuration
Configuration of dynamic run time information is defined in the App.config file for each test project.
General settings like URL, browser... etc. are defined in settings of the project.
Automated few UI cases on sample website named UltimateQA

