using JWT_Test.Data;
using JWT_Test.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JWT_Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ItemController : ControllerBase
    {
        private readonly AppDBContext _context;
        public ItemController(AppDBContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<List<Item>>> GetItems()
        {
            return Ok(await _context.Items.ToListAsync());
        }

        [HttpPost]
        [Route("Additem")]
        public async Task<ActionResult<List<Item>>> AddItem(Item item)
        {
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            return Ok(await _context.Items.ToListAsync());
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Item>> GetItem(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult<List<Item>>> DeleteItem(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return Ok(await _context.Items.ToListAsync());
        }
    }

    }
