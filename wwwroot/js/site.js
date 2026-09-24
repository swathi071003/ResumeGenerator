// ---- App state (mirrors the server-side ResumeModel shape) ----
const state = {
    personalInfo: { fullName: "", email: "", phone: "", location: "", linkedIn: "", website: "" },
    summary: "",
    experience: [],
    education: [],
    skills: [],
    projects: []
};

let atsDebounceTimer = null;

// ---- Personal info + summary bindings ----
const fieldMap = {
    fullName: "fullName", email: "email", phone: "phone",
    location: "location", linkedin: "linkedIn", website: "website"
};
Object.keys(fieldMap).forEach(id => {
    document.getElementById(id).addEventListener("input", e => {
        state.personalInfo[fieldMap[id]] = e.target.value;
        onStateChanged();
    });
});
document.getElementById("summary").addEventListener("input", e => {
    state.summary = e.target.value;
    onStateChanged();
});

// ---- Experience ----
function addExperience() {
    state.experience.push({ role: "", company: "", startDate: "", endDate: "", bullets: [] });
    renderExperience();
    onStateChanged();
}
function renderExperience() {
    const container = document.getElementById("experienceList");
    const tpl = document.getElementById("experienceTemplate").innerHTML;
    container.innerHTML = state.experience.map((item, i) => tpl.replaceAll("__INDEX__", i)).join("");

    container.querySelectorAll(".entry-card").forEach((card, i) => {
        card.querySelector(".exp-role").value = state.experience[i].role;
        card.querySelector(".exp-company").value = state.experience[i].company;
        card.querySelector(".exp-start").value = state.experience[i].startDate;
        card.querySelector(".exp-end").value = state.experience[i].endDate;
        card.querySelector(".exp-bullets").value = state.experience[i].bullets.join("\n");

        card.querySelector(".exp-role").addEventListener("input", e => { state.experience[i].role = e.target.value; onStateChanged(); });
        card.querySelector(".exp-company").addEventListener("input", e => { state.experience[i].company = e.target.value; onStateChanged(); });
        card.querySelector(".exp-start").addEventListener("input", e => { state.experience[i].startDate = e.target.value; onStateChanged(); });
        card.querySelector(".exp-end").addEventListener("input", e => { state.experience[i].endDate = e.target.value; onStateChanged(); });
        card.querySelector(".exp-bullets").addEventListener("input", e => {
            state.experience[i].bullets = e.target.value.split("\n").filter(b => b.trim() !== "");
            onStateChanged();
        });
    });
}

// ---- Education ----
function addEducation() {
    state.education.push({ degree: "", school: "", startDate: "", endDate: "" });
    renderEducation();
    onStateChanged();
}
function renderEducation() {
    const container = document.getElementById("educationList");
    const tpl = document.getElementById("educationTemplate").innerHTML;
    container.innerHTML = state.education.map((item, i) => tpl.replaceAll("__INDEX__", i)).join("");

    container.querySelectorAll(".entry-card").forEach((card, i) => {
        card.querySelector(".edu-degree").value = state.education[i].degree;
        card.querySelector(".edu-school").value = state.education[i].school;
        card.querySelector(".edu-start").value = state.education[i].startDate;
        card.querySelector(".edu-end").value = state.education[i].endDate;

        card.querySelector(".edu-degree").addEventListener("input", e => { state.education[i].degree = e.target.value; onStateChanged(); });
        card.querySelector(".edu-school").addEventListener("input", e => { state.education[i].school = e.target.value; onStateChanged(); });
        card.querySelector(".edu-start").addEventListener("input", e => { state.education[i].startDate = e.target.value; onStateChanged(); });
        card.querySelector(".edu-end").addEventListener("input", e => { state.education[i].endDate = e.target.value; onStateChanged(); });
    });
}

// ---- Skills ----
document.getElementById("skillsInput").addEventListener("keydown", e => {
    if (e.key === "Enter" && e.target.value.trim() !== "") {
        e.preventDefault();
        state.skills.push(e.target.value.trim());
        e.target.value = "";
        renderSkills();
        onStateChanged();
    }
});
function renderSkills() {
    const container = document.getElementById("skillsList");
    container.innerHTML = state.skills.map((s, i) =>
        `<span class="chip">${escapeHtml(s)}<button onclick="removeSkill(${i})">&times;</button></span>`
    ).join("");
}
function removeSkill(i) {
    state.skills.splice(i, 1);
    renderSkills();
    onStateChanged();
}

// ---- Projects ----
function addProject() {
    state.projects.push({ name: "", description: "", link: "" });
    renderProjects();
    onStateChanged();
}
function renderProjects() {
    const container = document.getElementById("projectsList");
    const tpl = document.getElementById("projectTemplate").innerHTML;
    container.innerHTML = state.projects.map((item, i) => tpl.replaceAll("__INDEX__", i)).join("");

    container.querySelectorAll(".entry-card").forEach((card, i) => {
        card.querySelector(".proj-name").value = state.projects[i].name;
        card.querySelector(".proj-desc").value = state.projects[i].description;
        card.querySelector(".proj-link").value = state.projects[i].link;

        card.querySelector(".proj-name").addEventListener("input", e => { state.projects[i].name = e.target.value; onStateChanged(); });
        card.querySelector(".proj-desc").addEventListener("input", e => { state.projects[i].description = e.target.value; onStateChanged(); });
        card.querySelector(".proj-link").addEventListener("input", e => { state.projects[i].link = e.target.value; onStateChanged(); });
    });
}

// ---- Generic remove handler used by template Remove buttons ----
function removeEntry(section, index) {
    state[section].splice(index, 1);
    if (section === "experience") renderExperience();
    if (section === "education") renderEducation();
    if (section === "projects") renderProjects();
    onStateChanged();
}

// ---- Live preview ----
function renderPreview() {
    const p = state.personalInfo;
    const contactParts = [p.email, p.phone, p.location, p.linkedIn, p.website].filter(Boolean);

    let html = `<h1>${escapeHtml(p.fullName) || '<span class="empty-hint">Your Name</span>'}</h1>`;
    html += `<div class="contact-line">${contactParts.map(escapeHtml).join(" &nbsp;|&nbsp; ")}</div>`;

    if (state.summary.trim()) {
        html += `<h3>Summary</h3><div>${escapeHtml(state.summary)}</div>`;
    }

    if (state.experience.length) {
        html += `<h3>Experience</h3>`;
        state.experience.forEach(e => {
            html += `<div class="entry-title"><span>${escapeHtml(e.role)}${e.company ? " — " + escapeHtml(e.company) : ""}</span><span class="entry-date">${escapeHtml(e.startDate)} - ${escapeHtml(e.endDate)}</span></div>`;
            if (e.bullets.length) {
                html += `<ul>${e.bullets.map(b => `<li>${escapeHtml(b)}</li>`).join("")}</ul>`;
            }
        });
    }

    if (state.education.length) {
        html += `<h3>Education</h3>`;
        state.education.forEach(ed => {
            html += `<div class="entry-title"><span>${escapeHtml(ed.degree)}${ed.school ? " — " + escapeHtml(ed.school) : ""}</span><span class="entry-date">${escapeHtml(ed.startDate)} - ${escapeHtml(ed.endDate)}</span></div>`;
        });
    }

    if (state.skills.length) {
        html += `<h3>Skills</h3><div>${state.skills.map(escapeHtml).join(" &nbsp;•&nbsp; ")}</div>`;
    }

    if (state.projects.length) {
        html += `<h3>Projects</h3>`;
        state.projects.forEach(p => {
            html += `<div class="entry-title"><span>${escapeHtml(p.name)}</span></div><div>${escapeHtml(p.description)}</div>`;
        });
    }

    document.getElementById("resumePreview").innerHTML = html;
}

function escapeHtml(str) {
    if (!str) return "";
    return str.replace(/[&<>"']/g, m => ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }[m]));
}

// ---- ATS score (debounced call to server) ----
function onStateChanged() {
    renderPreview();
    clearTimeout(atsDebounceTimer);
    atsDebounceTimer = setTimeout(updateAtsScore, 400);
}

async function updateAtsScore() {
    try {
        const res = await fetch("/Resume/CalculateAts", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(state)
        });
        if (!res.ok) return;
        const result = await res.json();
        renderAtsResult(result);
    } catch (err) {
        console.error("ATS score request failed", err);
    }
}

function renderAtsResult(result) {
    const fill = document.getElementById("atsBarFill");
    const text = document.getElementById("atsScoreText");
    const tipsList = document.getElementById("atsTips");

    fill.style.width = result.score + "%";
    text.textContent = result.score + "%";

    if (result.score >= 80) fill.style.background = "#2fa84f";
    else if (result.score >= 50) fill.style.background = "#e8a23d";
    else fill.style.background = "#e5484d";

    tipsList.innerHTML = result.tips.map(t => `<li>${escapeHtml(t)}</li>`).join("");
}

// ---- PDF download ----
document.getElementById("downloadBtn").addEventListener("click", async () => {
    const btn = document.getElementById("downloadBtn");
    const originalText = btn.textContent;
    btn.textContent = "Generating...";
    btn.disabled = true;
    try {
        const res = await fetch("/Resume/DownloadPdf", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(state)
        });
        if (!res.ok) throw new Error("PDF generation failed");
        const blob = await res.blob();
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement("a");
        a.href = url;
        a.download = (state.personalInfo.fullName || "resume").replace(/\s+/g, "_") + "_Resume.pdf";
        document.body.appendChild(a);
        a.click();
        a.remove();
        window.URL.revokeObjectURL(url);
    } catch (err) {
        alert("Could not generate PDF. Please try again.");
        console.error(err);
    } finally {
        btn.textContent = originalText;
        btn.disabled = false;
    }
});

// ---- Init ----
renderPreview();
updateAtsScore();
