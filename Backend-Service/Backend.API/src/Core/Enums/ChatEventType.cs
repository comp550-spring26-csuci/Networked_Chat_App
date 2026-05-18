namespace Backend.API.src.Core.Enums
{
    public enum ChatEventType
    {
        UserJoined,
        UserLeft,
        ChatGroupMembershipAdded,
        ChatGroupMembershipDeleted,
        ChatGroupDeleted,
        FriendshipAdded,
        FriendshipDeleted,
        UserStatusChanged
        //FriendRequestReceived,
        //FriendRequestAccepted
    }
}
