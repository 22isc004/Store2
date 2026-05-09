// ===== PANEL ADMIN - JavaScript =====

document.addEventListener('DOMContentLoaded', function () {
    // Toggle del sidebar en mobile
    const mobileToggle = document.getElementById('mobileToggle');
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebar = document.getElementById('adminSidebar');

    if (mobileToggle && sidebar) {
        mobileToggle.addEventListener('click', () => {
            sidebar.classList.toggle('open');
        });
    }

    // Cerrar alertas automáticamente
    document.querySelectorAll('.alert-dismissible .alert-close').forEach(btn => {
        btn.addEventListener('click', () => btn.closest('.alert').remove());
    });

    document.querySelectorAll('.alert-dismissible').forEach(alerta => {
        setTimeout(() => {
            alerta.style.opacity = '0';
            alerta.style.transition = 'opacity 0.5s';
            setTimeout(() => alerta.remove(), 500);
        }, 5000);
    });
});
