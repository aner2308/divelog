const CACHE_NAME = "divelog-v8";

//Filer som ska sparas lokalt vid installation av service workern
const urlsToCache = [
    "/",
    "/offline.html",
    "/manifest.json",

    "/css/site.css",
    "/css/dive.css",
    "/css/admin.css",
    "/css/person.css",
    "/css/layoutHero.css",
    "/css/layoutPattern.css",
    "/css/about.css",

    "/js/site.js",
    "/js/dive-calculations.js",
    "/js/dive-timer.js",

    "/images/hero.jpg",
    "/images/backgroundPattern.png",
    "/images/diver-marker.png",
    "/images/diver-standing.jpg",
    "/favicon.ico",
    "/images/favicon.png",
    "/images/faviconS.png",
    "/images/faviconL.png",
    "/images/logotype.png",
    "/images/reset.png",
    "/images/start.png",
    "/images/stop.png",
];

//Installation av service workern
self.addEventListener("install", event => {

    self.skipWaiting();

    event.waitUntil(
        //Filer cachas för offline-användning.
        caches.open(CACHE_NAME)
            .then(async cache => {
                for (const url of urlsToCache) {
                    try {
                        await cache.add(url);
                    } catch (err) {
                        console.warn("Failed to cache:", url, err);
                    }
                }
            })
    );
});

//Aktivering av service workern
self.addEventListener("activate", event => {

    event.waitUntil(
        (async () => {
            //Hämtar alla befintliga cacher
            const cacheNames = await caches.keys();

            await Promise.all(
                cacheNames.map(cache => {
                    //Ta bort gamla cacher
                    if (cache !== CACHE_NAME) {
                        return caches.delete(cache);
                    }
                })
            );

            //Gör service workern aktiv för alla öppna sidor
            await self.clients.claim();

        })()
    );
});

//Fetch-anrop som hanterar alla nätverksförfrågningar från sidan
self.addEventListener("fetch", event => {

    //Endast GET-anrop hanteras
    if (event.request.method !== "GET") {
        return;
    }

    const requestUrl = new URL(event.request.url);

    //Kontrollerar om förfrågan gäller statiska filer
    const isStaticAsset =

        requestUrl.pathname.startsWith("/css/") ||
        requestUrl.pathname.startsWith("/js/") ||
        requestUrl.pathname.startsWith("/images/") ||
        requestUrl.pathname.startsWith("/lib/") ||
        requestUrl.pathname === "/favicon.ico";

    if (isStaticAsset) {

        event.respondWith(
            (async () => {
                const cache = await caches.open(CACHE_NAME);

                //Hämtar filen från cachen först (om den finns sparad)
                const cachedResponse = await cache.match(event.request);

                //Uppdatera cache i bakgrunden
                const fetchPromise = fetch(event.request)
                    .then(networkResponse => {
                        if (networkResponse.ok) {
                            cache.put(
                                event.request,
                                networkResponse.clone()
                            );
                        }

                        return networkResponse;
                    })
                    .catch(() => null);

                //Returnera cache direkt om den finns
                if (cachedResponse) {
                    return cachedResponse;
                }

                //Annars vänta på nätverket
                const networkResponse = await fetchPromise;

                return networkResponse || Response.error();
            })()
        );

        return;
    }

    //Hantering av sidnavigering
    if (event.request.mode === "navigate") {

        //Om nätverket saknas visas offline-sidan istället
        event.respondWith(
            fetch(event.request)
                .then(response => response)
                .catch(async () => {

                    const cachedOfflinePage =
                        await caches.match("/offline.html");

                    return cachedOfflinePage || Response.error();
                })
        );

        return;
    }

    //Övriga anrop (ej statiska filer eller sidnavigering)
    event.respondWith(

        //Försök använda nätverket först, vid offline-läge hämta från cache
        fetch(event.request)
            .catch(async () => {
                const cachedResponse =
                    await caches.match(event.request);

                return cachedResponse || Response.error();
            })
    );
});