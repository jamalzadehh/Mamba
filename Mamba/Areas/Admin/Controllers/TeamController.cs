using AutoMapper;
using MambaProject.DAL;
using MambaProject.Migrations;
using MambaProject.Models;
using MambaProject.ViewModels.TeamVM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace MambaProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TeamController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TeamController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var teams = await _context.Teams.ToListAsync();
            return View(teams);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(TeamCreateVM team)
        {
            if (!ModelState.IsValid)
            {
                return View(team);
            }
            var isExisted=await _context.Teams.AnyAsync(x=>x.FullName.ToLower().Contains(team.FullName.ToLower()));
            if (isExisted)
            {
                ModelState.AddModelError("FullName", "Bu name movcuddur");
                return View(team);
            }
            var mapperService = _mapper.Map<Team>(team);

            await _context.Teams.AddAsync(mapperService);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var team=await _context.Teams.FirstOrDefaultAsync(x=>x.Id==id);
            if (team==null)
            {
                throw new Exception("Team movcud deyil");
            }
            var vm=_mapper.Map<TeamUpdateVM>(team);

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(TeamUpdateVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var existed=await _context.Teams.FirstOrDefaultAsync(x=>x.Id == vm.Id);
            if (existed==null)
            {
                throw new Exception("Movcuddur");
            }
            var isExisted = await _context.Teams.AnyAsync(x => x.FullName.ToLower().Contains(vm.FullName.ToLower()) && x.Id !=vm.Id);
            if (isExisted)
            {
                ModelState.AddModelError("FullName", "Bu name movcuddur");
                return View(vm);
            }

            var mapperService = _mapper.Map(vm, existed);

            _context.Update(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
            
        }
    }
}
