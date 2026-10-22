using CarBook.Dto.AppUserDtos;
using CarBook.Persistence.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarBook.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppUsersController : ControllerBase
    {
        private readonly CarBookContext _context;

        public AppUsersController(CarBookContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppUser(int id)
        {
            var user = await _context.AppUsers
                .Include(x => x.AppRole)
                .FirstOrDefaultAsync(x => x.AppUserId == id);

            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            var result = new ResultAppUserDto
            {
                AppUserId = user.AppUserId,
                Username = user.Username,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                AppRoleId = user.AppRoleId,
                RoleName = user.AppRole != null ? user.AppRole.AppRoleName : "Admin"
            };

            return Ok(result);
        }

        [HttpGet("GetByUsername/{username}")]
        public async Task<IActionResult> GetByUsername(string username)
        {
            var user = await _context.AppUsers
                .Include(x => x.AppRole)
                .FirstOrDefaultAsync(x => x.Username == username);

            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            var result = new ResultAppUserDto
            {
                AppUserId = user.AppUserId,
                Username = user.Username,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                AppRoleId = user.AppRoleId,
                RoleName = user.AppRole != null ? user.AppRole.AppRoleName : "Admin"
            };

            return Ok(result);
        }

        [HttpGet("GetFirst")]
        public async Task<IActionResult> GetFirst()
        {
            var user = await _context.AppUsers
                .Include(x => x.AppRole)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            var result = new ResultAppUserDto
            {
                AppUserId = user.AppUserId,
                Username = user.Username,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                AppRoleId = user.AppRoleId,
                RoleName = user.AppRole != null ? user.AppRole.AppRoleName : "Admin"
            };

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAppUser(UpdateAppUserDto dto)
        {
            var user = await _context.AppUsers.FindAsync(dto.AppUserId);
            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            user.Name = dto.Name;
            user.Surname = dto.Surname;
            user.Email = dto.Email;
            user.Username = dto.Username;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                user.Password = dto.Password;
            }

            await _context.SaveChangesAsync();
            return Ok("Profil bilgileri başarıyla güncellendi.");
        }
    }
}
