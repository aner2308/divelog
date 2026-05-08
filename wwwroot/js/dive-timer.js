let timers = {};

//Funktion som skapar timer
function createTimer(containerId, label, groupId, type) {

    //Skapar unikt ett unikt ID för timern
    const id = crypto.randomUUID();

    //Timern sparas i ett objekt
    timers[id] = {
        start: null,
        elapsed: 0,
        //Referens till ett aktivt setInterval
        interval: null,
        groupId: groupId,
        type: type
    };

    const row = document.createElement("div");
    row.className = "timer-row";
    row.dataset.timerId = id;
    row.innerHTML = `
        <span class="timer-label">${label}</span>
        <span id="time-${id}">00:00</span>

        <button type="button" onclick="startTimer('${id}')">Start</button>
        <button type="button" onclick="stopTimer('${id}')">Stopp</button>
        <button type="button" onclick="resetTimer('${id}')">Nollställ</button>
    `;

    document.getElementById(containerId).appendChild(row);
}

//Funktion som startar timer
function startTimer(id) {

    //Om id saknas eller om man klickar på start flera gånger stoppas funktionen
    const t = timers[id];
    if (!t || t.interval) return;

    //Om timern stoppas och startas igen så fortsätter den från tidigare tid
    t.start = Date.now() - t.elapsed;

    //Körs varje sekund och uppdaterar timern på sidan
    t.interval = setInterval(() => {

        t.elapsed = Date.now() - t.start;

        const sec = Math.floor(t.elapsed / 1000);
        const min = Math.floor(sec / 60);

        const el = document.getElementById(`time-${id}`);

        if (!el) return;

        el.innerText =
            `${String(min).padStart(2, '0')}:${String(sec % 60).padStart(2, '0')}`;

    }, 1000);
}

//Funktion som stoppar timer
function stopTimer(id) {

    //Om id saknas stoppas funktionen
    const t = timers[id];
    if (!t) return;

    //Stoppar den pågående timern
    clearInterval(t.interval);
    t.interval = null;

    //Variabel som sparar tiden i minuter, påbörjad minut avrundas uppåt
    const minutes = Math.ceil(t.elapsed / 60000);

    const group = document.getElementById(t.groupId);

    if (!group) return;

    //Kopplar tiden till formuläret (Dykare + Dykarskötare)
    if (group.classList.contains("surface-support-card")) {

        const input = group.querySelector("input[name$='.DiveTime']");
        if (input) input.value = minutes;
    }

    //Kopplar tiden till formuläret (Pardyk)
    if (group.classList.contains("pair-group")) {

        const inputs = group.querySelectorAll("input[name$='.DiveTime']");
        inputs.forEach(i => i.value = minutes);
    }
}

//Funktion som nollställer timer
function resetTimer(id) {

    const t = timers[id];
    if (!t) return;

    //Stoppar den pågående timern
    if (t.interval) {
        clearInterval(t.interval);
        t.interval = null;
    }

    //Nollställ timer
    t.start = null;
    t.elapsed = 0;

    //Uppdatera texten på sidan
    const el = document.getElementById(`time-${id}`);
    if (el) {
        el.innerText = "00:00";
    }

    //Rensar inputfält kopplat till timern
    const group = document.getElementById(t.groupId);
    if (!group) return;

    if (group.classList.contains("surface-support-card")) {

        const input = group.querySelector("input[name$='.DiveTime']");
        if (input) input.value = "";
    }

    if (group.classList.contains("pair-group")) {

        const inputs = group.querySelectorAll("input[name$='.DiveTime']");
        inputs.forEach(i => i.value = "");
    }
}

//Funktion som initierar en timer för den grupp som finns från början
function initExistingTimers() {

    //Dykare + Dykarskötare
    document.querySelectorAll(".surface-support-card").forEach((group, index) => {

        const id = group.id;

        createTimer("surface-timer-container", `Dyk ${index + 1}`, id, "dive");

        createTimer("ascent-timer-container", `Uppstigning ${index + 1}`, id, "ascent");
    });

    //Pardyk
    document.querySelectorAll(".pair-group").forEach((group, index) => {

        const id = group.id;

        createTimer("buddy-timer-container", `Dykpar ${index + 1}`, id);
    });
}

//Funktion som tar bort timer-raden för grupper som tas bort
function removeTimerByGroup(groupId) {

    for (const id in timers) {
        if (timers[id].groupId === groupId) {

            clearInterval(timers[id].interval);

            const el = document.querySelector(`[data-timer-id='${id}']`);
            if (el) el.remove();

            delete timers[id];
        }
    }
}

//Funktion som uppdaterar timer labels för dyk med Dykare + Dykarskötare
function updateSurfaceTimerLabels() {

    document.querySelectorAll(".surface-support-card").forEach((group, index) => {

        const diverSelect = group.querySelector("select[name$='.DiverId']");

        let diverText = `Dykare ${index + 1}`;

        //Om dykare är vald i selectboxen
        if (diverSelect && diverSelect.value !== "") {

            const selectedOption =
                diverSelect.options[diverSelect.selectedIndex];

            //Exempelvis "117 - Emma Larsson"
            const fullText = selectedOption.text;

            //Tar bara signaturen före "-"
            diverText = fullText.split("-")[0].trim();
        }

        //Hitta timers för gruppen
        for (const id in timers) {

            const timer = timers[id];

            if (timer.groupId === group.id) {

                const row = document.querySelector(
                    `[data-timer-id='${id}']`
                );

                if (!row) continue;

                const label = row.querySelector(".timer-label");

                if (!label) continue;

                //Uppdatera text på timer för dyktid
                if (timer.type === "dive") {
                    label.innerText = diverText;
                }

                //Uppdatera text på timer för uppstigningstid
                if (timer.type === "ascent") {
                    label.innerText = diverText;
                }
            }
        }
    });
}

//Funktion som byter namn på timers efter borttagning (Pardyk)
function renumberPairGroups() {

    document.querySelectorAll(".pair-group").forEach((group, index) => {

        const title = group.querySelector("h5");

        if (title) {
            title.innerText = `Dykpar ${index + 1}`;
        }
    });
}

//Funktion som uppdaterar timer labels för pardyk
function updateBuddyTimerLabels() {

    document.querySelectorAll(".pair-group").forEach((group, index) => {

        const diverSelects = group.querySelectorAll(
            "select[name*='.DiverId']"
        );

        let diverNames = [];

        diverSelects.forEach(select => {

            if (select.value !== "") {

                const selectedOption =
                    select.options[select.selectedIndex];

                //Exempelvis "117 - Emma Larsson"
                const fullText = selectedOption.text;

                //Tar bara signaturen
                const signature = fullText.split("-")[0].trim();

                diverNames.push(signature);
            }
        });

        //Om inga dykare valda
        let labelText =
            diverNames.length > 0
                ? diverNames.join(", ")
                : `Dykpar ${index + 1}`;

        //Uppdatera timer för gruppen
        for (const id in timers) {

            const timer = timers[id];

            if (timer.groupId === group.id) {

                const row = document.querySelector(
                    `[data-timer-id='${id}']`
                );

                if (!row) continue;

                const label = row.querySelector(".timer-label");

                if (!label) continue;

                //Uppdatera texten på timer för pardyk
                label.innerText = labelText;
            }
        }
    });
}