Feature: GoogleSearch
	In order to understand this framework's name
	As a curious tester / developer
	I want to read Wikipedia page on specific search term

@mytag
Scenario: 1.Find wiki entry for searched keyword
	Given I have entered "drill wiki" into Google search
	When I press Search button
	Then You should get a "Drill" entry in search results

@mytag
Scenario: 2.Find wiki entry for searched keyword
	Given I have entered "drill wiki" into Google search
	When I press Search button
	Then You should get a "Drill" entry in search results
