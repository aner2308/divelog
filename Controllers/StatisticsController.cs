using divelog.Data;
using divelog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace divelog.Controllers
{
    //Krav på authorisering för att komma åt controllern
    [Authorize]
    public class StatisticsController : Controller
    {
        //Databaskoppling
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        //VISA STATISTIK
        public async Task<IActionResult> Index(
            int? year,
            int? groupId,
            int? roleId,
            string? tab)
        {
            //Om inget år väljs så används nuvarande år
            int selectedYear = year ?? DateTime.Now.Year;

            //Standardroll = Dykare
            int selectedRoleId = roleId ?? 1;

            //Hämtar alla personer som har vald roll
            var personsQuery = _context.Persons
                .Include(p => p.Group)
                .Include(p => p.PersonRoles)
                .Where(p =>
                    !p.IsDeleted &&
                    p.PersonRoles.Any(pr => pr.DiveRoleId == selectedRoleId));

            //Gruppfilter
            if (groupId.HasValue)
            {
                personsQuery = personsQuery
                    .Where(p => p.GroupId == groupId);
            }

            //Sorterar person utefter signatur
            var persons = (await personsQuery.ToListAsync())
                .OrderBy(p => string.IsNullOrEmpty(p.Signature) || char.IsDigit(p.Signature[0]))
                .ThenBy(p => p.Signature)
                .ToList();

            // Alla dykdeltagare för valt år och vald roll
            var diveParticipants = await _context.DiveParticipants
                .Include(dp => dp.Dive)
                .Where(dp =>
                    dp.DiveRoleId == selectedRoleId &&
                    dp.Dive.Date.Year == selectedYear)
                .ToListAsync();

            //Losta till statistik-tabellen
            var rows = new List<StatisticsRowViewModel>();

            //Hämtar alla dyksyften
            var purposes = await _context.DivePurposes
                    .OrderBy(p => p.Name)
                    .ToListAsync();

            //Hämtar alla personer med rollen dykare 
            var divers = (await _context.Persons
                .Include(p => p.Group)
                .Include(p => p.PersonRoles)
                .ThenInclude(pr => pr.DiveRole)
                .Where(p =>
                    !p.IsDeleted &&
                    p.PersonRoles.Any(pr => pr.DiveRoleId == 1))
                .ToListAsync())
                .OrderBy(p => string.IsNullOrEmpty(p.Signature) || char.IsDigit(p.Signature[0]))
                .ThenBy(p => p.Signature)
                .ToList();

            //Filtrera dykare på grupp om grupp har valts
            if (groupId.HasValue)
            {
                divers = divers
                    .Where(p => p.GroupId == groupId.Value)
                    .ToList();
            }

            //Skapar statistikrad för varje person
            foreach (var person in persons)
            {
                //Hämtar personens dyk
                var personDives = diveParticipants
                    .Where(dp => dp.PersonId == person.Id)
                    .ToList();

                //Lägger ihop en dykares totala dyktid under valt år
                int totalDiveMinutes = personDives
                    .Sum(dp => dp.DiveTime ?? 0);

                //Summerar antal dyk per månad 
                var monthlyCounts = personDives
                    .GroupBy(dp => dp.Dive.Date.Month)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Count());

                //Läger till statistikrad
                rows.Add(new StatisticsRowViewModel
                {
                    PersonName = person.Name!,
                    Signature = person.Signature!,
                    //Hämtar mamnet på vald roll
                    RoleName = person.PersonRoles
                        .FirstOrDefault(pr => pr.DiveRoleId == selectedRoleId)?.DiveRole?.Name ?? "-",
                    MonthlyCounts = monthlyCounts,
                    TotalDiveMinutes = totalDiveMinutes

                });
            }

            //Losta för syftes-tabell
            var purposeRows = new List<DivePurposeStatisticsRowViewModel>();

            //Loopar igenom alla dykare
            foreach (var diver in divers)
            {
                //Dictionary med syfte + antal
                var purposeCounts = new Dictionary<string, int>();

                //Loopar igenom alla dyksyften
                foreach (var purpose in purposes)
                {
                    //Räknar antal dyk för aktuellt syfte (under valt år)
                    int count = await _context.DiveParticipants
                        .Include(dp => dp.Dive)
                        .Where(dp =>
                            dp.PersonId == diver.Id &&
                            dp.Dive.Date.Year == selectedYear &&
                            dp.Dive.DivePurposeId == purpose.Id)
                        .CountAsync();

                    //Sparar antal för respektive syfte
                    purposeCounts[purpose.Name!] = count;
                }

                //Lägger till statistikrad för dykaren
                purposeRows.Add(new DivePurposeStatisticsRowViewModel
                {
                    PersonName = diver.Name!,
                    Signature = diver.Signature!,
                    PurposeCounts = purposeCounts
                });
            }

            //Skapar viewmodel som skickas tilll vyn
            var vm = new StatisticsViewModel
            {
                SelectedYear = selectedYear,
                SelectedGroupId = groupId,
                SelectedRoleId = selectedRoleId,
                //Bestämer vilken flik som ska visas
                ActiveTab = tab ?? "dives",

                //Hämtar gruppnamn till select-filtret
                Groups = await _context.Groups
                    .OrderBy(g => g.Name)
                    .ToListAsync(),

                //Hämtar rollnamn till select-filtret
                Roles = await _context.DiveRoles
                    .Select(r => new SelectListItem
                    {
                        Value = r.Id.ToString(),
                        Text = r.Name
                    })
                    .ToListAsync(),

                //Statistik för antal dyk
                Rows = rows,
                //Statistik för dyksyften
                PurposeRows = purposeRows
            };

            return View(vm);
        }
    }
}