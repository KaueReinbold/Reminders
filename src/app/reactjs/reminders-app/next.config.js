/** @type {import('next').NextConfig} */

const isProd = process.env.NODE_ENV === 'production';
const isGitHubPages = process.env.GITHUB_PAGES === 'true';

// The Pages demo has no backend, so it always runs on the in-browser mock API.
// Set NEXT_PUBLIC_MOCK_API=true to run the same demo locally.
const isMockApi = process.env.NEXT_PUBLIC_MOCK_API === 'true' || isGitHubPages;

const nextConfig = {
  trailingSlash: true,
  reactStrictMode: false,
  env: { NEXT_PUBLIC_MOCK_API: String(isMockApi) },
  ...(isProd && isGitHubPages && {
    output: 'export',
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
