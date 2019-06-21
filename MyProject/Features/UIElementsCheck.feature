Feature: Sample UI Elements Check
	Verify UI elements in UltimateQA

@NoData @ultimateqa_01 @ultimateqa
Scenario: UltimateQA_01_Verify Button element in UltimateQA
	Given Navigate to UltimateQA Website
	When Click on Raise Button 
	Then The UltimateQA Website homepage should be opened


@ultimateqa_02_Data @ultimateqa_02 @ultimateqa
Scenario: UltimateQA_02_Verify TextBox element in UltimateQA
	Given Navigate to UltimateQA Website
	When The user fills Name and Email address
	And The user clicks on Email me button
	Then The User gets Thank you message

@NoData @ultimateqa_03 @ultimateqa
Scenario: UltimateQA_03_Verify RadioButton and Check box element in UltimateQA
	Given Navigate to UltimateQA Website
	When The user Selects Female Radio Button
	Then The user verifies that Female Radio button got selected
    And The user select bike check box
	And The user verifies the bike checkbox got selected

@NoData @ultimateqa_04 @ultimateqa
Scenario: UltimateQA_04_Verify dropdown element in UltimateQA
	Given Navigate to UltimateQA Website
	When The user Selects opel value from dropdown
	Then The user verifies that selected dropdown value is opel
    

