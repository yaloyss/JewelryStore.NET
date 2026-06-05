import type { Config } from "tailwindcss";

const config: Config = {
  content: ["./src/**/*.{js,ts,jsx,tsx,mdx}"],
  theme: {
    extend: {
      colors: {
        ink: "#001111",
        accent: "#E5BDDF",
      },
      borderRadius: {
        md: "10px",
      },
      boxShadow: {
        soft: "0 18px 45px rgba(0, 17, 17, 0.08)",
      },
    },
  },
  plugins: [],
};

export default config;

