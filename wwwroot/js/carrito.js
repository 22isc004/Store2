// ===== CARRITO - JavaScript =====
// Manejo del carrito con AJAX, sin recargar la página

document.addEventListener('DOMContentLoaded', function () {
    // Configurar token de anti-falsificación para peticiones POST
    const tokenMeta = document.querySelector('input[name="__RequestVerificationToken"]');
    const token = tokenMeta ? tokenMeta.value : '';

    // Helper para peticiones POST con JSON
    function postJson(url, datos) {
        return fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': token
            },
            body: JSON.stringify(datos)
        }).then(r => r.json());
    }

    // Actualizar total en el resumen del carrito
    function actualizarResumen(subtotal) {
        const envio = subtotal >= 1500 ? 0 : 150;
        const subtotalEl = document.getElementById('resumenSubtotal');
        const envioEl = document.getElementById('resumenEnvio');
        const totalEl = document.getElementById('resumenTotal');

        if (subtotalEl) subtotalEl.textContent = '$' + subtotal.toFixed(2);
        if (envioEl) {
            if (envio === 0) {
                envioEl.innerHTML = '<span class="envio-gratis">GRATIS</span>';
            } else {
                envioEl.textContent = '$' + envio.toFixed(2);
            }
        }
        if (totalEl) totalEl.textContent = '$' + (subtotal + envio).toFixed(2);
    }
});
