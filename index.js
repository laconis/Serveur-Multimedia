<script>
/* ============================
   1. Récupération des éléments
============================ */
const searchInput = document.querySelector(".inputSearchFilmSeries");
const selCategory = document.querySelector(".sel1");
const selGenre = document.querySelector(".sel2");
const selSort = document.querySelector(".sel3");
const cards = document.querySelectorAll(".super-card");

/* ============================
   2. Fonction principale
============================ */
function filterCards() {
    const searchText = searchInput.value.toLowerCase();
    const category = selCategory.value;
    const genre = selGenre.value;
    const sort = selSort.value;

    let cardArray = Array.from(cards);

    // --- Recherche texte ---
    cardArray.forEach(card => {
        const title = card.querySelector("h3").textContent.toLowerCase();
        const desc = card.querySelector(".desc").textContent.toLowerCase();
        const meta = card.querySelector(".meta").textContent.toLowerCase();

        const matchSearch =
            title.includes(searchText) ||
            desc.includes(searchText) ||
            meta.includes(searchText);

        card.style.display = matchSearch ? "block" : "none";
    });

    // --- Filtre catégorie ---
    if (category !== "Films" && category !== "Séries" && category !== "Documentaires" && category !== "Divers") {
        // rien à filtrer
    } else {
        cardArray.forEach(card => {
            const badge = card.querySelector(".badge-red").textContent.toLowerCase();
            if (!badge.includes(category.toLowerCase())) {
                card.style.display = "none";
            }
        });
    }

    // --- Filtre genre ---
    if (genre !== "Genre") {
        cardArray.forEach(card => {
            const metaGenre = card.querySelector(".meta span:nth-child(2)").textContent;
            if (metaGenre !== genre) {
                card.style.display = "none";
            }
        });
    }

    // --- Tri ---
    if (sort !== "Trier par") {
        const container = document.querySelector("#allData");

        cardArray.sort((a, b) => {
            const noteA = parseFloat(a.querySelector(".badge-dark").textContent.replace("★", ""));
            const noteB = parseFloat(b.querySelector(".badge-dark").textContent.replace("★", ""));

            if (sort === "Note") return noteB - noteA;
            if (sort === "Populaire") return noteB - noteA; // même logique pour l'instant
            if (sort === "Ajout récent") return Math.random() - 0.5; // faute de vraie date
        });

        cardArray.forEach(card => container.appendChild(card));
    }
}

/* ============================
   3. Écouteurs d'événements
============================ */
searchInput.addEventListener("input", filterCards);
selCategory.addEventListener("change", filterCards);
selGenre.addEventListener("change", filterCards);
selSort.addEventListener("change", filterCards);

/////////////////////// 2 ////////////////////

<script>
async function loadFilms() {
    const response = await fetch("getFilms.php");
    const films = await response.json();

    const container = document.querySelector("#allData");
    container.innerHTML = "";

    films.forEach(film => {
        const card = document.createElement("div");
        card.className = "super-card";
        card.style.background = `
            linear-gradient(to top, rgba(0,0,0,0.88), rgba(0,0,0,0.16)),
            url('${film.image_url}') center/cover
        `;

        card.innerHTML = `
            <div class="card-overlay">
                <div class="top-badges">
                    <span class="badge badge-red">${film.categorie}</span>
                    <span class="badge badge-dark">★ ${film.note}</span>
                </div>
                <div class="card-bottom">
                    <h3>${film.titre}</h3>
                    <div class="meta">
                        <span>${film.annee}</span>
                        <span>${film.genre}</span>
                        <span>${film.duree}</span>
                    </div>
                    <div class="desc">${film.description}</div>
                    <div class="card-actions">
                        <button class="mini-btn play">▶ Lecture</button>
                        <button class="mini-btn add">+ Liste</button>
                    </div>
                </div>
            </div>
        `;

        container.appendChild(card);
    });
}

loadFilms();
</script>

  
