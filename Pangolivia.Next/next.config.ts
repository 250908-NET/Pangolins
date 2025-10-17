const nextConfig = {
  reactStrictMode: true,
  async rewrites() {
    return [
      {
        source: "/api/:path*",
        destination: "http://localhost:5156/api/:path*", // Proxy to .NET API
      },
    ];
  },
};

export default nextConfig;

