// ----------------------------------------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: March 7th 2026
//  Description: Defines the essential data operations for Chat Groups.
// ----------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.API.src.Core.Entities;

namespace Backend.API.src.Core.Interface
{
    public interface IChatGroupRepository
    {

        // CREATE
        Task<ChatGroup> CreateGroupAsync(ChatGroup group);
        Task AddMemberAsync(ChatGroupMember member);


        // READ
        Task<ChatGroup?> GetGroupByIdAsync(Guid groupId);
        Task<IEnumerable<ChatGroup>> GetGroupsForUserAsync(Guid userId);


        // This helps us check if a user is already in a specific group
        Task<bool> IsUserInGroupAsync(Guid groupId, Guid userId);

        // DELETE
        Task RemoveMemberAsync(Guid groupId, Guid userId);
        Task DeleteGroupAsync(ChatGroup group);

        // COMMMIT
        Task<bool> SaveChangesAsync();


    }
}
