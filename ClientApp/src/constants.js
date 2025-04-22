export const API_BASE_URL = Object.freeze(
    process.env.NODE_ENV === 'production'
        ? 'https://hvzbot.live'
        : 'https://localhost:7235'
);