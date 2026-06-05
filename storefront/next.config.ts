import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  /* config options here */
  images: {
    remotePatterns: [
      {
        protocol: "http",
        hostname: "localhost",
        port: "5010",
        pathname: "/images/**",
      },
    ],
  },
};

export default nextConfig;
