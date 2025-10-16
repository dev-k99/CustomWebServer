# Custom Web Server

A lightweight HTTP web server built from scratch using .NET 8.0 and `HttpListener`, inspired by the ["Build Your Own X"](https://github.com/codecrafters-io/build-your-own-x) project. This project demonstrates core web server functionality, including request handling, logging, and serving static content (HTML, CSS, JavaScript, and images) with basic routing.

## Overview

This project implements a custom web server as a .NET 8.0 Class Library (`WebServer`) with a Console App (`WebServerApp`) for execution. It uses `HttpListener` to process HTTP requests and serves static files from a `Website` folder structure. The server logs requests, handles default routing for HTML pages (e.g., `index.html`), and supports assets like CSS, JavaScript, and images.

## Features

- **HTTP Request Handling**: Processes GET requests using .NET’s `HttpListener`.
- **Request Logging**: Logs client IP, HTTP method, and URL path to the console, revealing browser behaviors like `favicon.ico` requests.
- **Static Content Serving**: Serves files from a `Website` folder with subfolders:
  - `Pages`: HTML files (e.g., `index.html`).
  - `CSS`: Stylesheets (e.g., `demo.css`).
  - `Scripts`: JavaScript files (e.g., `jquery-1.11.2.min.js`).
  - `Images`: Image files (e.g., `favicon.ico`).
- **Basic Routing**: Maps URL paths to file extensions (e.g., `.html`, `.css`, `.js`, `.ico`) with appropriate content types and loaders.
- **Default Page Handling**: Serves `Pages/index.html` for root requests (e.g., `http://localhost:8080/`).

## Tech Stack

- **C# (.NET 8.0)**: Modern, cross-platform framework for the server and console app.
- **HttpListener**: Handles low-level HTTP request processing.
- **Semaphore**: Manages concurrent connections (up to 20 simultaneous requests).
- **Git/GitHub**: Version control for clean, professional code management.

## Project Structure

CustomWebServer/
├── Website/
│   ├── Pages/
│   │   └── index.html
│   ├── CSS/
│   │   └── demo.css
│   ├── Scripts/
│   │   └── jquery-1.11.2.min.js
│   ├── Images/
│   │   └── favicon.ico
├── WebServer/
│   ├── Server.cs
│   ├── Router.cs
│   ├── Extensions.cs
│   ├── WebServer.csproj
├── WebServerApp/
│   ├── Program.cs
│   ├── WebServerApp.csproj
├── .gitignore
├── README.md

- **WebServer**: Class Library with `HttpListener`, routing, and logging logic.
- **WebServerApp**: Console App to run the server.
- **Website**: Static content folder for HTML, CSS, JavaScript, and images.
- **.gitignore**: Excludes `.vs/`, `bin/`, `obj/` for a clean repository.

## Setup and Installation

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/dev-k99/CustomWebServer.git
   cd CustomWebServer

Ensure .NET 8.0 SDK:Install from Microsoft .NET.
Verify: dotnet --version (should output 8.0.xxx).

Build the Solution:bash

dotnet build

Run the Server:bash

cd WebServerApp
dotnet run

Test in Browser:Visit http://localhost:8080/ to see a styled HTML page with a clickable button powered by jQuery.
Console logs show requests (e.g., GET /, GET /favicon.ico).

Grant Permissions (if needed):Run as Administrator or:bash

netsh http add urlacl url=http://+:8080/ user=Everyone

Design DecisionsHttpListener: Chosen for simplicity over raw sockets, providing built-in HTTP request parsing despite slight performance trade-offs.
Modular Structure: Implemented as a Class Library (WebServer) for reusability, with a separate Console App (WebServerApp) for execution.
Static Content: Uses a Website folder to mimic real-world web server file serving, with routing for common file types.
Semaphore: Limits concurrent connections to 20 for controlled resource usage.

Challenges OvercomeFramework Mismatch: Resolved by updating WebServerApp to .NET 8.0, removing App.config, and ensuring compatibility with WebServer.
Git Issues: Fixed permission errors by adding .gitignore to exclude .vs/, bin/, and obj/, and resolved non-fast-forward push errors with git pull --rebase.
Logging: Replaced article’s non-standard RightOf method with request.Url.AbsolutePath for simpler, reliable request logging.
Routing: Created a Router class to map file extensions to content types and loaders, handling special cases like index.html for root requests.
Website Folder: Set up Website structure and adjusted GetWebsitePath to locate files correctly from the solution root.

What I LearnedDeepened understanding of HTTP request lifecycle and HttpListener mechanics.
Improved skills in C# async programming and file I/O for serving static content.
Gained experience with Git troubleshooting (e.g., merge conflicts, permissions).
Learned to observe real-world browser behavior (e.g., automatic favicon.ico requests).

Current LimitationsNo Error Handling: Missing files or unknown extensions return a basic 404 without detailed responses.
GET-Only: Only supports GET requests; POST and other verbs are not handled.
No Dynamic Content: Content is served statically; no database or dynamic generation.
No Security: Lacks authentication, HTTPS, or session management.

Future ImprovementsAdd error handling for 404/500 responses with user-friendly messages.
Support POST requests for form submissions.
Implement dynamic content generation (e.g., from a database).
Add middleware for logging, authentication, or redirects.
Deploy to Azure/Docker for a live demo.

DemoRun dotnet run in WebServerApp and visit http://localhost:8080/ to see a styled HTML page with a clickable button that triggers a jQuery alert. The console logs requests for index.html, demo.css, jquery-1.11.2.min.js, and favicon.ico.LicenseMIT LicenseBuilt by [Kwanele] (https://github.com/dev-k99). Connect with me on LinkedIn for more projects!

