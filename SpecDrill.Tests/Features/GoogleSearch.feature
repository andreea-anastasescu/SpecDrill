Feature: GoogleSearch
	In order to understand this framework's name
	As a curious tester / developer
	I want to read Wikipedia page on specific search term

@mytag
Scenario: Find wiki entry for searched keyword
	Given I have entered "drill wiki" into Google search
	When I press Search button
	Then You should get a "Wikipedia" entry in search results
	Then ReOpen search page

