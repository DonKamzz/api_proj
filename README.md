RaceDay — Part 1: System Planning & Database

Module: PROG6212/w — Programming 2B
Assessment: Portfolio of Evidence — Part 1
Project: RaceDay Event Management Platform
Database: Microsoft SQL Server

1. Project Overview

RaceDay is a full-stack, API-driven event management platform designed for the South African road running, walking, and cycling community.

The system is designed to replace paper-based registration and spreadsheet-driven event management processes with a centralised digital platform.

RaceDay allows Event Organisers to manage events, categories, routes, enrolments and participant results, while Participants can register accounts, browse upcoming events, enrol in races and track their results history.

Note: Part 1 contains no application code. It focuses on system planning, database design, API planning and documentation before development begins in Part 2.


2. System Planning — 20 Key Points
#	Area	Description
1	System Overview	RaceDay is a full-stack, API-driven event management platform for running, walking and cycling events.
2	Problem Statement	Paper forms and spreadsheets can cause duplicate registrations, data-entry errors, lost information and increased administrative work.
3	Proposed Solution	RaceDay provides a centralised digital platform for managing events, participants, categories, enrolments, routes and results.
4	System Objectives	The system aims to simplify event administration, improve registration, reduce errors and provide easy access to participant and event information.
5	User Roles	The platform has two main roles: Organiser and Participant.
6	Organiser Capabilities	Organisers can create, edit and delete events, manage categories, view enrolments and capture participant results.
7	Participant Capabilities	Participants can register, log in, browse events, select categories, enrol and view their own enrolments and results.
8	Database Design	RaceDay uses a relational SQL Server database called RaceDayDB to store and manage system information.
9	Users Entity	Stores user information including UserID, name, email, password information, contact details and role.
10	Events Entity	Stores event information such as event name, description, date, location and organiser.
11	Categories Entity	Stores race categories associated with events, including different distances and age groups.
12	Routes Entity	Stores route information and supports future features such as maps, GPS information and weather information.
13	Enrolments Entity	Connects participants to events and categories. A participant can only enrol once in the same event.
14	Results Entity	Stores participant race results such as finishing time, position and result status.
15	Relationships	Primary keys, foreign keys and cardinality define relationships between Users, Events, Categories, Routes, Enrolments and Results.
16	API Planning	The API is divided into Authentication, User Profile, Events, Categories, Enrolments and Results using GET, POST, PUT and DELETE methods.
17	Security & Integrity	Planned security includes authentication, authorisation, password hashing and input validation. Database constraints protect data integrity.
18	Sample Data	The database contains 2 Organisers, 2 Participants, 3 Events, categories, routes, enrolments and sample result data.
19	CI/CD & GitHub	GitHub Actions automatically checks that the required planning documents and README are present whenever changes are pushed.
20	Testing & Future Development	The database can be tested in SSMS using SQL queries. Future versions may include payments, GPS routes, live weather, QR check-in, notifications and live results.

   3. User Roles
Organiser

Organisers are responsible for managing sporting events.

Organisers can:

Create events
Edit events
Delete events
Create and manage categories
View event enrolments
Capture participant results
Manage event route information
Participant

Participants use RaceDay to find and participate in events.

Participants can:

Register an account
Log in
Update their profile
Browse upcoming events
View event categories
View route information
Enrol in events
View their own enrolments
View their own results history

4. Database Design

The RaceDay database contains six entities:

Entity	Purpose
Users	Stores Organiser and Participant accounts
Events	Stores sporting event information
Categories	Stores race categories belonging to events
Routes	Stores route information for events
Enrolments	Connects participants to events and categories
Results	Stores participant race results

Database Relationships
Relationship	Cardinality
Users → Events	1 : Many
Events → Categories	1 : Many
Events → Routes	1 : Many
Users → Enrolments	1 : Many
Events → Enrolments	1 : Many
Categories → Enrolments	1 : Many
Enrolments → Results	1 : 0..1
