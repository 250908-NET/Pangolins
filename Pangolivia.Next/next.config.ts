const API_BASE = process.env.NEXT_PUBLIC_API_BASE || "http://localhost:5156";

const nextConfig = {
  reactStrictMode: true,
  output: "standalone",               // ← REQUIRED for /.next/standalone
  async rewrites() {
    return [
      {
        source: "/api/:path*",
        destination: `${API_BASE}/api/:path*`,
      },
    ];
  },
};

export default nextConfig;
