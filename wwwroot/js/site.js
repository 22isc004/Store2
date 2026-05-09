// ===== SNEACKERS STORE - JavaScript Global =====

// Toast de notificaciones
function mostrarNotificacion(mensaje, tipo = 'success') {
    let toast = document.getElementById('toastNotificacion');

    if (!toast) {
        toast = document.createElement('div');
        toast.id = 'toastNotificacion';
        toast.innerHTML = '<i class="fas fa-check-circle"></i><span id="toastMensaje"></span>';
        document.body.appendChild(toast);
    }

    toast.className = `toast-${tipo}`;
    document.getElementById('toastMensaje').textContent = mensaje;
    const iconEl = toast.querySelector('i');
    iconEl.className = tipo === 'success' ? 'fas fa-check-circle' : 'fas fa-exclamation-circle';

    toast.classList.add('visible');
    setTimeout(() => toast.classList.remove('visible'), 3000);
}

// Actualizar el badge del carrito
function actualizarContadorCarrito(count) {
    const badge = document.getElementById('cartCount');
    if (badge) {
        badge.textContent = count;
        badge.style.display = count > 0 ? 'flex' : 'none';
    }
}

// Cargar contador del carrito al iniciar
document.addEventListener('DOMContentLoaded', function () {
    // Obtener contador del carrito
    fetch('/Carrito/Contador')
        .then(r => r.json())
        .then(data => actualizarContadorCarrito(data.count))
        .catch(() => {});

    // Cerrar alertas automáticamente
    const alertas = document.querySelectorAll('.alert-dismissible');
    alertas.forEach(alerta => {
        setTimeout(() => {
            alerta.style.opacity = '0';
            alerta.style.transition = 'opacity 0.5s ease';
            setTimeout(() => alerta.remove(), 500);
        }, 4000);

        const btnClose = alerta.querySelector('.alert-close');
        if (btnClose) {
            btnClose.addEventListener('click', () => alerta.remove());
        }
    });

    // Agregar al carrito con botones globales (en grillas de productos)
    document.querySelectorAll('.btn-add-cart').forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            agregarAlCarrito(this.dataset.id, 1, null, null);
        });
    });

    // Búsqueda en tiempo real
    const searchInput = document.getElementById('searchInput');
    const searchDropdown = document.getElementById('searchResultsDropdown');

    if (searchInput && searchDropdown) {
        let timerBusqueda;

        searchInput.addEventListener('input', function () {
            clearTimeout(timerBusqueda);
            const q = this.value.trim();

            if (q.length < 2) {
                searchDropdown.classList.remove('visible');
                searchDropdown.innerHTML = '';
                return;
            }

            timerBusqueda = setTimeout(() => {
                fetch(`/Catalogo/BusquedaRapida?q=${encodeURIComponent(q)}`)
                    .then(r => r.json())
                    .then(data => {
                        if (data.length === 0) {
                            searchDropdown.innerHTML = '<div class="search-result-item" style="color:#888">Sin resultados</div>';
                        } else {
                            searchDropdown.innerHTML = data.map(p => `
                                <a href="/Catalogo/Detalle/${p.id}" class="search-result-item">
                                    <img src="${p.imagen}" alt="${p.nombre}" />
                                    <div>
                                        <div style="font-weight:600;font-size:14px">${p.nombre}</div>
                                        <div style="font-size:12px;color:#888">${p.marca || ''} — $${p.precio.toFixed(2)}</div>
                                    </div>
                                </a>
                            `).join('');
                        }
                        searchDropdown.classList.add('visible');
                    })
                    .catch(() => {});
            }, 300);
        });

        // Cerrar dropdown al hacer clic fuera
        document.addEventListener('click', e => {
            if (!e.target.closest('.search-container')) {
                searchDropdown.classList.remove('visible');
            }
        });

        // Búsqueda al presionar Enter
        document.getElementById('searchBtn')?.addEventListener('click', () => {
            const q = searchInput.value.trim();
            if (q) window.location.href = `/Catalogo?busqueda=${encodeURIComponent(q)}`;
        });

        searchInput.addEventListener('keydown', e => {
            if (e.key === 'Enter') {
                const q = searchInput.value.trim();
                if (q) window.location.href = `/Catalogo?busqueda=${encodeURIComponent(q)}`;
            }
        });
    }
});

// Función global para agregar al carrito
function agregarAlCarrito(productoId, cantidad, talla, color) {
    fetch('/Carrito/Agregar', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'X-Requested-With': 'XMLHttpRequest'
        },
        body: JSON.stringify({ productoId, cantidad, talla, color })
    })
    .then(r => r.json())
    .then(data => {
        if (data.success) {
            mostrarNotificacion(data.message || 'Agregado al carrito', 'success');
            actualizarContadorCarrito(data.totalItems);
        } else {
            mostrarNotificacion(data.message || 'Error al agregar', 'error');
        }
    })
    .catch(() => mostrarNotificacion('Error de conexión', 'error'));
}
