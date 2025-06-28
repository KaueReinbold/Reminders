/** @type {import('next').NextConfig} */
const nextConfig = {
  output: 'export',
  trailingSlash: true,
  basePath: '/Reminders',
  assetPrefix: '/Reminders/',
  images: {
    unoptimized: true
  }
}

module.exports = nextConfig
