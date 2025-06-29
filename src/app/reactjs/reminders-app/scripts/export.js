#!/usr/bin/env node

const fs = require('fs');
const path = require('path');

// Simple export script to work around Next.js static export issues with dynamic routes
console.log('Creating static export for GitHub Pages...');

// Ensure out directory exists
const outDir = 'out';
if (!fs.existsSync(outDir)) {
  fs.mkdirSync(outDir);
}

// Copy static assets
if (fs.existsSync('.next/static')) {
  fs.cpSync('.next/static', path.join(outDir, '_next', 'static'), { recursive: true });
}

// Copy public assets
if (fs.existsSync('public')) {
  fs.cpSync('public', outDir, { recursive: true });
}

// Create a simple index.html that redirects to the app with base path
const indexHtml = `<!DOCTYPE html>
<html>
<head>
  <meta charset="utf-8">
  <title>Reminders App</title>
  <meta name="viewport" content="width=device-width,initial-scale=1">
  <meta http-equiv="refresh" content="0; url=/Reminders/">
  <link rel="canonical" href="/Reminders/">
</head>
<body>
  <p>Redirecting to <a href="/Reminders/">Reminders App</a>...</p>
</body>
</html>`;

fs.writeFileSync(path.join(outDir, 'index.html'), indexHtml);

console.log('Static export completed successfully');