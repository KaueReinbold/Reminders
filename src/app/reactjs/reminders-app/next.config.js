/** @type {import('next').NextConfig} */

const isProd = process.env.NODE_ENV === 'production';
const nextConfig = {
  output: 'export',
  trailingSlash: true,
  reactStrictMode: false,
  ...(isProd && {
    basePath: '/Reminders',
    assetPrefix: '/Reminders/',
    images: { unoptimized: true }
  }),
  ...(!isProd && {
    images: { unoptimized: true }
  })
};

module.exports = nextConfig
