// ── Bar Chart Renderer ────────────────────────────────────────────────────
function renderBarChart(el) {
    const values = el.dataset.values.split(',').map(Number);
    const labels = el.dataset.labels.split(',');
    const color  = el.dataset.color || '#00b4d8';
    const max    = Math.max(...values);

    el.innerHTML = values.map((v, i) => {
        const h = max > 0 ? Math.round((v / max) * 100) : 0;
        return `<div class="bar-wrap">
            <div class="bar-val">${v}</div>
            <div class="bar-item" style="height:${h}%;background:${color}" title="${labels[i]}: ${v}"></div>
            <div class="bar-lbl">${labels[i]}</div>
        </div>`;
    }).join('');
}

document.addEventListener('DOMContentLoaded', () => {
    // Render all bar charts
    document.querySelectorAll('.bar-chart').forEach(el => renderBarChart(el));

    // Auto-dismiss alert banners after 5 seconds
    document.querySelectorAll('.alert-banner').forEach(el => {
        setTimeout(() => {
            el.style.transition = 'opacity .4s';
            el.style.opacity = '0';
            setTimeout(() => el.remove(), 400);
        }, 5000);
    });

    // Active nav highlight based on current URL
    const path = window.location.pathname.toLowerCase();
    document.querySelectorAll('.nav-item').forEach(item => {
        const href = (item.getAttribute('href') || '').toLowerCase();
        if (href && path.startsWith(href) && href !== '/') {
            item.classList.add('active');
        }
    });

    // Sticky topbar shadow on scroll
    const topbar = document.querySelector('.topbar');
    if (topbar) {
        window.addEventListener('scroll', () => {
            topbar.style.boxShadow = window.scrollY > 0
                ? '0 4px 16px rgba(10,22,40,.12)'
                : '0 2px 8px rgba(10,22,40,.06)';
        });
    }
});
