export function UserAvatar({ avatarUrl, username }) {
    return (
        <img src={avatarUrl} alt={`${username}'s Avatar`} />
    );
}