// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: May 11 2026
//  Description: Implements the ChatGroup data operations using EF Core.
// --------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.API.src.Core.Entities;
using Backend.API.src.Core.Interface;
using Backend.API.src.Core.Logging;
using Microsoft.EntityFrameworkCore;



namespace Backend.API.src.Infrastructure.Persistence.Repositories
{
    public class ChatGroupRepository : IChatGroupRepository
    {

        private readonly AppDbContext _context;

        public ChatGroupRepository(AppDbContext context)
        { 
        
            _context = context;_context = context;
        
        }

        public async Task<ChatGroup> CreateGroupAsync(ChatGroup group) 
        {

            await _context.ChatGroups.AddAsync(group);
            return group;
        
        }


        public async Task AddMemberAsync(ChatGroupMember member)
        { 
        
            await _context.ChatGroupMembers.AddAsync(member);
        
        }


        public async Task<ChatGroup?> GetGroupByIdAsync(Guid groupId) 
        { 
        
            // We use .Include() so EF Core brings the members list along with the group
            return await _context.ChatGroups
                .Include(cg => cg.Members)
                .FirstOrDefaultAsync(cg => cg.Id == groupId); 
        
        }


        public async Task<IEnumerable<ChatGroup>> GetGroupsForUserAsync(Guid userId)
        { 
        
            // Find all membership rows for this user, then grab the actual ChatGroup data
            return await _context.ChatGroupMembers
                .Where(cgm => cgm.UserId == userId)
                .Include(cgm => cgm.ChatGroup)
                .Select(cgm => cgm.ChatGroup!)
                .ToListAsync();        
        }

        public async Task<bool> IsUserInGroupAsync(Guid groupId, Guid userId)
        {

            return await _context.ChatGroupMembers
                .AnyAsync(cgm => cgm.ChatGroupId == groupId && cgm.UserId == userId);
        
        
        }

        public async Task RemoveMemberAsync(Guid groupId, Guid userId)
        {
            var member = await _context.ChatGroupMembers
                .FirstOrDefaultAsync(cgm => cgm.ChatGroupId == groupId && cgm.UserId == userId);

            if (member != null)
            {
                _context.ChatGroupMembers.Remove(member);
            }
        }

        public async Task DeleteGroupAsync(ChatGroup group)
        {

            _context.ChatGroups.Remove(group);
            // We don't need a task here because Remove is synchronous in EF Core
            await Task.CompletedTask;
        
        }

        public async Task<bool> SaveChangesAsync()
        {

            // SaveChangesAsync returns the number of rows changed. If > 0, it was successful!
            return await _context.SaveChangesAsync() > 0;

        
        }



    }
}
