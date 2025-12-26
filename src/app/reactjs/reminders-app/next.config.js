/** @type {import('next').NextConfig} */

const isProd = process.env.NODE_ENV === 'production';
const isGitHubPages = process.env.GITHUB_PAGES === 'true';

const nextConfig = {
  trailingSlash: true,
  reactStrictMode: false,
  output: 'export',
  ...(isProd && isGitHubPages && {
    basePath: '/Reminders',
    assetPrefix: '/Reminders/',
    images: { unoptimized: true }
  }),
  ...(!isProd && {
    images: { unoptimized: true }
  }),
  ...(isProd && !isGitHubPages && {
    images: { unoptimized: true }
  })
};

module.exports = nextConfig
