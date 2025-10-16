CustomWebServer

Build Status .NET LicenseA lightweight, custom-built HTTP web server written in C# using HttpListener, designed to serve static content, handle dynamic routing, and implement session-based authentication with CSRF protection. This project showcases a minimal yet robust web server capable of handling GET, POST, and PUT requests, with support for AJAX, custom error pages, and public IP redirects for deployed environments. Ideal for learning the internals of web servers or prototyping small-scale web applications.

Table of Contents
Project Overview (#project-overview)
Features (#features)
Project Flow (#project-flow)
Setup Instructions (#setup-instructions)
Usage and Testing (#usage-and-testing)
Challenges and Solutions (#challenges-and-solutions)
Future Enhancements (#future-enhancements)
License (#license)
Contact (#contact)

Project OverviewCustomWebServer is a from-scratch implementation of an HTTP web server in C#, built to deepen understanding of web server mechanics while providing a functional, extensible platform for serving web content. Unlike frameworks like ASP.NET, this server prioritizes simplicity and control, with only ~650 lines of code across four core classes (Server, Router, Session, RouteHandlers). It supports static file serving (HTML, CSS, JS, images), custom routing for different HTTP verbs, session management, AJAX requests, CSRF protection, and public IP handling for cloud deployment.The project was developed iteratively through nine steps, addressing common web server challenges such as error handling, authentication, and security. It’s a portfolio piece demonstrating proficiency in C#, networking, and web development fundamentals.FeaturesHTTP Request Handling: Supports GET, POST, and PUT verbs using HttpListener.
Static Content Serving: Delivers HTML, CSS, JavaScript, and images from a Website directory.
Custom Routing: Flexible routing for specific verb-path combinations with support for anonymous, authenticated, and expirable routes.
Session Management: Tracks client sessions by IP with authorization and expiration (60 seconds default).
Error Handling: Custom error pages for file not found, unauthorized access, session expiration, and more.
AJAX Support: Handles asynchronous PUT and GET requests with JSON-like responses.
CSRF Protection: Injects and validates anti-forgery tokens in forms to prevent cross-site request forgery.
Public IP Redirects: Supports redirects using public IP or domain for deployed environments.
Logging: Detailed console logging of requests, parameters, and errors for debugging.
Minimal Footprint: Lightweight design with <650 lines of code, emphasizing clarity and maintainability.

Project Flow
The project was developed in nine iterative steps, each addressing specific functionality and challenges:Basic HTTP Server (Steps 1-3):Set up HttpListener to listen on http://localhost:8080/ and local IPs (e.g., 172.16.106.65:8080).
Served static files (e.g., index.html, demo.css) from Website/Pages.
Implemented default routing for root (/) to index.html.

Error Handling (Step 4):Added ServerError enum and custom error pages (fileNotFound.html, unknownType.html, etc.) in Website/ErrorPages.
Implemented ErrorHandler to redirect to appropriate error pages for 404, 500, etc.

Custom Routing and Verbs (Step 5):Introduced Route class and route table to handle GET and POST verbs.
Added /demo/redirect page with POST form to redirect to /demo/clicked.
Processed URL and POST parameters into key-value pairs.

Session Management and Authentication (Step 6):Added Session and SessionManager to track client sessions by IP.
Implemented AnonymousRouteHandler, AuthenticatedRouteHandler, and AuthenticatedExpirableRouteHandler for routing control.
Added /demo/auth and /demo/auth-exp pages to test authentication and session expiration.

AJAX Queries (Step 7):Added /demo/ajax page with jQuery AJAX (PUT and GET) support.
Refactored route handlers to return ResponsePacket for flexible responses (data or redirects).

Public IP Support (Step 8):Added publicIP field for redirects in deployed environments (e.g., AWS EC2).
Supported domain-based redirects (e.g., www.yourdomain.com).

CSRF Protection and HTML Post-Processing (Step 9):Added CSRF token generation and validation for non-GET requests.
Implemented HTML post-processing to inject tokens (<%AntiForgeryToken%>).
Created validationError.html for CSRF failures.

Session Cleanup (Enhancement):Added a timer to remove stale sessions after 24 hours to prevent memory bloat.

Troubleshooting:Addressed issues like missing request logs by ensuring network permissions and file paths.

Setup InstructionsPrerequisites.NET SDK 8.0: Install from dotnet.microsoft.com.
Git: For cloning the repository.
jQuery: Included as jquery-1.11.2.min.js in Website/Scripts.
Windows: For HttpListener and file system paths (Linux/Mac support requires path adjustments).

InstallationClone the Repository:bash

git clone https://github.com/dev-k99/CustomWebServer.git
cd CustomWebServer

Verify Project Structure:

CustomWebServer/
├── WebServer/                  # Core server logic
│   ├── Extensions.cs
│   ├── Router.cs
│   ├── Server.cs
│   ├── Session.cs
│   ├── RouteHandlers.cs
├── WebServerApp/               # Application entry point
│   ├── Program.cs
│   ├── WebServerApp.csproj
├── Website/                    # Static content
│   ├── Pages/
│   │   ├── index.html
│   │   ├── demo/
│   │   │   ├── redirect.html
│   │   │   ├── clicked.html
│   │   │   ├── auth.html
│   │   │   ├── auth-exp.html
│   │   │   ├── ajax.html
│   ├── CSS/
│   │   └── demo.css
│   ├── Scripts/
│   │   └── jquery-1.11.2.min.js
│   ├── Images/
│   │   └── favicon.ico
│   ├── ErrorPages/
│   │   ├── expiredSession.html
│   │   ├── notAuthorized.html
│   │   ├── fileNotFound.html
│   │   ├── pageNotFound.html
│   │   ├── serverError.html
│   │   ├── unknownType.html
│   │   ├── validationError.html
├── README.md
├── .gitignore

Grant Network Permissions (Windows):bash

netsh http add urlacl url=http://+:8080/ user=Everyone
netsh advfirewall firewall add rule name="CustomWebServer" dir=in action=allow protocol=TCP localport=8080

Build and Run:bash

cd WebServerApp
dotnet build
dotnet run

Access the Server:Open a browser to http://localhost:8080/.
Console should show:

Website Path: C:\path\to\CustomWebServer\Website
Listening on IP http://172.16.106.65:8080/
Waiting for a connection...

Usage and TestingDemo PagesHome Page: http://localhost:8080/ serves index.html with a styled button.
Redirect Demo: http://localhost:8080/demo/redirect (POST to /demo/clicked).
Authenticated Route: http://localhost:8080/demo/auth (requires session.Authorized, spoofed in Program.cs).
Expirable Route: http://localhost:8080/demo/auth-exp (expires after 60 seconds).
AJAX Demo: http://localhost:8080/demo/ajax (PUT request, alerts “You said 5”).
AJAX GET: http://localhost:8080/demo/ajax?number=10 (returns “GET response: You said 10”).
Error Pages: Try http://localhost:8080/test.xyz (unknown type) or http://localhost:8080/nonexistent.html (file not found).

Testing with Curlbash

curl http://localhost:8080/demo/ajax?number=10
curl -X PUT -d "number=5&__CSRFToken__=<guid>" http://localhost:8080/demo/ajax

Expected Console Output

Connection received!
127.0.0.1:xxxxx GET /demo/ajax
Routing: GET /demo/ajax with params number=10
Connection received!
127.0.0.1:xxxxx PUT /demo/ajax
number : 5
__CSRFToken__ : a9161119-de6f-4bb2-8e21-8d089d556c37
Routing: PUT /demo/ajax with params

Challenges and Solutions
No Request Logs: Resolved by adding debug logs in StartConnectionListener and ensuring network permissions.
File Addition Issues: Fixed by checking .gitignore and file system permissions for Website/Pages/demo.
AJAX Handling: Refactored route handlers to return ResponsePacket for flexible data responses.
CSRF Security: Implemented token injection and validation for non-GET requests.
Session Bloat: Added a cleanup timer to remove stale sessions after 24 hours.
Deployment: Supported public IP redirects for cloud environments like AWS EC2.

Future Enhancements
HTTPS Support: Add SSL certificate handling for secure connections.
Parameter Decoding: Decode URL-encoded parameters (e.g., + to spaces, %xx to characters).
Login System: Implement a /demo/login page to set session.Authorized.
More Verbs: Support DELETE, PATCH, etc., for RESTful APIs.
Dynamic Content: Integrate a database or template engine for server-side rendering.
Chained Post-Processing: Allow multiple HTML transformations (e.g., Slim template parsing).

License
This project is licensed under the MIT License - see the LICENSE file for details.
ContactAuthor: Kwanele (dev-k99)
GitHub: dev-k99
Issues: Report bugs or suggest features via GitHub Issues

