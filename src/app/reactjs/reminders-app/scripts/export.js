#!/usr/bin/env node

const fs = require('fs');
const path = require('path');

// Simple export script to work around Next.js static export issues with dynamic routes
console.log('Preparing static export for GitHub Pages...');

const outDir = 'out';

// Ensure out directory exists (Next.js should have created it)
if (!fs.existsSync(outDir)) {
  console.error('Error: out directory does not exist. Make sure to run "next build" first.');
  process.exit(1);
}

// Copy 404.html from public to out directory for SPA routing support
const source404 = path.join('public', '404.html');
const dest404 = path.join(outDir, '404.html');

if (fs.existsSync(source404)) {
  fs.copyFileSync(source404, dest404);
  console.log('Copied 404.html for GitHub Pages SPA routing support');
} else {
  console.warn('Warning: public/404.html not found');
}

// Create .nojekyll file to prevent GitHub Pages from ignoring files starting with _
fs.writeFileSync(path.join(outDir, '.nojekyll'), '');
console.log('Created .nojekyll file');

console.log('Static export preparation completed successfully');