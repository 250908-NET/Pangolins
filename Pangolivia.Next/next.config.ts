const API_BASE = process.env.NEXT_PUBLIC_API_BASE || "http://api:8080";

const nextConfig = {
  reactStrictMode: true,
  output: "standalone",
  async rewrites() {
    return [{ source: "/api/:path*", destination: `${API_BASE}/api/:path*` }];
  },
};

export default nextConfig;
