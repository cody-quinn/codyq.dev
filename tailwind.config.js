module.exports = {
  purge: ["./pages/**/*.{js,ts,jsx,tsx}", "./components/**/*.{js,ts,jsx,tsx}"],
  darkMode: false, // or 'media' or 'class'
  theme: {
    extend: {
      spacing: {
        100: "25rem",
        200: "50rem",
        300: "75rem",
        400: "100rem",
      },
    },
  },
  variants: {
    extend: {},
  },
  plugins: [],
};
