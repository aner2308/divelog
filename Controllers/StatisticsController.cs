using divelog.Data;
using divelog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace divelog.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            int? year,
            int? groupId,
            int? roleId,
            string? tab)
        {
            int selectedYear = year ?? DateTime.Now.Year;

            // Standard = Dykare
            int selectedRoleId = roleId ?? 1;

            // Alla personer som HAR rollen
            var personsQuery = _context.Persons
                .Include(p => p.Group)
                .Include(p => p.PersonRoles)
                .Where(p =>
                    !p.IsDeleted &&
                    p.PersonRoles.Any(pr => pr.DiveRoleId == selectedRoleId));

            // Gruppfilter
            if (groupId.HasValue)
            {
                personsQuery = personsQuery
                    .Where(p => p.GroupId == groupId);
            }

            var persons = await personsQuery
                .OrderBy(p => p.Signature)
                .ToListAsync();

            // Alla dyk under året
            var diveParticipants = await _context.DiveParticipants
                .Include(dp => dp.Dive)
                .Where(dp =>
                    dp.DiveRoleId == selectedRoleId &&
                    dp.Dive.Date.Year == selectedYear)
                .ToListAsync();

            var rows = new List<StatisticsRowViewModel>();

            var purposes = await _context.DivePurposes
                    .OrderBy(p => p.Name)
                    .ToListAsync();

            var divers = await _context.Persons
                .Include(p => p.Group)
                .Include(p => p.PersonRoles)
                .ThenInclude(pr => pr.DiveRole)
                .Where(p =>
                    !p.IsDeleted &&
                    p.PersonRoles.Any(pr => pr.DiveRoleId == 1))
                .ToListAsync();

            if (groupId.HasValue)
            {
                divers = divers
                    .Where(p => p.GroupId == groupId.Value)
                    .ToList();
            }

            foreach (var person in persons)
            {
                var personDives = diveParticipants
                    .Where(dp => dp.PersonId == person.Id)
                    .ToList();

                int totalDiveMinutes = personDives
                    .Sum(dp => dp.DiveTime ?? 0);

                var monthlyCounts = personDives
                    .GroupBy(dp => dp.Dive.Date.Month)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Count());

                rows.Add(new StatisticsRowViewModel
                {
                    PersonName = person.Name!,
                    Signature = person.Signature!,
                    RoleName = person.PersonRoles
                        .FirstOrDefault(pr => pr.DiveRoleId == selectedRoleId)?.DiveRole?.Name ?? "-",
                    MonthlyCounts = monthlyCounts,
                    TotalDiveMinutes = totalDiveMinutes

                });
            }

            var purposeRows = new List<DivePurposeStatisticsRowViewModel>();

            foreach (var diver in divers)
            {
                var purposeCounts = new Dictionary<string, int>();

                foreach (var purpose in purposes)
                {
                    int count = await _context.DiveParticipants
                        .Include(dp => dp.Dive)
                        .Where(dp =>
                            dp.PersonId == diver.Id &&
                            dp.Dive.Date.Year == selectedYear &&
                            dp.Dive.DivePurposeId == purpose.Id)
                        .CountAsync();

                    purposeCounts[purpose.Name!] = count;
                }

                purposeRows.Add(new DivePurposeStatisticsRowViewModel
                {
                    PersonName = diver.Name!,
                    Signature = diver.Signature!,
                    PurposeCounts = purposeCounts
                });
            }

            var vm = new StatisticsViewModel
            {
                SelectedYear = selectedYear,
                SelectedGroupId = groupId,
                SelectedRoleId = selectedRoleId,
                ActiveTab = tab ?? "dives",

                Groups = await _context.Groups
                    .OrderBy(g => g.Name)
                    .ToListAsync(),

                Roles = await _context.DiveRoles
                    .Select(r => new SelectListItem
                    {
                        Value = r.Id.ToString(),
                        Text = r.Name
                    })
                    .ToListAsync(),

                Rows = rows,
                PurposeRows = purposeRows
            };

            return View(vm);
        }
    }
}