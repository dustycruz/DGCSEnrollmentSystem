// File: EnrollmentSystem.DAL/Repositories/Implementations/UserRepository.cs
using EnrollmentSystem.DAL.Data;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly EnrollmentSystemDbContext _context;

    public UserRepository(EnrollmentSystemDbContext context)
    {
        _context = context;
    }

    public async Task<AspNetUser?> GetByUsernameAsync(string userName)
        => await _context.AspNetUsers.FirstOrDefaultAsync(u => u.UserName == userName);

    public async Task<AspNetUser?> GetByEmailAsync(string email)
        => await _context.AspNetUsers.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<AspNetUser?> GetByIdAsync(string id)
        => await _context.AspNetUsers.FirstOrDefaultAsync(u => u.Id == id);

    public async Task<IEnumerable<AspNetUser>> GetAllAsync()
        => await _context.AspNetUsers.AsNoTracking().OrderBy(u => u.UserName).ToListAsync();

    public async Task<IEnumerable<string>> GetUserRoleNamesAsync(string userId)
        => await (from ur in _context.AspNetUserRoles
                  join r in _context.AspNetRoles on ur.RoleId equals r.Id
                  where ur.UserId == userId
                  select r.Name!)
                 .ToListAsync();

    public async Task<AspNetRole?> GetRoleByNameAsync(string roleName)
        => await _context.AspNetRoles.FirstOrDefaultAsync(r => r.Name == roleName);

    public async Task<AspNetRole> EnsureRoleAsync(string roleName)
    {
        var role = await _context.AspNetRoles.FirstOrDefaultAsync(r => r.Name == roleName);
        if (role is null)
        {
            role = new AspNetRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = roleName,
                UserName = roleName
            };
            await _context.AspNetRoles.AddAsync(role);
            await _context.SaveChangesAsync();
        }
        return role;
    }

    public async Task AddUserAsync(AspNetUser user)
        => await _context.AspNetUsers.AddAsync(user);

    public async Task AddUserToRoleAsync(string userId, string roleId, string userName, string passwordHash)
    {
        var link = new AspNetUserRole
        {
            UserId = userId,
            RoleId = roleId,
            UserName = userName,
            PasswordHash = passwordHash
        };
        await _context.AspNetUserRoles.AddAsync(link);
    }

    public async Task<bool> UserNameExistsAsync(string userName)
        => await _context.AspNetUsers.AnyAsync(u => u.UserName == userName);

    public async Task<bool> EmailExistsAsync(string email)
        => await _context.AspNetUsers.AnyAsync(u => u.Email == email);

    public async Task<int> SaveAsync()
        => await _context.SaveChangesAsync();

    public async Task<string?> GetLastUserNameByPrefixAsync(string prefix)
       => await _context.AspNetUsers
           .Where(u => u.UserName != null && u.UserName.StartsWith(prefix))
           .OrderByDescending(u => u.UserName)
           .Select(u => u.UserName)
           .FirstOrDefaultAsync();
    public async Task DeleteUserByUserNameAsync(string userName)
    {
        var user = await _context.AspNetUsers.FirstOrDefaultAsync(u => u.UserName == userName);
        if (user is null) return;

        var notifs = _context.Notifications.Where(n => n.UserId == user.Id);
        _context.Notifications.RemoveRange(notifs);

        var links = _context.AspNetUserRoles.Where(r => r.UserId == user.Id);
        _context.AspNetUserRoles.RemoveRange(links);

        _context.AspNetUsers.Remove(user);
        await _context.SaveChangesAsync();
    }
}