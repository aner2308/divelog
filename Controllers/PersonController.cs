using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using divelog.Data;
using divelog.Models;
using divelog.ViewModels;

namespace divelog.Controllers
{
    public class PersonController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PersonController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Person
        //Hämtar alla personer från databasen
        public async Task<IActionResult> Index(int page = 1, int? groupId = null)
        {
            //Totalt antal personer per sida
            int pageSize = 10;

            //Hämta alla personer
            var query = _context.Persons
                                .Include(p => p.Group)
                                .AsQueryable();

            //Filtrera på grupp om groupId är valt
            if (groupId.HasValue)
            {
                query = query.Where(p => p.GroupId == groupId);
            }

            //Sortera efter namn
            query = query.OrderBy(p => p.Name);

            var totalItems = await query.CountAsync();

            //Hämta bara sidan vi vill visa
            var persons = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            //ViewBag för paginering
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            //ViewBag för grupp-filter dropdown
            ViewBag.Groups = new SelectList(
                _context.Groups.OrderBy(g => g.Name),
                "Id",
                "Name",
                groupId
            );

            return View(persons);
        }

        // GET: Person/Details/5
        //Hämtar en specifik person (med all information) fån databasen
        public async Task<IActionResult> Details(int? id)
        {
            //Om id inte finns
            if (id == null)
            {
                return NotFound();
            }

            //Hämtar person inklusive grupp och roller
            var person = await _context.Persons
                .Include(p => p.Group)
                    .Include(p => p.PersonRoles)
                    .ThenInclude(pr => pr.DiveRole)
                .FirstOrDefaultAsync(m => m.Id == id);

            //Om person inte finns
            if (person == null)
            {
                return NotFound();
            }

            //Returnerar en person
            return View(person);
        }

        // GET: Person/Create
        //Förbereder data som behövs för formuläret
        public IActionResult Create()
        {
            //Skapar en ViewModel och fyller den med alla roller
            var vm = new PersonCreateViewModel
            {
                AvailableRoles = _context.DiveRoles.ToList(),
                StartedDiving = DateTime.Today
            };

            //Skapar dropdown för groups
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Name");

            return View(vm);
        }

        // POST: Person/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //Lägger till ny person i databasen
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PersonCreateViewModel vm)
        {
            //Hämta id för rollen Dykare
            var diverRoleId = await _context.DiveRoles
                .Where(r => r.Name == "Dykare")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            //Kontrollera om personen är dykare
            bool isDiver = vm.SelectedRoleIds.Contains(diverRoleId);

            //Felhantering
            //Kontrollera korrekt ifyllt fält för signatur
            if (string.IsNullOrWhiteSpace(vm.Signature))
            {
                ModelState.AddModelError("Signature", "Signatur måste anges");
            }
            //Kontrollera korrekt ifyllt fält för namn
            if (string.IsNullOrWhiteSpace(vm.Name))
            {
                ModelState.AddModelError("Name", "Namn måste anges");
            }
            //Kontrollera att StartedDiving är ifyllt om personen är dykare
            if (isDiver && vm.StartedDiving == null)
            {
                ModelState.AddModelError("StartedDiving", "Datum för när personen började dyka måste anges");
            }

            //Om formulärdata är felaktig
            if (!ModelState.IsValid)
            {
                //Återställ roller
                vm.AvailableRoles = await _context.DiveRoles.ToListAsync();

                //Återställ grupper
                ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Name", vm.GroupId);

                return View(vm);
            }

            //Skapa person
            var person = new Person
            {
                Name = vm.Name,
                Signature = vm.Signature,
                GroupId = vm.GroupId,
                StartedDiving = isDiver ? vm.StartedDiving : null, //Om personen inte är dykare sätts StartedDiving till null
                CreatedAt = DateTime.UtcNow, //Sätts automatiskt till aktuell tid
                PersonRoles = new List<PersonRole>()
            };

            //Lägg till valda roller
            if (vm.SelectedRoleIds != null && vm.SelectedRoleIds.Any())
            {
                person.PersonRoles = vm.SelectedRoleIds
                    .Select(roleId => new PersonRole
                    {
                        DiveRoleId = roleId
                    })
                    .ToList();
            }

            //Spara
            _context.Persons.Add(person);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Person/Edit/5
        //Förbereder data som behövs för formuläret
        public async Task<IActionResult> Edit(int? id)
        {
            //Kontrollerar om id finns
            if (id == null)
            {
                return NotFound();
            }

            //Hämtar person med roller
            var person = await _context.Persons
            .Include(p => p.PersonRoles)
            .FirstOrDefaultAsync(p => p.Id == id);

            //Kontrollerar om person finns
            if (person == null)
            {
                return NotFound();
            }

            //Skapar viewModel och fyller med data från person
            var vm = new PersonEditViewModel
            {
                Id = person.Id,
                Name = person.Name,
                Signature = person.Signature,
                GroupId = person.GroupId,
                StartedDiving = person.StartedDiving ?? DateTime.Today,
                AvailableRoles = _context.DiveRoles.ToList(),
                SelectedRoleIds = person.PersonRoles.Select(pr => pr.DiveRoleId).ToList()
            };

            //Skapar en dropdown för grupper
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Name", person.GroupId);

            return View(vm);
        }

        // POST: Person/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //Uppdatera person i databasen
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PersonEditViewModel vm)
        {
            //Hämta id för rollen Dykare
            var diverRoleId = await _context.DiveRoles
                .Where(r => r.Name == "Dykare")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            //Kontrollera om personen är dykare
            bool isDiver = vm.SelectedRoleIds.Contains(diverRoleId);

            //Felhantering
            //Kontrollera korrekt ifyllt fält för signatur
            if (string.IsNullOrWhiteSpace(vm.Signature))
            {
                ModelState.AddModelError("Signature", "Signatur måste anges");
            }
            //Kontrollera korrekt ifyllt fält för namn
            if (string.IsNullOrWhiteSpace(vm.Name))
            {
                ModelState.AddModelError("Name", "Namn måste anges");
            }
            //Kontrollera att StartedDiving är ifyllt om personen är dykare
            if (isDiver && vm.StartedDiving == null)
            {
                ModelState.AddModelError("StartedDiving", "Datum för när personen började dyka måste anges");
            }

            //Om formulärdata är felaktig
            if (!ModelState.IsValid)
            {
                //Återställ dykroller
                vm.AvailableRoles = await _context.DiveRoles.ToListAsync();
                //Återställ grupper
                ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Name", vm.GroupId);

                return View(vm);
            }

            //Hämtar person från databasen
            var person = await _context.Persons
                .Include(p => p.PersonRoles)
                .FirstOrDefaultAsync(p => p.Id == vm.Id);

            //Om person inte finns
            if (person == null)
            {
                return NotFound();
            }

            //Uppdaterar data
            person.Signature = vm.Signature;
            person.Name = vm.Name;
            person.GroupId = vm.GroupId;
            //Om personen inte är dykare sätts StartedDiving till null
            person.StartedDiving = isDiver ? vm.StartedDiving : null;

            //Uppdaterar roller
            var existingRoleIds = person.PersonRoles.Select(pr => pr.DiveRoleId).ToList();

            //Tar bort roller som inte längre är valda
            var rolesToRemove = person.PersonRoles.Where(pr => !vm.SelectedRoleIds.Contains(pr.DiveRoleId)).ToList();
            _context.PersonRoles.RemoveRange(rolesToRemove);

            //Lägger till nya roller
            var rolesToAdd = vm.SelectedRoleIds.Where(rid => !existingRoleIds.Contains(rid)).ToList();

            //Uppdaterar och registrerar valda roller till person
            foreach (var roleId in rolesToAdd)
            {
                _context.PersonRoles.Add(new PersonRole
                {
                    PersonId = person.Id,
                    DiveRoleId = roleId
                });
            }

            //Sparar ändringar
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Person/Delete/5
        //Hämta person på sidan delete
        public async Task<IActionResult> Delete(int? id)
        {
            //Om id inte finns
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Persons
                .Include(p => p.Group)
                .Include(p => p.PersonRoles)
                .ThenInclude(pr => pr.DiveRole)
                .FirstOrDefaultAsync(m => m.Id == id);

            //Om person inte finns
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: Person/Delete/5
        //Ta bort person från databas
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //Hämta person med hjälp av id
            var person = await _context.Persons.FindAsync(id);

            if (person != null)
            {
                //Ta bort personen
                _context.Persons.Remove(person);
            }

            //Sparar ändringar
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonExists(int id)
        {
            return _context.Persons.Any(e => e.Id == id);
        }
    }
}
