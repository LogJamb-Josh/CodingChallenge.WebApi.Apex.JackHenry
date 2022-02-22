# APEX Coding Challenge

## Description
Write a web api that will call an external(3rd party) service that will return past years accounting information for NYC departments.  Include unit tests, error handling, caching, dependency injection, and best practices. 

## Output Endpoints
The api should have endpoints that: 
1. return departments whose expenses meet or exceed their funding
   - "/Departments/ExpensesOverFunding"
2. return deparments whose expenses have increased over time by user specified percentage (int) and # of years (int)
   - "/Departments/ExpensesIncreased"
3. return departments whose expenses are a user specified percentage below their funding year over year.
   - "/Departments/ExpensesBelowFunding"
   
## Error Codes
- 400's
  - 404
  - Bad Request
- 200
- 500

## Unit Testing
- Use Mocking, specifically moq.

## Caching
- Use Microsoft's in memory cache.  It is a nuget package.

## Models
Note: for this challenge the json indexes of importance are: 
** (This means in the returned data, the data at index 9 is the fiscal year.) **
- 9 = fiscal year 
- 10 = dept. id
- 11 = dept. name
- 12 = funds available
- 13 = funds used
- 14 = remarks
   
## Input Endpoint
1. This is the endpoint this WebApi should hit.
   - https://mockbin.org/bin/fb525688-91a7-47da-a319-fcfc24a14001

## Contacts
- Ed Mclaughlin issues the test.
  - edmclaughlin@hotmail.com

- Ben is the contact at Apex.
  - bwoschnick@apexsystems.com


## Finally
When the project is complete, compile it.  Then locate the Bin and Object folders, zip them up, and email them to Ben.