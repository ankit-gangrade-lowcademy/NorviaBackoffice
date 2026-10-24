Norvia Backoffice

Enterprise Tenant & Platform Administration System

Developed by Ankit Gangrade | Enterprise Software Architect | OutSystems Champion

Norvia Backoffice is an enterprise-grade administrative application built using ASP.NET Core (.NET 10) and Azure SQL Database.

The application provides centralized management of products, tenants, administrators, user roles, and external application provisioning.

This project is part of the AI Developer Industry Simulation Program by Lowcademy, designed to demonstrate practical enterprise software development using modern technologies and AI-assisted coding tools.

⸻

📌 Project Overview

The fictional University of Norvia operates a multi-tenant SaaS application called IntelliCampus, developed using OutSystems ODC.

As the university expands its digital ecosystem, it requires a centralized backoffice system to manage multiple applications independently of their underlying technologies.

Norvia Backoffice addresses this requirement by providing a standalone administrative platform capable of integrating with applications built using OutSystems, .NET, and other technologies.

Key Objectives

* Centralize tenant and product administration.
* Manage tenant administrators and user access.
* Support multiple enterprise applications.
* Enable integration with external systems through REST APIs.
* Provide secure role-based access control.
* Establish a scalable foundation for future applications.

⸻

🛠️ Technology Stack

Component	Technology
Application Framework	ASP.NET Core (.NET 10)
Programming Language	C#
Database	Microsoft Azure SQL Database
Cloud Platform	Microsoft Azure
Development Environment	Visual Studio Code
AI Coding Tools	OpenAI Codex / Claude Code
API Integration	REST APIs
Version Control	Git & GitHub
Security	Role-Based Access Control

⸻

🚀 Core Features

1. Product Management

* Create and manage Norvia products.
* Update product information and descriptions.
* Maintain a centralized product catalog.
* Configure product-specific access roles.

2. Product Role Management

* Define roles for individual products.
* Maintain product-specific role mappings.
* Configure role identifiers and display order.
* Support different authorization models across products.

3. Tenant Management

* Create new tenant organizations.
* Update tenant information.
* Search and manage existing tenants.
* Activate or deactivate tenants.
* Maintain centralized tenant records.

4. Tenant Administrator Management

* Create and manage tenant administrators.
* Associate administrators with tenants.
* Maintain administrator details.
* Track provisioning status across integrated products.

5. External User Provisioning

* Integrate with external application APIs.
* Provision tenant administrators in connected applications.
* Store external user identifiers.
* Track provisioning responses.
* Handle provisioning failures.

6. Backoffice User Management

Manage internal users with three access levels:

* Super Admin: Full administrative access.
* Admin: Administrative access based on assigned permissions.
* User: Restricted access based on assigned permissions.

All access restrictions must be enforced through server-side authorization.

⸻

🗄️ Database Architecture

Norvia Backoffice uses Azure SQL Database for persistent data storage.

Main Database Tables

Table	Description
NorviaProducts	Stores product information
ProductUserRole	Maintains product-specific role definitions
Tenant	Stores tenant organizations
UserExtended	Stores extended user and administrator information
BackofficeUsers	Manages internal backoffice users

The database architecture is designed to support multi-product administration and future scalability.

⸻

🔄 Application Workflow

The typical tenant onboarding workflow consists of the following steps:

1. An authorized backoffice user logs into the application.
2. The user creates a new tenant organization.
3. Tenant information is stored in Azure SQL Database.
4. The user creates a tenant administrator.
5. Administrator information is saved in the database.
6. The user initiates external application provisioning.
7. Norvia Backoffice communicates with the external application using REST APIs.
8. The external application creates the corresponding user.
9. The returned external user identifier is stored in Norvia Backoffice.
10. The provisioning status is updated.

⸻

💻 Local Development Setup

Prerequisites

Ensure the following tools and services are available:

* .NET 10 SDK
* Visual Studio Code
* Git
* Azure subscription
* Azure SQL Database
* SQL Server extension for VS Code
* AI coding assistant such as Codex or Claude Code (optional)

Step 1: Clone the Repository

git clone <repository-url>
cd NorviaBackoffice

Replace <repository-url> with the actual GitHub repository URL.

Step 2: Restore Dependencies

dotnet restore

Step 3: Configure Database Connection

Configure the Azure SQL Database connection string using .NET User Secrets or environment variables.

Example configuration structure:

{
  "ConnectionStrings": {
    "DefaultConnection": "<AZURE_SQL_CONNECTION_STRING>"
  }
}

Never commit actual database credentials or API secrets to the repository.

Step 4: Build the Application

dotnet build

Step 5: Run the Application

dotnet run

Alternatively, use:

dotnet watch run

The application will start using the local development URLs configured in the project.

⸻

🔐 Security Considerations

Norvia Backoffice follows enterprise application security principles.

* Role-based authorization.
* Server-side permission validation.
* Secure authentication.
* HTTPS communication.
* Secure credential management.
* Protection against unauthorized data access.
* Input validation.
* Secure external API communication.
* Administrative audit logging.

Sensitive configuration values should be managed using environment variables, .NET User Secrets, or Azure Key Vault.

⸻

📋 Software Requirements Specification

The complete requirements document is available online.

Norvia Backoffice — Software Requirements Specification

The document contains the business requirements, functional specifications, application workflows, and acceptance criteria.

Developers should review the requirements before implementing new functionality.

⸻

🎓 About the AI Developer Industry Simulation Program

Norvia Backoffice is developed as part of the AI Developer Industry Simulation Program by Lowcademy.

The program simulates how enterprise software projects are planned, developed, tested, and delivered.

Instead of focusing exclusively on technology tutorials, participants work through a fictional enterprise project with evolving requirements and practical engineering challenges.

Learning Objectives

* Understand enterprise software requirements.
* Design scalable application architecture.
* Develop applications using .NET and Azure SQL.
* Implement business logic and database operations.
* Integrate external REST APIs.
* Apply authentication and authorization.
* Work with AI-assisted development tools.
* Understand enterprise development workflows.
* Practice debugging and problem-solving.
* Gain practical experience with software deployment.

Program Website: Lowcademy

⸻

📈 Project Status

Status: Under Development

The project is being implemented according to the approved Software Requirements Specification.

Additional functionality and improvements will be introduced as development progresses.

⸻

👨‍💻 Developer & Author

Ankit Gangrade

Enterprise Software Architect | OutSystems Champion | AI & Low-Code Specialist | Technical Educator

Ankit Gangrade is an Enterprise Software Architect and Technical Consultant with 14+ years of experience in software engineering, specializing in enterprise application architecture, low-code development, cloud technologies, and AI-powered applications.

His professional experience includes enterprise solution architecture, OutSystems ODC and OutSystems 11, multi-tenant SaaS platforms, application performance optimization, cloud integrations, and AI/Agentic application development.

Professional Highlights

* 14+ Years of software engineering experience.
* OutSystems Champion and community contributor.
* 6× OutSystems Certified.
* 500+ Technical Interviews conducted.
* 3,000+ Learners trained through technical education programs.
* Founder of Lowcademy, a practical technology learning platform.
* Organizer of the OutSystems User Group — Indore.
* Experience in enterprise architecture, cloud platforms, AI, RAG, and Agentic AI.

Areas of Expertise

Enterprise Architecture

* Solution Architecture
* Application Modernization
* Multi-Tenant SaaS Architecture
* Performance Optimization
* Enterprise Integrations

Development Technologies

* OutSystems 11
* OutSystems Developer Cloud (ODC)
* C# / .NET
* SQL Server
* REST APIs

Cloud & AI

* Microsoft Azure
* Azure SQL Database
* Azure AI Search
* Retrieval-Augmented Generation (RAG)
* Agentic AI
* AI-Assisted Software Development

🌐 Connect With Me

Platform	Link
LinkedIn	Ankit Gangrade
Personal Website	ankitg.in
Lowcademy	lowcademy.com
YouTube	Simplify With Ankit
Instagram	@ankitg.in
Ask Me Anything	ama.ankitg.in
Technical Insights	insights.ankitg.in

⸻

📜 Disclaimer

The University of Norvia, Norvia Labs, and the associated project scenario are fictional entities created for educational and software development simulation purposes.

This repository is intended to demonstrate enterprise application development concepts and practices.

⸻

© Copyright

Designed & Developed by Ankit Gangrade

Enterprise Software Architect | OutSystems Champion | Founder, Lowcademy

© 2026 Ankit Gangrade. All Rights Reserved.