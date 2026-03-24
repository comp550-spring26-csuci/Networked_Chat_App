# Running frontend

Before starting, make sure you are in the Frontend_Service directory.

First, you must run this to install all node modules:

> npm install

To start the vite server and run the frontend in browser, use:

> npm run dev:react

To run the frontend as an electron app window after starting the vite server, use:

> npm run dev:electron

To run the electron app as a distribution package, use:

> // For mac 
> npm run dist:mac

> // For Windows
> npm run dist:win

> // For Linux
> npm run dist:linux

This will appear as a dist folder in the Frontend-Service directory, which contains the app that you can then run on your desktop
