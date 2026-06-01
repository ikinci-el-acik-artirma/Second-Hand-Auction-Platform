const listingGrid = document.querySelector("#listingGrid");
const listingMessage = document.querySelector("#listingMessage");
const sessionStatus = document.querySelector("#sessionStatus");
const detailDialog = document.querySelector("#detailDialog");
const toast = document.querySelector("#toast");

const currencyFormatter = new Intl.NumberFormat("tr-TR", {
    style: "currency",
    currency: "TRY"
});

const dateFormatter = new Intl.DateTimeFormat("tr-TR", {
    dateStyle: "medium",
    timeStyle: "short"
});

async function request(url, options = {}) {
    const response = await fetch(url, {
        headers: {
            "Content-Type": "application/json",
            ...options.headers
        },
        ...options
    });

    if (!response.ok) {
        const message = await response.text();
        throw new Error(message || "İşlem tamamlanamadı.");
    }

    return response.status === 204 ? null : response.json();
}

function showToast(message, isError = false) {
    toast.textContent = message;
    toast.classList.toggle("toast-error", isError);
    toast.classList.add("toast-visible");

    window.setTimeout(() => {
        toast.classList.remove("toast-visible");
    }, 3500);
}

function renderListings(items) {
    listingGrid.replaceChildren();

    if (items.length === 0) {
        listingMessage.textContent = "Bu kategori için gösterilecek ilan bulunamadı.";
        return;
    }

    listingMessage.textContent = `${items.length} ilan gösteriliyor.`;

    for (const item of items) {
        const card = document.createElement("article");
        card.className = "listing-card";

        const category = document.createElement("span");
        category.className = "category";
        category.textContent = item.category;

        const title = document.createElement("h3");
        title.textContent = item.title;

        const description = document.createElement("p");
        description.textContent = item.description;

        const footer = document.createElement("div");
        footer.className = "card-footer";

        const price = document.createElement("strong");
        price.textContent = currencyFormatter.format(item.startingPrice);

        const button = document.createElement("button");
        button.type = "button";
        button.textContent = "Detayları Gör";
        button.addEventListener("click", () => openDetail(item));

        footer.append(price, button);
        card.append(category, title, description, footer);
        listingGrid.append(card);
    }
}

function openDetail(item) {
    document.querySelector("#detailTitle").textContent = item.title;
    document.querySelector("#detailCategory").textContent = item.category;
    document.querySelector("#detailPrice").textContent = currencyFormatter.format(item.startingPrice);
    document.querySelector("#detailEndDate").textContent = dateFormatter.format(new Date(item.auctionEndDate));
    document.querySelector("#detailDescription").textContent = item.description;
    detailDialog.showModal();
}

async function loadListings(category = "") {
    listingMessage.textContent = "İlanlar yükleniyor...";

    try {
        const query = category ? `?category=${encodeURIComponent(category)}` : "";
        const items = await request(`/api/auctionitems${query}`);
        renderListings(items);
    } catch (error) {
        listingGrid.replaceChildren();
        listingMessage.textContent = "İlanlar yüklenemedi.";
        showToast(error.message, true);
    }
}

document.querySelector("#registerForm").addEventListener("submit", async (event) => {
    event.preventDefault();
    const form = new FormData(event.currentTarget);

    try {
        const user = await request("/api/auth/register", {
            method: "POST",
            body: JSON.stringify(Object.fromEntries(form))
        });

        sessionStatus.textContent = `${user.email} (${user.role})`;
        event.currentTarget.reset();
        showToast("Kayıt işlemi tamamlandı.");
    } catch (error) {
        showToast(error.message, true);
    }
});

document.querySelector("#loginForm").addEventListener("submit", async (event) => {
    event.preventDefault();
    const form = new FormData(event.currentTarget);

    try {
        const user = await request("/api/auth/login", {
            method: "POST",
            body: JSON.stringify(Object.fromEntries(form))
        });

        sessionStatus.textContent = `${user.email} (${user.role})`;
        event.currentTarget.reset();
        showToast("Giriş başarılı.");
    } catch (error) {
        showToast(error.message, true);
    }
});

document.querySelector("#filterForm").addEventListener("submit", (event) => {
    event.preventDefault();
    const form = new FormData(event.currentTarget);
    loadListings(form.get("category").trim());
});

document.querySelector("#clearFilterButton").addEventListener("click", () => {
    document.querySelector("#filterForm").reset();
    loadListings();
});

document.querySelector("#closeDetailButton").addEventListener("click", () => {
    detailDialog.close();
});

loadListings();
