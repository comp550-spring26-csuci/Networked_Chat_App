# Running frontend

Before starting, make sure you are in the Frontend-Service directory.

First, you must run this to install all node modules with:
```
npm install
```
To start the vite server and run the frontend in browser, use:
```
npm run dev:react
```
To run the frontend as an electron app window after starting the vite server, use:
```
npm run dev:electron
```
To package the electron app, use one of these depending on your operating system:

```
npm run dist:mac
npm run dist:win
npm run dist:linux
```

This will appear as a dist folder in the Frontend-Service directory, which contains the app that you can then run on your desktop
