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
    public class DiveController : Controller
    {
        private readonly ApplicationDbContext _context;

        //Controllern kommunicerar med databasen
        public DiveController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Metod för att fylla dropdown-listor för dykpersonal
        //Används både vid GET och POST för samma logik
        private void PopulateDropdowns(CreateDiveViewModel vm)
        {
            vm.DiveLeaders = _context.Persons
                .Where(p => p.PersonRoles.Any(r => r.DiveRoleId == 1))
                .ToList();

            vm.Divers = _context.Persons
                .Where(p => p.PersonRoles.Any(r => r.DiveRoleId == 2))
                .ToList();

            vm.SurfaceSupports = _context.Persons
                .Where(p => p.PersonRoles.Any(r => r.DiveRoleId == 3))
                .ToList();
        }

        //Metod för att skapa ett grundobjekt för ett nytt dyk.
        //Bra när flera dyk skapas samtidigt (Flera dykare med olika tider i samma formulär)
        private Dive CreateBaseDive(CreateDiveViewModel vm)
        {
            return new Dive
            {
                Date = vm.Date,
                LocationName = vm.LocationName,
                Latitude = vm.Latitude,
                Longitude = vm.Longitude,
                Notes = vm.Notes,
                DiveParticipants = new List<DiveParticipant>()
            };
        }

        // GET: Dive
        public async Task<IActionResult> Index()
        {
            return View(await _context.Dives.ToListAsync());
        }

        // GET: Dive/Details/5
        //Inladdning av detaljsida för specifikt dyk
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dive = await _context.Dives
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dive == null)
            {
                return NotFound();
            }

            return View(dive);
        }

        // GET: Dive/Create
        //Inladdning av registreringssidan för nya dyk
        public IActionResult Create()
        {
            var vm = new CreateDiveViewModel
            {

                //Standard för pardyk, med två dykare
                PairGroups = new List<PairGroupViewModel>
                {
                    new PairGroupViewModel
                    {
                        Divers = new List<PairDiverViewModel>
                        {
                            new PairDiverViewModel(),
                            new PairDiverViewModel()
                        }
                    }
                },
                //Standardvärde för "Luft från ytan"
                //En grupp med dykare + dykskötare som standard
                SurfaceSupportGroups = new List<DiveGroupViewModel>
                {
                    new DiveGroupViewModel()
                }
            };
            //Fyller dropdowns med personal
            PopulateDropdowns(vm);
            return View(vm);
        }

        // POST: Dive/Create
        //Tar emot formulärdata när användare skapar ett nytt dyk
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDiveViewModel vm)
        {
            //Ser till att listor aldrig blir null
            vm.PairGroups ??= new();
            vm.SurfaceSupportGroups ??= new();

            foreach (var g in vm.PairGroups)
                g.Divers ??= new();

            //Kontrollerar att användare valt typ av dyk (pardyk, luft från ytan)
            if (string.IsNullOrEmpty(vm.DiveType))
            {
                ModelState.AddModelError("", "Välj typ av dyk");
                PopulateDropdowns(vm);
                return View(vm);
            }

            //Om validering misslyckas skickas formuläret tillbaka med felmeddelanden
            if (!ModelState.IsValid)
            {
                PopulateDropdowns(vm);
                return View(vm);
            }

            //LOGIK FÖR PARDYK
            if (vm.DiveType == "Pardyk")
            {
                foreach (var pairGroup in vm.PairGroups)
                {
                    var dive = CreateBaseDive(vm);

                    //Lägger till vald dykledare
                    dive.DiveParticipants?.Add(new DiveParticipant
                    {
                        PersonId = vm.DiveLeaderId,
                        DiveRoleId = 1
                    });

                    //Lägger till valda dykare
                    foreach (var diver in pairGroup.Divers.Where(d => d.DiverId > 0))
                    {
                        dive.DiveParticipants?.Add(new DiveParticipant
                        {
                            PersonId = diver.DiverId,
                            DiveRoleId = 2,
                            Depth = diver.Depth,
                            DiveTime = diver.DiveTime,
                            AirPressureBefore = diver.AirPressureBefore,
                            AirPressureAfter = diver.AirPressureAfter
                        });
                    }

                    _context.Dives.Add(dive);
                }
            }
            //LOGIK FÖR LYFT FRÅN YTAN
            else if (vm.DiveType == "LuftFrånYtan")
            {
                foreach (var group in vm.SurfaceSupportGroups.Where(g => g.DiverId > 0))
                {
                    var dive = CreateBaseDive(vm);

                    //Lägger till vald dykledare
                    dive.DiveParticipants?.Add(new DiveParticipant
                    {
                        PersonId = vm.DiveLeaderId,
                        DiveRoleId = 1
                    });

                    //Lägger till vald dykare
                    dive.DiveParticipants?.Add(new DiveParticipant
                    {
                        PersonId = group.DiverId,
                        DiveRoleId = 2,
                        Depth = group.Depth,
                        DiveTime = group.DiveTime,
                        AirPressureBefore = group.AirPressureBefore,
                        AirPressureAfter = group.AirPressureAfter
                    });

                    //Lägger till dykskötare
                    if (group.SurfaceSupportId.HasValue)
                    {
                        dive.DiveParticipants?.Add(new DiveParticipant
                        {
                            PersonId = group.SurfaceSupportId.Value,
                            DiveRoleId = 3
                        });
                    }

                    _context.Dives.Add(dive);
                }
            }

            //Sparar alla skapade dyk till databasen
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Dive/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dive = await _context.Dives.FindAsync(id);
            if (dive == null)
            {
                return NotFound();
            }
            return View(dive);
        }

        // POST: Dive/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,StartTime,LocationName,Latitude,Longitude,Notes")] Dive dive)
        {
            if (id != dive.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dive);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiveExists(dive.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dive);
        }

        // GET: Dive/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dive = await _context.Dives
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dive == null)
            {
                return NotFound();
            }

            return View(dive);
        }

        // POST: Dive/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dive = await _context.Dives.FindAsync(id);
            if (dive != null)
            {
                _context.Dives.Remove(dive);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiveExists(int id)
        {
            return _context.Dives.Any(e => e.Id == id);
        }
    }
}
