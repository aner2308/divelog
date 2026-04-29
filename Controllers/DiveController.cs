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

        private List<Person> SortPersons(List<Person> persons)
        {
            return persons
                .OrderBy(p => string.IsNullOrEmpty(p.Signature) || char.IsDigit(p.Signature[0]))
                .ThenBy(p => p.Signature)
                .ToList();
        }

        //Metod för att fylla dropdown-listor för dykpersonal
        //Används både vid GET och POST för samma logik
        private void PopulateDropdowns(CreateDiveViewModel vm)
        {
            vm.Divers = SortPersons(_context.Persons
                .Where(p => p.PersonRoles.Any(r => r.DiveRoleId == 1))
                .ToList());

            vm.DiveLeaders = SortPersons(_context.Persons
                .Where(p => p.PersonRoles.Any(r => r.DiveRoleId == 2))
                .ToList());

            vm.SurfaceSupports = SortPersons(_context.Persons
                .Where(p => p.PersonRoles.Any(r => r.DiveRoleId == 3))
                .ToList());

            vm.DivePurposes = _context.DivePurposes.ToList();
        }

        //Dropdowns vid redigering av pardyk
        private void PopulateBuddyEditDropdowns(EditBuddyDiveViewModel vm)
        {
            vm.AvailableDivers = SortPersons(_context.Persons
                .Where(p => p.PersonRoles.Any(r => r.DiveRoleId == 1))
                .ToList());

            vm.DiveLeaders = SortPersons(_context.Persons
                .Where(p => p.PersonRoles.Any(r => r.DiveRoleId == 2))
                .ToList());

            vm.DivePurposes = _context.DivePurposes.ToList();
        }

        //Dropdowns vid redigering av LFY dyk
        private void PopulateSurfaceEditDropdowns(EditSurfaceSupportDiveViewModel vm)
        {
            vm.AvailableDivers = SortPersons(_context.Persons
                .Where(p => p.PersonRoles.Any(r => r.DiveRoleId == 1))
                .ToList());

            vm.DiveLeaders = SortPersons(_context.Persons
                .Where(p => p.PersonRoles.Any(r => r.DiveRoleId == 2))
                .ToList());

            vm.SurfaceSupports = SortPersons(_context.Persons
                .Where(p => p.PersonRoles.Any(r => r.DiveRoleId == 3))
                .ToList());

            vm.DivePurposes = _context.DivePurposes.ToList();
        }

        // GET: Dive
        public async Task<IActionResult> Index(int page = 1, int? groupId = null, int? personId = null, int? divePurposeId = null)
        {
            //Totalt antal dyk per sida
            int pageSize = 10;

            var query = _context.Dives
                .Include(d => d.DiveParticipants)
                    .ThenInclude(dp => dp.Person)
                .Include(d => d.DivePurpose)
                .AsQueryable();

            //Filtrera på grupp
            if (groupId.HasValue)
            {
                query = query.Where(d =>
                    d.DiveParticipants.Any(p => p.Person.GroupId == groupId));
            }

            //Filtrera på person
            if (personId.HasValue)
            {
                query = query.Where(d =>
                    d.DiveParticipants.Any(p => p.PersonId == personId));
            }

            //Filtrera på syfte
            if (divePurposeId.HasValue)
            {
                query = query.Where(d => d.DivePurposeId == divePurposeId);
            }

            //Sortera efter datum
            query = query.OrderByDescending(d => d.Date);

            var totalItems = await query.CountAsync();

            //Hämta sidan vi vill visa
            var dives = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            //ViewBag för paginering
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            //Dropdown för grupper
            ViewBag.Groups = new SelectList(
                _context.Groups.OrderBy(g => g.Name),
                "Id",
                "Name",
                groupId
            );

            //Hämta personer från databasen
            var persons = _context.Persons.Where(p => !p.IsDeleted).AsQueryable();

            //Om det redan är filtrerat på grupp, välj endast personer från gruppen
            if (groupId.HasValue)
            {
                persons = persons.Where(p => p.GroupId == groupId);
            }

            //Dropdown för personer
            ViewBag.Persons = new SelectList(
                    persons
                    .OrderBy(p => p.Name)
                    .ToList()
                    .Select(p => new
                    {
                        p.Id,
                        DisplayName = (p.Name ?? "").Length > 25
                        ? $"{p.Signature} - {p.Name.Substring(0, 20)}..."
                        : $"{p.Signature} - {p.Name}"
                    }),
                    "Id",
                    "DisplayName",
                    personId
            );

            //Dropdown för syften
            ViewBag.Purposes = new SelectList(
                _context.DivePurposes.OrderBy(p => p.Name),
                "Id",
                "Name",
                divePurposeId
            );

            //Sparar aktuella filtervärden för att behålla filtrering vid paginering
            ViewBag.GroupId = groupId;
            ViewBag.PersonId = personId;
            ViewBag.DivePurposeId = divePurposeId;

            return View(dives);
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
            .Include(d => d.DiveParticipants)
            .ThenInclude(dp => dp.Person)
            .Include(d => d.DiveParticipants)
            .ThenInclude(dp => dp.DiveRole)
            .Include(d => d.DivePurpose)
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
                Date = DateTime.Today,
                StartTime = TimeSpan.FromMinutes(Math.Round(DateTime.Now.TimeOfDay.TotalMinutes / 5) * 5),

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
            var divesToSave = new List<Dive>();

            //Ser till att listor aldrig blir null
            vm.PairGroups ??= new();
            vm.SurfaceSupportGroups ??= new();

            foreach (var g in vm.PairGroups)
                g.Divers ??= new();

            //Kontrollerar att användare valt typ av dyk (pardyk, luft från ytan)
            if (vm.DiveType <= 0)
            {
                ModelState.AddModelError("", "Välj typ av dyk");
                PopulateDropdowns(vm);
                return View(vm);
            }

            //Kontroll om angett datum är giltigt
            if (vm.Date == default)
                ModelState.AddModelError("", "Datum måste anges.");

            //Kontroll av namn på dykplats
            if (string.IsNullOrWhiteSpace(vm.LocationName))
                ModelState.AddModelError("", "Plats måste anges.");

            if (vm.LocationName?.Length >= 30)
                ModelState.AddModelError("", "Platsnamn får vara max 30 tecken.");

            //Kontroll av dykledare
            if (vm.DiveLeaderId <= 0)
                ModelState.AddModelError("", "Dykledare måste väljas.");

            //Kontroll av dyktyp
            if (vm.DiveType <= 0)
                ModelState.AddModelError("", "Typ av dyk måste väljas.");

            //FELHANTERING PARDYK
            if (vm.DiveType == DiveType.Pardyk)
            {
                foreach (var pairGroup in vm.PairGroups)
                {
                    if (pairGroup.Divers.Count < 2)
                    {
                        ModelState.AddModelError("", "Minst två dykare krävs för pardyk.");
                    }

                    foreach (var diver in pairGroup.Divers)
                    {
                        if (!diver.DiverId.HasValue)
                            ModelState.AddModelError("", "Alla dykare i pardyk måste väljas.");

                        if (!diver.Depth.HasValue)
                            ModelState.AddModelError("", "Alla dykare måste ha djup angivet.");

                        if (!diver.DiveTime.HasValue)
                            ModelState.AddModelError("", "Alla dykare måste ha dyktid angiven.");

                        if (!diver.AirPressureBefore.HasValue)
                            ModelState.AddModelError("", "Lufttryck före måste anges för alla dykare.");

                        if (!diver.AirPressureAfter.HasValue)
                            ModelState.AddModelError("", "Lufttryck efter måste anges för alla dykare.");
                    }

                    var ids = new List<int>();

                    if (vm.DiveLeaderId.HasValue)
                        ids.Add(vm.DiveLeaderId.Value);

                    ids.AddRange(pairGroup.Divers
                        .Where(d => d.DiverId.HasValue)
                        .Select(d => d.DiverId.Value));

                    if (ids.Count != ids.Distinct().Count())
                    {
                        ModelState.AddModelError("", "Samma person kan inte väljas flera gånger i samma pardyk.");
                    }
                }
            }

            //FELHANTERING LUFT FRÅN YTAN
            if (vm.DiveType == DiveType.LuftFranYtan)
            {
                foreach (var group in vm.SurfaceSupportGroups)
                {
                    if (!group.DiverId.HasValue)
                        ModelState.AddModelError("", "Dykare måste väljas.");

                    if (!group.SurfaceSupportId.HasValue)
                        ModelState.AddModelError("", "Dykskötare måste väljas.");

                    if (!group.Depth.HasValue)
                        ModelState.AddModelError("", "Djup måste anges.");

                    if (!group.DiveTime.HasValue)
                        ModelState.AddModelError("", "Dyktid måste anges.");

                    var ids = new List<int>();

                    if (vm.DiveLeaderId.HasValue)
                        ids.Add(vm.DiveLeaderId.Value);

                    if (group.DiverId.HasValue)
                        ids.Add(group.DiverId.Value);

                    if (group.SurfaceSupportId.HasValue)
                        ids.Add(group.SurfaceSupportId.Value);

                    if (ids.Count != ids.Distinct().Count())
                    {
                        ModelState.AddModelError("", "Samma person kan inte ha flera roller i samma dyk.");
                    }
                }
            }

            //Om validering misslyckas skickas formuläret tillbaka med felmeddelanden
            if (!ModelState.IsValid)
            {

                PopulateDropdowns(vm);

                if (!vm.PairGroups.Any())
                {
                    vm.PairGroups.Add(new PairGroupViewModel
                    {
                        Divers = new List<PairDiverViewModel>
                        {
                            new PairDiverViewModel(),
                            new PairDiverViewModel()
                        }
                    });
                }

                if (!vm.SurfaceSupportGroups.Any())
                {
                    vm.SurfaceSupportGroups.Add(new DiveGroupViewModel());
                }
                return View(vm);
            }

            //LOGIK FÖR PARDYK
            if (vm.DiveType == DiveType.Pardyk)
            {
                foreach (var pairGroup in vm.PairGroups)
                {
                    divesToSave.Add(CreateDiveFromPairGroup(vm, pairGroup));
                }

            }
            //LOGIK FÖR LYFT FRÅN YTAN
            else if (vm.DiveType == DiveType.LuftFranYtan)
            {

                foreach (var group in vm.SurfaceSupportGroups.Where(g => g.DiverId.HasValue))
                {
                    divesToSave.Add(CreateDiveFromSurfaceGroup(vm, group));
                }

            }

            //Sparar alla skapade dyk till databasen
            _context.Dives.AddRange(divesToSave);
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

            var dive = await _context.Dives
            .Include(d => d.DiveParticipants)
            .FirstOrDefaultAsync(d => d.Id == id);

            if (dive == null)
            {
                return NotFound();
            }

            if (dive.DiveType == DiveType.Pardyk)
            {
                var vm = new EditBuddyDiveViewModel
                {
                    DiveId = dive.Id,
                    Date = dive.Date,
                    StartTime = dive.StartTime,
                    LocationName = dive.LocationName,
                    Latitude = dive.Latitude,
                    Longitude = dive.Longitude,
                    Notes = dive.Notes,
                    DivePurposeId = dive.DivePurposeId,
                    //Dykledare
                    DiveLeaderId = dive.DiveParticipants
                    .First(dp => dp.DiveRoleId == 2).PersonId,
                    //Dykare
                    Divers = dive.DiveParticipants
                    .Where(dp => dp.DiveRoleId == 1)
                    .Select(dp => new PairDiverViewModel
                    {
                        DiverId = dp.PersonId,
                        Depth = dp.Depth,
                        DiveTime = dp.DiveTime,
                        AirPressureBefore = dp.AirPressureBefore,
                        AirPressureAfter = dp.AirPressureAfter
                    }).ToList()
                };

                PopulateBuddyEditDropdowns(vm);

                return View("EditBuddyDive", vm);
            }
            else
            {
                var diver = dive.DiveParticipants.First(dp => dp.DiveRoleId == 1);
                var support = dive.DiveParticipants.First(dp => dp.DiveRoleId == 3);

                var vm = new EditSurfaceSupportDiveViewModel
                {
                    DiveId = dive.Id,
                    Date = dive.Date,
                    StartTime = dive.StartTime,
                    LocationName = dive.LocationName,
                    Latitude = dive.Latitude,
                    Longitude = dive.Longitude,
                    Notes = dive.Notes,
                    DivePurposeId = dive.DivePurposeId,
                    //Dykledare
                    DiveLeaderId = dive.DiveParticipants
                        .First(dp => dp.DiveRoleId == 2).PersonId,
                    //Dykare
                    DiverId = diver.PersonId,
                    SurfaceSupportId = support.PersonId,
                    Depth = diver.Depth,
                    DiveTime = diver.DiveTime,
                    AirPressureBefore = diver.AirPressureBefore,
                    AirPressureAfter = diver.AirPressureAfter
                };

                PopulateSurfaceEditDropdowns(vm);

                return View("EditSurfaceSupportDive", vm);
            }
        }

        // POST: Dive/Edit/5
        //REDIGERA PARDYK
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBuddyDive(EditBuddyDiveViewModel vm)
        {
            var dive = await _context.Dives
                .Include(d => d.DiveParticipants)
                .FirstOrDefaultAsync(d => d.Id == vm.DiveId);

            if (dive == null) return NotFound();

            //VALIDERING
            if (vm.DiveLeaderId <= 0)
                ModelState.AddModelError("", "Dykledare måste väljas.");

            if (string.IsNullOrWhiteSpace(vm.LocationName))
                ModelState.AddModelError("", "Plats måste anges.");

            if (vm.Date == default)
                ModelState.AddModelError("", "Datum måste anges.");

            if (vm.StartTime == default)
                ModelState.AddModelError("", "Starttid måste anges.");

            if (vm.Divers == null || vm.Divers.Count < 2)
                ModelState.AddModelError("", "Minst två dykare krävs.");

            var ids = new List<int>();

            if (vm.DiveLeaderId.HasValue)
                ids.Add(vm.DiveLeaderId.Value);

            foreach (var d in vm.Divers ?? new())
            {
                if (!d.DiverId.HasValue)
                    ModelState.AddModelError("", "Alla dykare måste väljas.");

                if (!d.Depth.HasValue || d.Depth < 1)
                    ModelState.AddModelError("", "Djup måste anges.");

                if (!d.DiveTime.HasValue || d.DiveTime < 1)
                    ModelState.AddModelError("", "Dyktid måste anges.");

                if (!d.AirPressureBefore.HasValue)
                    ModelState.AddModelError("", "Luft före måste anges.");

                if (!d.AirPressureAfter.HasValue)
                    ModelState.AddModelError("", "Luft efter måste anges.");

                if (d.DiverId.HasValue)
                    ids.Add(d.DiverId.Value);
            }

            if (ids.Count != ids.Distinct().Count())
                ModelState.AddModelError("", "Samma person kan inte ha flera roller i samma dyk.");

            if (!ModelState.IsValid)
            {
                PopulateBuddyEditDropdowns(vm);
                return View("EditBuddyDive", vm);
            }

            //UPPDATERING
            // Uppdaterar basic info
            dive.Date = vm.Date;
            dive.StartTime = vm.StartTime;
            dive.LocationName = vm.LocationName;
            dive.Latitude = vm.Latitude;
            dive.Longitude = vm.Longitude;
            dive.Notes = vm.Notes;
            dive.DivePurposeId = vm.DivePurposeId;

            // Tar bort gamla participants
            _context.DiveParticipants.RemoveRange(dive.DiveParticipants);

            // Lägger till ny dykledare
            dive.DiveParticipants.Add(new DiveParticipant
            {
                PersonId = vm.DiveLeaderId!.Value,
                DiveRoleId = 2
            });

            // Lägger till dykare
            foreach (var diver in vm.Divers)
            {
                var exposureTime = CalculateExposureTime(diver.Depth!.Value, diver.DiveTime!.Value);

                dive.DiveParticipants.Add(new DiveParticipant
                {
                    PersonId = diver.DiverId!.Value,
                    DiveRoleId = 1,
                    Depth = diver.Depth,
                    ExposureTime = exposureTime,
                    DiveTime = diver.DiveTime,
                    AirPressureBefore = diver.AirPressureBefore,
                    AirPressureAfter = diver.AirPressureAfter
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //REDIGERA LUFT FRÅN YTAN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSurfaceSupportDive(EditSurfaceSupportDiveViewModel vm)
        {
            var dive = await _context.Dives
                .Include(d => d.DiveParticipants)
                .FirstOrDefaultAsync(d => d.Id == vm.DiveId);

            if (dive == null) return NotFound();

            // VALIDERING
            if (vm.DiveLeaderId <= 0)
                ModelState.AddModelError("", "Dykledare måste väljas.");

            if (!vm.DiverId.HasValue)
                ModelState.AddModelError("", "Dykare måste väljas.");

            if (!vm.SurfaceSupportId.HasValue)
                ModelState.AddModelError("", "Dykskötare måste väljas.");

            if (!vm.Depth.HasValue || vm.Depth < 1)
                ModelState.AddModelError("", "Djup måste anges.");

            if (!vm.DiveTime.HasValue || vm.DiveTime < 1)
                ModelState.AddModelError("", "Dyktid måste anges.");

            var ids = new List<int>();

            if (vm.DiveLeaderId.HasValue)
                ids.Add(vm.DiveLeaderId.Value);

            if (vm.DiverId.HasValue)
                ids.Add(vm.DiverId.Value);

            if (vm.SurfaceSupportId.HasValue)
                ids.Add(vm.SurfaceSupportId.Value);

            if (ids.Count != ids.Distinct().Count())
                ModelState.AddModelError("", "Samma person kan inte ha flera roller i samma dyk.");

            if (!ModelState.IsValid)
            {
                PopulateSurfaceEditDropdowns(vm);
                return View("EditSurfaceSupportDive", vm);
            }

            //UPPDATERING
            //Uppdaterar basic info
            dive.Date = vm.Date;
            dive.StartTime = vm.StartTime;
            dive.LocationName = vm.LocationName;
            dive.Latitude = vm.Latitude;
            dive.Longitude = vm.Longitude;
            dive.Notes = vm.Notes;
            dive.DivePurposeId = vm.DivePurposeId;

            // Tar bort gamla participants
            _context.DiveParticipants.RemoveRange(dive.DiveParticipants);

            dive.DiveParticipants.Add(new DiveParticipant
            {
                PersonId = vm.DiveLeaderId!.Value,
                DiveRoleId = 2
            });

            var exposureTime = CalculateExposureTime(vm.Depth!.Value, vm.DiveTime!.Value);

            dive.DiveParticipants.Add(new DiveParticipant
            {
                PersonId = vm.DiverId!.Value,
                DiveRoleId = 1,
                Depth = vm.Depth,
                ExposureTime = exposureTime,
                DiveTime = vm.DiveTime,
                AirPressureBefore = vm.AirPressureBefore,
                AirPressureAfter = vm.AirPressureAfter
            });

            dive.DiveParticipants.Add(new DiveParticipant
            {
                PersonId = vm.SurfaceSupportId!.Value,
                DiveRoleId = 3
            });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Dive/Delete/5
        // GET: Dive/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dive = await _context.Dives
                .Include(d => d.DiveParticipants)
                    .ThenInclude(dp => dp.Person)
                .Include(d => d.DiveParticipants)
                    .ThenInclude(dp => dp.DiveRole)
                .Include(d => d.DivePurpose)
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
                _context.Dives.Remove(dive);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiveExists(int id)
        {
            return _context.Dives.Any(e => e.Id == id);
        }

        //Metod för att skapa ett grundobjekt för ett nytt dyk.
        //Bra när flera dyk skapas samtidigt (Flera dykare med olika tider i samma formulär)
        private Dive CreateBaseDive(CreateDiveViewModel vm)
        {
            return new Dive
            {
                Date = vm.Date.Date,
                StartTime = vm.StartTime,
                LocationName = vm.LocationName,
                Latitude = vm.Latitude,
                Longitude = vm.Longitude,
                Notes = vm.Notes,
                DiveType = vm.DiveType,
                DivePurposeId = vm.DivePurposeId,
                DiveParticipants = new List<DiveParticipant>()
            };
        }

        //SKAPA DYKDATA FÖR PARDYK
        private Dive CreateDiveFromPairGroup(CreateDiveViewModel vm, PairGroupViewModel pairGroup)
        {
            var dive = CreateBaseDive(vm);

            //Lägger till vald dykledare
            dive.DiveParticipants?.Add(new DiveParticipant
            {
                PersonId = vm.DiveLeaderId!.Value,
                DiveRoleId = 2
            });

            //Lägger till valda dykare
            foreach (var diver in pairGroup.Divers.Where(d => d.DiverId.HasValue))
            {
                var exposureTime = CalculateExposureTime(diver.Depth!.Value, diver.DiveTime!.Value);

                dive.DiveParticipants?.Add(new DiveParticipant
                {
                    PersonId = diver.DiverId!.Value,
                    DiveRoleId = 1,
                    Depth = diver.Depth,
                    ExposureTime = exposureTime,
                    DiveTime = diver.DiveTime,
                    AirPressureBefore = diver.AirPressureBefore,
                    AirPressureAfter = diver.AirPressureAfter,

                });
            }

            return dive;
        }

        //SKAPA DYKDATA FÖR LUFT FRÅN UTAN
        private Dive CreateDiveFromSurfaceGroup(CreateDiveViewModel vm, DiveGroupViewModel group)
        {
            var dive = CreateBaseDive(vm);

            //Lägger till vald dykledare
            dive.DiveParticipants?.Add(new DiveParticipant
            {
                PersonId = vm.DiveLeaderId!.Value,
                DiveRoleId = 2
            });

            var exposureTime = CalculateExposureTime(group.Depth!.Value, group.DiveTime!.Value);

            //Lägger till vald dykare
            dive.DiveParticipants?.Add(new DiveParticipant
            {
                PersonId = group.DiverId!.Value,
                DiveRoleId = 1,
                Depth = group.Depth,
                ExposureTime = exposureTime,
                DiveTime = group.DiveTime,
                AirPressureBefore = group.AirPressureBefore,
                AirPressureAfter = group.AirPressureAfter,

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

            return dive;
        }

        private int CalculateExposureTime(double depth, int diveTime)
        {
            var extraMinutes = (int)Math.Ceiling(depth / 9.0);
            return diveTime - extraMinutes;
        }
    }
}
