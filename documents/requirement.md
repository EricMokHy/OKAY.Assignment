# Tech Stack
ASP.NET MVC
ASP.NET Identity Framework
SQL Server

## Test Environment
SQL Server In-Memory

# Tables
## Properties
field | type | description
-- | -- | -- 
Id | int | PK
Name | string | max length 200 not null
Bedroom | int | null
IsAvailable | bit | not null, default 0
LeasePrice | decimal(10, 2) | not null, default 0
UserId | string | not null, reference `ApplicationUser.Id`

field | type | description
-- | -- | -- 
Id | int | PK
PropertyId | int | not null, reference `Properties.Id`
UserId | string | not null, reference `ApplicationUser.Id`
TransactionDate | dateTime | not null, default `getdate()`

> Notes:  
> To simplify the assignment, I put the result relationship between Property and User to be one to many. A property will belong to only one user.  

# Requirements
## Extending Identity Model
- add `bool IsAministrator` to identity model

## Registration Page
- allow user to register with email address and password
- optional to set as admin

## Login Page
- login with email and password

## Properties List Page
- authorized required
- add button to [Property Detail Page](#property-detail-page) in `create` mode
- keyword search, support property name
- result as table list
- control to redirect to [Property Detail Page](#property-detail-page) in `view` mode
- control to delete the property
- pagination
- sorting

## Property Detail Page
- authorized required
- accept id as only parameter in query string
- display all fields of the property
- allow update
- `Save` button to save changes
- disalbe `Save` button when nothing changed.
- `Cancel` button to go back to [Properties List Page](#properties-list-page)

## Transaction List Page
- authorized required
- add button to [Transaction Detail Page](#transaction-detail-page) in `create` mode.  
- keyword search, support property name.
- result as table list
- control to redirect to  [Transaction Detail Page](#transaction-detail-page) in `view` mode.  
- control to delete the transaction.
- pagination
- sorting

## Transaction Detail Page
- authorized required
- accept id as only parameter in query string
- display all field of transaction, include property
- allow update
- `Save` button to save changes
- disable `Save` button when nothing changed.
- `Cancel` button to go back to [Transaction List Page](#transaction-list-page)

## Authorization
- this logic apply to [Property List Page](#property-list-page), [Property Detail Page](#propert-detail-page), [Transaction List Page](#transaction-list-page) and [Transaction Detial Page](#transaction-detail-page).
- user can view, list, edit or delete property or transaction which is belong to him.
- admin user can view, list, edit or delete property or transaction for all owner.
- redirect to [Error Page](#error-page) if user is attempt to visit unauthorized resource.