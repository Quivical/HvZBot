export function UserAvatar({ userId, avatarHash, size, username }) {
    return (
        <img src={`https://cdn.discordapp.com/avatars/${userId}/${avatarHash}.webp?size=${size}`} alt={`${username}'s Avatar`} />
    );
}