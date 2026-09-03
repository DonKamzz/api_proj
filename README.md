RaceDay — Part 1: System Planning and Database

Module: PROG6212/w — Programming 2B
Assessment: Portfolio of Evidence, Part 1 — System Planning & Database
Project: RaceDay Event Management Platform
Technology Focus: SQL Server, REST API Planning, Database Design, GitHub Actions

1. Introduction

RaceDay is a proposed full-stack, API-driven event management platform designed specifically for the South African road running, walking, and cycling community.

Many smaller sporting events still rely on manual registration forms, paper-based participant lists, spreadsheets, WhatsApp communication, and manually captured race results. These processes can become difficult to manage as the number of participants, events, categories, and results increases.

RaceDay aims to provide a centralised digital platform where event organisers can manage their events and participants can register, enrol, and track their race history through a structured system.

The system is designed around two primary user roles:

Organisers
Participants

The platform will provide functionality for event creation, event category management, participant enrolment, route information, and race results.

Part 1 focuses exclusively on system planning and database design. No application code is implemented in this phase. The purpose of this part of the Portfolio of Evidence is to establish a clear technical foundation that can be used during application development in Part 2.

2. Problem Statement

Traditional event management processes can create several challenges for local sporting event organisers.

When registrations are handled manually, organisers may need to:

Capture participant information from paper forms.
Maintain multiple spreadsheets.
Manually check whether a participant has already registered.
Keep separate lists for different race categories.
Manually record race results.
Search through spreadsheets when participants request historical results.
Communicate route information separately from registration information.
Manually update event information when changes occur.

These processes increase the possibility of:

Duplicate registrations.
Incorrect participant information.
Lost or inconsistent records.
Data-entry errors.
Difficulty retrieving historical results.
Poor communication between organisers and participants.
Increased administrative workload.

RaceDay addresses these problems by providing a structured relational database and API-driven system where information can be stored, accessed, and managed from a central platform.

3. Proposed Solution

RaceDay will provide a centralised event management system consisting of a database, REST API, and user-facing application.

The database will store information relating to:

Users
Events
Categories
Routes
Enrolments
Results

The REST API will provide controlled access to the database and will allow the future application to perform operations such as:

User registration.
User authentication.
Viewing and updating profiles.
Creating events.
Editing events.
Deleting events.
Managing categories.
Viewing routes.
Enrolling participants.
Viewing enrolments.
Capturing race results.
Viewing historical results.

The architecture separates the database from the user interface through the API layer. This provides a more maintainable structure and allows the system to be expanded in future development phases.

4. System Objectives

The primary objective of RaceDay is to create a reliable digital platform for managing sporting events and participant information.

The specific objectives are:

4.1 Digital Event Management

Allow organisers to create, update, and remove sporting events without relying on spreadsheets or paper records.

4.2 Participant Registration

Allow participants to create accounts and maintain their personal information.

4.3 Online Enrolment

Allow participants to browse available events and enrol in an event by selecting an appropriate category.

4.4 Category Management

Allow organisers to create categories based on factors such as:

Age group.
Gender.
Distance.
Race type.
4.5 Results Management

Allow organisers to capture participant results and allow participants to view their own results history.

4.6 Route Information

Store route-related information associated with events so that participants can access information that assists them in preparing for race day.

4.7 Data Integrity

Ensure that relationships between users, events, categories, enrolments, routes, and results are maintained using database constraints.

4.8 Scalability

Create a database structure that can support additional events, participants, categories, routes, and results as the platform grows.

5. System Scope
5.1 Included in the System

The initial RaceDay design includes:

User account registration.
User role management.
Authentication planning.
User profile management.
Event management.
Category management.
Route information.
Participant enrolment.
Race result management.
Historical result viewing.
Relational database storage.
REST API endpoint planning.
5.2 Outside the Current Scope

The following functionality is not implemented in Part 1:

Front-end application development.
Mobile application development.
Actual REST API implementation.
Payment processing.
Online ticket/payment integration.
Live GPS participant tracking.
Automatic timing hardware integration.
Social media integration.
Email/SMS notification implementation.
Live weather API implementation.

These features could be considered during future development phases.
